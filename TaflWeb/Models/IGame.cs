using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaflWeb.Models
{
    public interface IGame
    {

        string GetString();

        void SetString(string value);
    }
}
