using PongCliente_Sockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets.MVC.Model.Serializable
{
    class Jugada : Mostrar
    {
        public Jugada()
        {
        }

        public Jugada(Player player, Ball ball, StatusBoard statusBoard)
        {
            this.player = player;
            this.ball = ball;
            this.statusBoard = statusBoard;
        }

        public Player player { get; set; }
        public Ball ball { get; set; }
        public StatusBoard statusBoard { get; set; }
    }
}
