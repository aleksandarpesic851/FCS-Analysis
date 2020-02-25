using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Online_FCS_Analysis.Controllers
{
    public class FCSController : Controller
    {
        public IActionResult Wbc()
        {
            return View();
        }

        public IActionResult Rbc()
        {
            return View();
        }
    }
}