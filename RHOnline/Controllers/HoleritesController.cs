using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RHOnline.Library.Filters;

namespace RHOnline.Controllers
{
    [Login]
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