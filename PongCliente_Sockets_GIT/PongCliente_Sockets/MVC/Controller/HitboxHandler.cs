using PongCliente_Sockets.MVC.Model.Math_Objects;
using PongCliente_Sockets.MVC.Model.Serializable;
using PongCliente_Sockets.MVC.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets.MVC.Controller
{
    class HitboxHandler
    {
        // HitBoxes
        //........................................................................................................

        /// <summary> Checks if the ball hits the wall and changes its direction</summary>
        public static void handleHit(ref Ball ball, ref Wall wall)
        {
            // Hit top of the wall
            if (ball.pos.y == wall.line.p2.y - 1) bounceY(ref ball);

            // Hit bottom of wall
            if (ball.pos.y == wall.line.p2.y + 1) bounceY(ref ball);

        }

        /// <summary> Checks if the ball hits the player and changes its direction</summary>
        public static void handleHit(ref Ball ball, ref Player player)
        {
            // Hit left of the player
            if (ball.pos.x == player.pos.x - 1)
            {
                handlePlayerHitbox(ref ball, ref player);
            }

            // Hit right of the player
            if (ball.pos.x == player.pos.x + 1)
            {
                handlePlayerHitbox(ref ball, ref player);
            }
        }

        /// <summary> Checks if the ball has hit the player.pos.y and bounces the ball accordingly</summary>
        private static void handlePlayerHitbox(ref Ball ball, ref Player player)
        {
            // Checks if it has to bounce on the player
            if (!(ball.pos.y < player.bottom.y || ball.pos.y > player.top.y))
            {
                bounceX(ref ball);
            }
        }

        private static void bounceX(ref Ball ball)
        {
            ball.vector.x *= -1;
            ball.Beep();
        }

        private static void bounceY(ref Ball ball)
        {
            ball.vector.y *= -1;
            ball.Beep();
        }

        //........................................................................................................

        // Goals
        //........................................................................................................

        /// <summary> Checks if the ball is in goal position and updates the scoreboard if there is a goal </summary>
        public static void handleGoal(ref Ball ball, ref Player player1, ref Player player2, ref StatusBoard statusBoard)
        {
            // Passed Player1
            if (ball.pos.x == player2.pos.x - 10)
            {
                statusBoard.p1_Score++;
                ball.pos = fPoint.Cast(ScreenHandler.centerOfScreen);
            }


            // Passed Player2
            if (ball.pos.x == player1.pos.x + 10)
            {
                statusBoard.p2_Score++;
                ball.pos = fPoint.Cast(ScreenHandler.centerOfScreen);
            }
        }

        //........................................................................................................

    }
}
