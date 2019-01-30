using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RHOnline.Controllers
{
    public class HoleritesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Holerite()
        {
            return View();
        }

    }
}