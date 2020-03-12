using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlowCytometry;
using FlowCytometry.Mie_Scatter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Online_FCS_Analysis.Models;
using Online_FCS_Analysis.Models.Entities;
using Online_FCS_Analysis.Models.ViewModel;
using Online_FCS_Analysis.Utilities;

namespace Online_FCS_Analysis.Controllers
{
    [Authorize]
    public class FCSController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOptions<AppSettings> _appSettings;
        public FCSController(ApplicationDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings;
        }

        public IActionResult Wbc()
        {
            return View();
        }

        public IActionResult WbcFullItem()
        {
            return View();
        }

        public IActionResult WbcCompare()
        {
            return View();
        }

        public IActionResult WbcReport()
        {
            return View();
        }

        public IActionResult Rbc()
        {
            return View();
        }

        #region WBC

        [HttpPost]
        public IActionResult LoadWBCTable()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                int userId = Convert.ToInt32(User.FindFirst(Constants.CLAIM_TYPE_USER_ID).Value);
                string type_wbc = Constants.FCS_TYPE_WBC;
                // Getting all Customer data  
                var wbcData = (from tempWbc in _dbContext.FCSs.Where(fcs => fcs.enabled && fcs.fcs_type.Contains(type_wbc) && fcs.user_id == userId) select tempWbc);

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    if (sortColumnDirection == "asc")
                        wbcData = wbcData.OrderBy(wbc => typeof(FCSModel).GetProperty(sortColumn).GetValue(wbc));
                    else
                        wbcData = wbcData.OrderByDescending(wbc => typeof(FCSModel).GetProperty(sortColumn).GetValue(wbc));
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    wbcData = wbcData.Where(m => m.fcs_name.Contains(searchValue));
                }

                //total number of rows count   
                recordsTotal = wbcData.Count();

                //Paging   
                var data = wbcData.Skip(skip).Take(pageSize).ToList();
                
                //Returning Json Data  
                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });

            }
            catch (Exception)
            {
                throw;
            }
        }
    
        [HttpPost]
        public IActionResult LoadWbcData(int wbcId)
        {
            int userId = Convert.ToInt32(User.FindFirst(Constants.CLAIM_TYPE_USER_ID).Value);
            FCSModel fcsData = _dbContext.FCSs.FirstOrDefault(e => e.id == wbcId && e.user_id == userId);
            if (fcsData == null)
                return Ok();

            bool isEOS = fcsData.fcs_type == Constants.FCS_TYPE_WBC_EOS;
            bool isAML = fcsData.fcs_type == Constants.FCS_TYPE_WBC_AML;

            string cellsFile = Constants.wwwroot_abs_path + fcsData.wbc_3cells;
            string gate2File = Constants.wwwroot_abs_path + fcsData.wbc_gate2;
            string heatmapFile = fcsData.wbc_heatmap;
            string fcsFile = Constants.wwwroot_abs_path + fcsData.fcs_path;
            string gate1File = Path.Combine(Constants.wwwroot_abs_path, _appSettings.Value.defaultGateSetting.path, _appSettings.Value.defaultGateSetting.gate1);
            string gate3File = Path.Combine(Constants.wwwroot_abs_path, _appSettings.Value.defaultGateSetting.path, _appSettings.Value.defaultGateSetting.gate3);
            string gateEOSFile = Path.Combine(Constants.wwwroot_abs_path, _appSettings.Value.defaultGateSetting.path, _appSettings.Value.defaultGateSetting.gateEos);
            string customGatePath = Path.Combine(Constants.wbc_custom_full_gate, fcsData.fcs_file_name);
            
            if (isAML)
            {
                gate1File = Path.Combine(Constants.wwwroot_abs_path, _appSettings.Value.defaultGateSetting.path, _appSettings.Value.defaultGateSetting.aml_gate1);
                gate3File = Path.Combine(Constants.wwwroot_abs_path, _appSettings.Value.defaultGateSetting.path, _appSettings.Value.defaultGateSetting.aml_gate3);
            }

            FCMeasurement fcsMeasurement = new FCMeasurement(fcsFile);
            WBC3Cell wbc3Cell = Global.ReadFromBinaryFile<WBC3Cell>(cellsFile);
            List<Polygon> gate1Polygon = FCMeasurement.loadPolygon(gate1File);
            List<Polygon> gate2Polygon = new List<Polygon>();
            if (!isAML)
                gate2Polygon = Global.ReadFromBinaryFile<List<Polygon>>(gate2File);

            List<Polygon> gate3Polygon = FCMeasurement.loadPolygon(gate3File);
            List<Polygon> gateEOSPolygon = FCMeasurement.loadPolygon(gateEOSFile);
            List<GatePolygon> customGate = Global.ReadFromBinaryFile<List<GatePolygon>>(customGatePath);
            return Json( 
                new { 
                    wbcData = fcsMeasurement.Channels,
                    wbc3Cell  = new {
                        wbc3Cell.wbc_n,
                        wbc3Cell.wbc_m,
                        wbc3Cell.wbc_l,
                    },
                    customGate,
                    heatmapFile,
                    nomenclature = Constants.WBC_NOMENCLATURES[fcsData.nomenclature],
                    isEOS,
                    isAML,
                    gate1Polygon = gate1Polygon.Select(e => e.poly),
                    gate2Polygon = gate2Polygon.Select(e => e.poly),
                    gate3Polygon = gate3Polygon.Select(e => e.poly),
                    gateEOSPolygon = gateEOSPolygon.Select(e => e.poly),
                });
        }

        [HttpPost]
        public IActionResult SaveCustomGate(int wbcId, List<GatePolygon> customGate)
        {
            int userId = Convert.ToInt32(User.FindFirst(Constants.CLAIM_TYPE_USER_ID).Value);
            FCSModel fcsData = _dbContext.FCSs.FirstOrDefault(e => e.id == wbcId && e.user_id == userId);
            if (fcsData == null)
                return Ok();
            string customGatePath = Path.Combine(Constants.wbc_custom_full_gate, fcsData.fcs_file_name);
            if (customGate.Count > 0)
            {
                Global.WriteToBinaryFile(customGatePath, customGate);
            } 
            else
            {
                Global.DeleteFile(customGatePath);
            }
            return Ok();
        }

        #endregion WBC

        #region RBC
        [HttpPost]
        public IActionResult LoadRBCTable()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                int userId = Convert.ToInt32(User.FindFirst(Constants.CLAIM_TYPE_USER_ID).Value);
                string type_rbc = Constants.FCS_TYPE_RBC;

                // Getting all Customer data  
                var rbcData = (from tempRbc in _dbContext.FCSs.Where(fcs => fcs.enabled && fcs.fcs_type == type_rbc && fcs.user_id == userId) select tempRbc);

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    if (sortColumnDirection == "asc")
                        rbcData = rbcData.OrderBy(wbc => typeof(FCSModel).GetProperty(sortColumn).GetValue(wbc));
                    else
                        rbcData = rbcData.OrderByDescending(wbc => typeof(FCSModel).GetProperty(sortColumn).GetValue(wbc));
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    rbcData = rbcData.Where(m => m.fcs_name.Contains(searchValue));
                }

                //total number of rows count   
                recordsTotal = rbcData.Count();

                //Paging   
                var data = rbcData.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data  
                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult LoadRbcData(int rbcId)
        {
            int userId = Convert.ToInt32(User.FindFirst(Constants.CLAIM_TYPE_USER_ID).Value);
            FCSModel fcsData = _dbContext.FCSs.FirstOrDefault(e => e.id == rbcId && e.user_id == userId);
            if (fcsData == null)
                return Ok();

            string fcsFile = Constants.wwwroot_abs_path + fcsData.fcs_path;
            string gateFile = Path.Combine(Constants.wwwroot_abs_path, _appSettings.Value.defaultGateSetting.path, _appSettings.Value.defaultGateSetting.gateRBC);
            FCMeasurement fcsMeasurement = new FCMeasurement(fcsFile);
            List<Polygon> gatePolygon = FCMeasurement.loadPolygon(gateFile);
            return Json(
                new
                {
                    rbcData = fcsMeasurement.Channels,
                    gatePolygon = gatePolygon.Select(e => e.poly),
                    nomenclature = Constants.WBC_NOMENCLATURES[fcsData.nomenclature]
                });
        }

        [HttpPost]
        public IActionResult CalculateVHC([FromBody]List<SPoint> rbcData)
        {
            //List<SPoint> rbcData = formData.data;
            List<int> arrV = new List<int>();
            List<int> arrHC = new List<int>();
            double s1, s2;
            foreach (SPoint pt in rbcData)
            {
                s1 = pt.x * Constants.Kx;
                s2 = pt.y * Constants.Ky;

                Record record = VHC.arrVHC.Aggregate((e1, e2) =>
                    Math.Abs(e1.S1 - s1) + Math.Abs(e1.S2 - s2) <
                    Math.Abs(e2.S1 - s1) + Math.Abs(e2.S2 - s2) ?
                    e1 : e2);

                arrV.Add(record.V);
                arrHC.Add(record.HC);
            }
            return Json(new
            {
                arrV,
                arrHC
            });
        }
        #endregion RBC
    }
}