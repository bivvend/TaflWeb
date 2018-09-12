using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaflWeb.Models;

namespace TaflWeb.Controllers
{
    public class HomeController : Controller
    {

        private IGame game { get; set; }

        public HomeController(IGame gameIn) => game = gameIn;
        public ViewResult Index()
        {
            return View();
        }
    }
}