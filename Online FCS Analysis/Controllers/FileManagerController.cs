using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FlowCytometry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Online_FCS_Analysis.Models;
using Online_FCS_Analysis.Models.Entities;
using Online_FCS_Analysis.Utilities;

namespace Online_FCS_Analysis.Controllers
{
    [Authorize]
    public class FileManagerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOptions<AppSettings> _appSettings;
        public FileManagerController(ApplicationDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Upload WBC Files
        public async Task<IActionResult> UploadWbc()
        {
            var files = HttpContext.Request.Form.Files;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string orgFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fileName = Path.GetRandomFileName() + "_" + orgFileName;
                    string fullPath = Path.Combine(Constants.wbc_fcs_full_path, fileName);
                    using (var stream = System.IO.File.Create(fullPath))
                    {
                        await file.CopyToAsync(stream);

                    }

                    if (!System.IO.File.Exists(fullPath))
                        continue;

                    
                    try
                    {
                        using (FCMeasurement fcsMeasurement = new FCMeasurement(fullPath))
                        {
                            int nomenclature = fcsMeasurement.GetNomenclature();
                            string wbc_type = GetWBCType(fcsMeasurement, nomenclature);
                            if (nomenclature < 0)
                            {
                                System.IO.File.Delete(fullPath);
                                continue;
                            }
                            await Task.Run(() => StoreDynamicGateResultsOnDisk(fcsMeasurement, fileName, nomenclature));
                            await Task.Run(() => CreateNewWBC(orgFileName, fileName, nomenclature, wbc_type));
                        }
                    }
                    catch
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            return Ok();
        }

        private string GetWBCType(FCMeasurement fcsMeasurement, int nomenclature)
        {
            string wbc_type = Constants.FCS_TYPE_WBC;
            if (fcsMeasurement.IsEOS(Constants.WBC_NOMENCLATURES[nomenclature]))
                wbc_type = Constants.FCS_TYPE_WBC_EOS;
            if (fcsMeasurement.IsAML())
                wbc_type = Constants.FCS_TYPE_WBC_AML;
            return wbc_type;
        }

        // Save WBC Information on DB
        private void CreateNewWBC(string orgFileName, string fileName, int nomenclature, string wbc_type)
        {
            FCSModel newWbc = new FCSModel
            {
                fcs_name = orgFileName,
                fcs_file_name = fileName,
                fcs_path = Constants.wbc_fcs_path+ fileName,
                user_id = Convert.ToInt32(User.FindFirst(Constants.CLAIM_TYPE_USER_ID).Value),
                fcs_type = wbc_type,
                wbc_3cells = Constants.wbc_3cell_path + fileName + ".obj",
                wbc_gate2 = wbc_type == Constants.FCS_TYPE_WBC_AML ? "" : Constants.wbc_gate2_path + fileName + ".obj",
                wbc_heatmap = Constants.wbc_heatmap_path + fileName + ".png",
                nomenclature = nomenclature
            };
            _dbContext.FCSs.Add(newWbc);
            _dbContext.SaveChangesAsync();
        }

        // Save Calculate results for wbc on DISK.
        private void StoreDynamicGateResultsOnDisk(FCMeasurement fcsMeasurement, string fileName, int nomenclature)
        {
            #region initialize Objects
            string gate3File = Path.Combine(Constants.wwwroot_abs_path, _appSettings.Value.defaultGateSetting.path, _appSettings.Value.defaultGateSetting.gate3);
            string cellsFile = Path.Combine(Constants.wbc_3cell_full_path, fileName + ".obj");
            string gate2File = Path.Combine(Constants.wbc_gate2_full_path, fileName + ".obj");
            string heatmapFile = Path.Combine(Constants.wbc_heatmap_full_path, fileName + ".png");
            string channelNomenclature = Constants.WBC_NOMENCLATURES[nomenclature];
            string channel1 = FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);
            string channel2 = FCMeasurement.GetChannelName("SSCpeak", channelNomenclature);
            string channel3 = FCMeasurement.GetChannelName("FCS1area", channelNomenclature);
            List<double[]> arrData = FCMeasurement.GetChannelData(fcsMeasurement, channel1, channel2);
            List<double[]> arrGate2Data = new List<double[]>();

            if (fcsMeasurement.IsAML())
            {
                gate3File = Path.Combine(Constants.wwwroot_abs_path, _appSettings.Value.defaultGateSetting.path, _appSettings.Value.defaultGateSetting.aml_gate3);
            } else
            {
                arrGate2Data = FCMeasurement.GetChannelData(fcsMeasurement, channel3, channel1);
            }
            List<Polygon> polygons = FCMeasurement.loadPolygon(gate3File);
            
            if (polygons.Count < 3)
            {
                return;
            }

            int i, x, y;
            for (i = 0; i < 3; i++)
            {
                FlowCytometry.CustomCluster.Global.CELL_CENTER[i] = FlowCytometry.CustomCluster.Global.GetCentroid(polygons[i].poly);
            }

            FlowCytometry.CustomCluster.Global.diff3_enable = true;
            FlowCytometry.CustomCluster.Global.T_Y_1 = (int)polygons[2].poly[0].Y;
            FlowCytometry.CustomCluster.Global.T_Y_2 = (int)polygons[0].poly[0].Y;
            FlowCytometry.CustomCluster.Global.is_aml = fcsMeasurement.IsAML();

            #endregion initialize Objects

            #region Calculate gate2 and save on disk
            if (!fcsMeasurement.IsAML())
            {
                double[] singletsFit = FCMeasurement.NewLinearRegression(arrGate2Data.ToArray());
                double slope = singletsFit[0];
                double intercept = singletsFit[1];
                double delta_slope = 0.3;
                double delta_intercept = 0.5;
                double minDelta = 7000 * slope * delta_slope;
                int yMax = fcsMeasurement.Channels[channel1].Range;
                int xMax = fcsMeasurement.Channels[channel3].Range;

                List<PointF> gate2Points = new List<PointF>();
                gate2Points.Add(new PointF(0, 0));
                gate2Points.Add(new PointF((float)(intercept * (1 + delta_intercept) + minDelta), 0));
                gate2Points.Add(new PointF((float)(slope * 7000 + intercept * (1 + delta_intercept) + minDelta), 7000));
                gate2Points.Add(new PointF((float)((1 + delta_slope) * slope * yMax + intercept * (1 + delta_intercept)), yMax));
                gate2Points.Add(new PointF((float)((1 - delta_slope) * slope * yMax + intercept * (1 - delta_intercept)), yMax));
                gate2Points.Add(new PointF((float)(slope * 7000 + intercept * (1 - delta_intercept) - minDelta), 7000));
                gate2Points.Add(new PointF(0, (float)((minDelta - intercept * (1 - delta_intercept)) / slope)));
                gate2Points.Add(new PointF(0, 0));

                List<Polygon> gate2Polygon = new List<Polygon>();
                gate2Polygon.Add(new Polygon(gate2Points.ToArray(), Color.OrangeRed));

                Global.WriteToBinaryFile(gate2File, gate2Polygon);
            }
            #endregion Calculate gate2 and save on disk

            //#region Generate Heatmap Image and save on disk
            using (FlowCytometry.CustomCluster.Custom_Meanshift meanshift = new FlowCytometry.CustomCluster.Custom_Meanshift(arrData))
            {
                meanshift.CalculateKDE();
                int nGridCnt = meanshift.nGridCnt;
                using (Bitmap bmp = new Bitmap(nGridCnt, nGridCnt))
                {
                    for (y = 0; y < nGridCnt; y++)
                    {
                        for (x = 0; x < nGridCnt; x++)
                        {
                            bmp.SetPixel(x, nGridCnt - y - 1, Global.GetHeatColor(meanshift.kde[x, y], meanshift.maxKde));
                        }
                    }
                    bmp.Save(heatmapFile);
                }
                

                //#endregion Generate Heatmap Image and save on disk

                #region Calculate clusters and save on disk
                List<FlowCytometry.CustomCluster.Cluster> clusters = meanshift.CalculateCluster();
                // Write WBC 3 Cells into the hard disk
                WBC3Cell wbc3Cell = new WBC3Cell();
                foreach(FlowCytometry.CustomCluster.Cluster cluster in clusters)
                {
                    if (string.IsNullOrEmpty(cluster.clusterName))
                        continue;
                    switch(cluster.clusterName)
                    {
                        case "Neutrophils":
                            wbc3Cell.wbc_n.AddRange(cluster.points);
                            break;
                        case "Monocytes":
                            wbc3Cell.wbc_m.AddRange(cluster.points);
                            break;
                        case "Lymphocytes":
                            wbc3Cell.wbc_l.AddRange(cluster.points);
                            break;
                    }
                }
                Global.WriteToBinaryFile(cellsFile, wbc3Cell);
            }

            #endregion Calculate clusters and save on disk
        }

        #endregion Upload FCS Files


        #region Upload RBC Files
        public async Task<IActionResult> UploadRbc()
        {
            var files = HttpContext.Request.Form.Files;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string orgFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fileName = Path.GetRandomFileName() + "_" + orgFileName;
                    string fullPath = Path.Combine(Constants.rbc_fcs_full_path, fileName);
                    using (var stream = System.IO.File.Create(fullPath))
                    {
                        await file.CopyToAsync(stream);

                    }

                    if (!System.IO.File.Exists(fullPath))
                        continue;

                    try
                    {
                        using (FCMeasurement fcsMeasurement = new FCMeasurement(fullPath))
                        {
                            int nomenclature = fcsMeasurement.GetNomenclature();
                            if (nomenclature < 0)
                            {
                                System.IO.File.Delete(fullPath);
                                continue;
                            }
                            await Task.Run(() => CreateNewRBC(orgFileName, fileName, nomenclature));
                        }
                    }
                    catch
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            return Ok();
        }
        private void CreateNewRBC(string orgFileName, string fileName, int nomenclature)
        {
            FCSModel newWbc = new FCSModel
            {
                fcs_name = orgFileName,
                fcs_file_name = fileName,
                fcs_path = Constants.rbc_fcs_path + fileName,
                user_id = Convert.ToInt32(User.FindFirst(Constants.CLAIM_TYPE_USER_ID).Value),
                fcs_type = Constants.FCS_TYPE_RBC,
                nomenclature = nomenclature
            };
            _dbContext.FCSs.Add(newWbc);
            _dbContext.SaveChangesAsync();
        }
        #endregion Upload RBC Files

        #region Delete FCS Files
        public IActionResult Delete(int id)
        {
            int user_id = Convert.ToInt32(User.FindFirst(Constants.CLAIM_TYPE_USER_ID).Value);

            FCSModel fcs = _dbContext.FCSs.Where(e => e.id == id && e.user_id == user_id).FirstOrDefault();
            if (fcs != null)
            {
                fcs.enabled = false;
                _dbContext.SaveChanges();
            }
            return Ok("sucess");
        }
        #endregion
    }
}