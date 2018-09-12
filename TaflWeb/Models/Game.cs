using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaflWeb.Models
{
    public class Game : IGame
    {
        private string test { get; set; }

        public Game()
        {
            test = "Hello from API.";
        }
        public void SetString(string value)
        {
            test = value;
        }

        public string GetString()
        {
            return test;
        }
    }
}
