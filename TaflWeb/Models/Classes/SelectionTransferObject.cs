using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaflWeb.Models.Classes
{
    /// <summary>
    /// Simple object to allow exchange of Selection data from API as JSon string
    /// </summary>
    public class SelectionTransferObject
    {
        private bool _selected;
        public bool selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
            }
        }

        private bool _highlighted;
        public bool highlighted
        {
            get
            {
                return _highlighted;
            }
            set
            {
                _highlighted = value;
            }
        }
    }
}
