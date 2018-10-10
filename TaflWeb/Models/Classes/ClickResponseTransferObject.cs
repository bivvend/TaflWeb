using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaflWeb.Models.Classes
{
    /// <summary>
    /// Useed to send the response to a click action back to the client
    /// </summary>
    public class ClickResponseTransferObject
    {
        private string _responseText;
        public string responseText
        {
            get
            {
                return _responseText;
            }
            set
            {
                _responseText = value;
            }
        }

        public static string NOT_A_VALID_SQUARE = "NOT_A_VALID_SQUARE";
        public static string NO_PIECE_FOUND = "NO_PIECE_FOUND";
        public static string PIECE_FOUND_SELECTING = "PIECE_FOUND_SELECTING";
        public static string VALID_MOVE_FOUND_EXECUTING = "VALID_MOVE_FOUND_EXECUTING";

        private bool _requestReDraw;
        public bool requestReDraw
        {
            get
            {
                return _requestReDraw;
            }
            set
            {
                _requestReDraw = value;
            }
        }

        private string _boardAsJson;
        public string boardAsJson
        {
            get
            {
                return _boardAsJson;
            }
            set
            {
                _boardAsJson = value;
            }
        }
    }
}
