using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaflWeb.Models;

namespace TaflWeb.Controllers
{
    public class GameBoardController : Controller
    {

        private IGame game { get; set; }
        public GameBoardController(IGame gameIn) => game = gameIn;
        public IActionResult CurrentBoard()
        {
            return View();
        }
    }
}