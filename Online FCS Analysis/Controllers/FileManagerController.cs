using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Online_FCS_Analysis.Models.Entities;
using Online_FCS_Analysis.Utilities;

namespace Online_FCS_Analysis.Controllers
{
    public class FileManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadAsync()
        {
            // save files into hard disk
            SaveFiles();
            return Ok();
        }

        // save files into hard disk
        private void SaveFiles()
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
                        file.CopyTo(stream);
                        
                    }
                    CalculateDynamicGate(fileName);
                    CreateNewWBC(orgFileName, fileName);
                }
            }
        }

        private void CreateNewWBC(string orgFileName, string fileName)
        {
            FCSModel newWbc = new FCSModel
            {
                fcs_name = orgFileName,
                fcs_path = Constants.wbc_fcs_path+ fileName,
                user_id = Convert.ToInt32(User.FindFirst(Constants.CLAIM_TYPE_USER_ID).Value),
                fcs_type = Constants.FCS_TYPE_WBC,
                wbc_3cells = Constants.wbc_3cell_path + fileName,
                wbc_heatmap = Constants.wbc_heatmap_path + fileName,
                nomenclature = GetNomenclature(fileName)
            };
        }

        private async void CalculateDynamicGate(string fileName)
        {

        }

        private int GetNomenclature(string fileName)
        {

            return 0;
        }
    }
}