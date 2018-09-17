using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Simple representation of a square 
/// </summary>
namespace TaflWeb.Model.Classes
{
    public class SimpleSquare
    {

        private Square.occupation_type occupation;
        public Square.occupation_type Occupation
        {
            get
            {
                return occupation;
            }
            set
            {
                occupation = value;
                if (value == Square.occupation_type.Attacker)
                {
                    KingPresent = false;
                    AttackerPresent = true;
                    DefenderPresent = false;
                }
                if (value == Square.occupation_type.Defender)
                {
                    KingPresent = false;
                    AttackerPresent = false;
                    DefenderPresent = true;
                }
                if (value == Square.occupation_type.King)
                {
                    KingPresent = true;
                    AttackerPresent = false;
                    DefenderPresent = false;
                }
                if (value == Square.occupation_type.Empty)
                {
                    KingPresent = false;
                    AttackerPresent = false;
                    DefenderPresent = false;
                }                

            }
        }

        private Square.square_type squareType;
        public Square.square_type SquareType
        {
            get
            {
                return squareType;
            }
            set
            {
                squareType = value;                
            }
        }

       

        private bool attackerPresent;
        public bool AttackerPresent
        {
            get
            {
                return attackerPresent;
            }
            set
            {
                attackerPresent = value;                
            }
        }

        private bool defenderPresent;
        public bool DefenderPresent
        {
            get
            {
                return defenderPresent;
            }
            set
            {
                defenderPresent = value;                
            }
        }
        private bool kingPresent;
        public bool KingPresent
        {
            get
            {
                return kingPresent;
            }
            set
            {
                kingPresent = value;                
            }
        }


        private int row;
        public int Row
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
                Coords[0] = value;
            }
        }

        private int column;


        public int Column
        {
            get
            {
                return column;
            }
            set
            {
                column = value;
                Coords[0] = value;
            }
        }

        private int[] coords;
        public int[] Coords
        {
            get
            {
                return coords;
            }
            set
            {
                coords = value;
            }
        }

        

        public SimpleSquare()
        {
            this.Coords = new int[] { 0, 0 };
        }

        public SimpleSquare(int _column, int _row, Square.occupation_type _occupancy_type, Square.square_type _square_type)
        {
            this.Coords = new int[] { _column, _row };
            this.Row = _row;
            this.Column = _column;
            this.Occupation = _occupancy_type;
            this.SquareType = _square_type;
        }
    }
}
