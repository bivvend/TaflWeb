﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaflWeb.Model.Classes;
using TaflWeb.Models;

namespace TaflWeb.Controllers
{
    /// <summary>
    /// API access controller - Access to data of current (and historic boards)
    /// Also used to run and return server side AI calculations.
    /// </summary>
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

        [HttpGet]
        [Route("api/[controller]/GetBoard")]
        public async Task<string> GetBoard()
        {
            string boardAsJson =  await game.GetBoardAsJson();
            return boardAsJson;
        }

        [HttpPost]
        [Route("api/[controller]/SetString")]
        public void SetString(string Text)
        {
            game.SetString(Text);
        }
    }
}