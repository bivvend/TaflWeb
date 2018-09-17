using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaflWeb.Model.Classes
{
    public class Piece
    {
        //Location
        public int Row { get; set; }
        public int Column { get; set; }
        //Type
        public enum PieceType
        {
            King, Attacker, Defender
        };

        public PieceType Type { get; set; }

        public Piece()
        {
            this.Row = 0;
            this.Column = 0;
            this.Type = PieceType.Attacker;
        }

        public Piece(int iColumn, int iRow, PieceType aType)
        {
            this.Row = iRow;
            this.Column = iColumn;
            this.Type = aType;
        }

        public override string ToString()
        {
            return Type.ToString() + "," + this.Column.ToString() + "," + this.Row.ToString();
        }

    }
}
