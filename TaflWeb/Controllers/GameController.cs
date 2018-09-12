using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaflWeb.Models;

namespace TaflWeb.Controllers
{
    public class GameController : Controller
    {
        private IGame game { get; set; }

        public GameController(IGame gameIn)
        {
            game = gameIn;
        }

        [HttpGet]
        [Route("api/[controller]/GetString")]
        public string GetString()
        {
            return game.GetString();
        }

        [HttpPost]
        [Route("api/[controller]/SetString")]
        public void SetString(string Text)
        {
            game.SetString(Text);
        }
    }
}