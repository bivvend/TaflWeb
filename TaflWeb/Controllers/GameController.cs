using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        /// <summary>
        /// Return the just created board object 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/GetBoard")]
        public async Task<string> GetBoard()
        {
            game.board.CreateBoard();
            string boardAsJson = await Task<string>.Run(() => game.GetBoardAsJson());
            return boardAsJson;
        }


        [HttpPost]
        [Route("api/[controller]/SquareClick")]
        public async Task<string> SquareClick(int column, int row, string boardDataAsJson)
        {
            try
            {
                game = JsonConvert.DeserializeObject<Game>(boardDataAsJson);         
               
                string responseJSon = await Task<string>.Factory.StartNew(() => game.SquareClickResponse(column, row));
                return responseJSon;
            }
            catch(Exception ex)
            {
                return "FAILED";
            }
        }


    }


}