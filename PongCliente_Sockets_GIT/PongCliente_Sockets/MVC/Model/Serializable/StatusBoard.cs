using PongCliente_Sockets.Interfaces;
using PongCliente_Sockets.MVC.Model.Math_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets.MVC.Model.Serializable
{
    [Serializable]
    class StatusBoard : ICloneable, ICompareBool
    {
        public Point pos { get; set; }

        public int p1_Score { get; set; }
        public int p2_Score { get; set; }

        public int win_Score { get; set; }

        public bool gameIsOver { get; set; } = false;

        public StatusBoard(Point pos, int p1_Score, int p2_Score, int win_Score)
        {
            this.pos = pos;
            this.p1_Score = p1_Score;
            this.p2_Score = p2_Score;
            this.win_Score = win_Score;
        }


        public bool chekGameStatus()
        {
            if((p1_Score >= win_Score) || (p1_Score >= win_Score))
            {
                gameIsOver = true;
            }
            return gameIsOver;
        }

        public object Clone()
        {
            return new StatusBoard((Point)pos.Clone(), p1_Score, p2_Score, win_Score);
        }

        public bool Compare(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(StatusBoard)) return false;
            StatusBoard o = (StatusBoard)obj;

            if (o.pos.Compare(pos) && o.p1_Score == p1_Score && o.p2_Score == p2_Score
                && o.win_Score == win_Score && o.gameIsOver == gameIsOver) return true;
            else return false;
        }
    }
}
