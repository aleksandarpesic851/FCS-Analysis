using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FlowCytometry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Online_FCS_Analysis.Models;
using Online_FCS_Analysis.Models.Entities;
using Online_FCS_Analysis.Utilities;

namespace Online_FCS_Analysis.Controllers
{
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

        public async Task<IActionResult> UploadAsync()
        {
            var files = HttpContext.Request.Form.Files;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string orgFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fileName = Path.GetRandomFileName() + "_" + orgFileName;
                    string fullPath = Constants.wbc_fcs_full_path + fileName;
                    using (var stream = System.IO.File.Create(fullPath))
                    {
                        await file.CopyToAsync(stream);

                    }

                    if (!System.IO.File.Exists(fullPath))
                        continue;

                    FCMeasurement fcsMeasurement;
                    try
                    {
                        fcsMeasurement = new FCMeasurement(fullPath);
                    }
                    catch
                    {
                        System.IO.File.Delete(fullPath);
                        continue;
                    }

                    int nomenclature = fcsMeasurement.GetNomenclature();
                    if (nomenclature < 0)
                    {
                        System.IO.File.Delete(fullPath);
                        continue;
                    }
                    await Task.Run(() => StoreDynamicGateResultsOnDisk(fcsMeasurement, fileName, nomenclature));
                    await Task.Run(() => CreateNewWBC(orgFileName, fileName, nomenclature));
                }
            }
            return Ok();
        }

        private void CreateNewWBC(string orgFileName, string fileName, int nomenclature)
        {
            FCSModel newWbc = new FCSModel
            {
                fcs_name = orgFileName,
                fcs_path = Constants.wbc_fcs_path+ fileName,
                user_id = Convert.ToInt32(User.FindFirst(Constants.CLAIM_TYPE_USER_ID).Value),
                fcs_type = Constants.FCS_TYPE_WBC,
                wbc_3cells = Constants.wbc_3cell_path + fileName + ".obj",
                wbc_heatmap = Constants.wbc_heatmap_path + fileName + ".png",
                nomenclature = nomenclature
            };
            _dbContext.FCSs.Add(newWbc);
        }

        private void StoreDynamicGateResultsOnDisk(FCMeasurement fcsMeasurement, string fileName, int nomenclature)
        {
            #region initialize Objects
            string gateFile = Constants.wwwroot_abs_path + _appSettings.Value.defaultGateSetting.path + fileName;
            string cellsFile = Constants.wbc_3cell_full_path + fileName + ".obj";
            string heatmapFile = Constants.wbc_heatmap_full_path + fileName + ".png";
            string channelNomenclature = Constants.WBC_NOMENCLATURES[nomenclature];

            string channel1 = FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);
            string channel2 = FCMeasurement.GetChannelName("SSCpeak", channelNomenclature);
            List<double[]> arrData = FCMeasurement.GetChannelData(fcsMeasurement, channel1, channel2);
            List<Polygon> polygons = FCMeasurement.loadPolygon(gateFile);
            
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

            FlowCytometry.CustomCluster.Custom_Meanshift meanshift = new FlowCytometry.CustomCluster.Custom_Meanshift(arrData);
            #endregion

            #region Generate Heatmap Image and save on disk
            meanshift.CalculateKDE();
            int nGridCnt = meanshift.nGridCnt;
            Bitmap bmp = new Bitmap(nGridCnt, nGridCnt);
            for (y = 0; y < nGridCnt; y ++)
            {
                for (x = 0; x < nGridCnt; x ++)
                {
                    bmp.SetPixel(x, y, Global.GetHeatColor(meanshift.kde[x, y], meanshift.maxKde));
                }
            }
            bmp.Save(heatmapFile);

            #endregion

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
            #endregion
        }

        
    }
}