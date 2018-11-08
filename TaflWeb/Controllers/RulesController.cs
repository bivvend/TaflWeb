using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaflWeb.Models;

namespace TaflWeb.Controllers
{
    public class RulesController : Controller
    {


        public RulesController()
        {

        }

        public IActionResult Rules()
        {
            return View();
        }
    }
}