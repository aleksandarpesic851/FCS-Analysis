using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_FCS_Analysis.Models;
using Online_FCS_Analysis.Models.Entities;
using Online_FCS_Analysis.Utilities;

namespace Online_FCS_Analysis.Controllers
{
    [Authorize]
    public class FCSController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public FCSController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Wbc()
        {
            return View();
        }
        public IActionResult Rbc()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoadWbc()
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
                var wbcData = (from tempWbc in _dbContext.FCSs.Where(fcs => fcs.enabled && fcs.fcs_type == type_wbc && fcs.user_id == userId) select tempWbc);

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

                //Paging   
                var data = wbcData.Skip(skip).Take(pageSize).ToList();

                //total number of rows count   
                recordsTotal = data.Count;

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}