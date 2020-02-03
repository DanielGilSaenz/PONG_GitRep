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
        public static bool handleHit(ref Ball ball, ref Wall wall)
        {
            // Hit top of the wall and the ball is going to hit wall
            if ((ball.pos.y == wall.line.p2.y - 1) && (ball.vector.y > 0))
            {
                bounceY(ref ball);
                return true;
            }

            // Hit bottom of wall and the ball is going to hit wall
            if ((ball.pos.y == wall.line.p2.y + 1) && (ball.vector.y < 0))
            {
                bounceY(ref ball);
                return true;
            }

            return false;

        }

        /// <summary> Checks if the ball hits the player and changes its direction</summary>
        public static bool handleHit(ref Ball ball, ref Player player)
        {
            // Hit left of the player and the ball is going to hit the player
            if ((ball.pos.x == player.pos.x - 1) && (ball.vector.x > 0))
            {
                return handlePlayerHitbox(ref ball, ref player);
            }

            // Hit right of the player and the ball is going to hit the player
            if ((ball.pos.x == player.pos.x + 1) && (ball.vector.x < 0))
            {
                return handlePlayerHitbox(ref ball, ref player);
            }

            return false;
        }

        /// <summary> Checks if the ball has hit the player.pos.y and bounces the ball accordingly</summary>
        private static bool handlePlayerHitbox(ref Ball ball, ref Player player)
        {
            // Checks if it has to bounce on the player
            if (!(ball.pos.y < player.bottom.y || ball.pos.y > player.top.y))
            {
                bounceX(ref ball);
                return true;
            }

            return false;
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
        public static bool handleGoal(ref Ball ball, ref Player player1, ref Player player2, ref Wall topwall, ref Wall bottomwall, ref StatusBoard statusBoard)
        {
            // Passed Player1
            if (ball.pos.x == player2.pos.x - 10)
            {
                statusBoard.p1_Score++;
                ball.pos = fPoint.Cast(ScreenHandler.centerOfScreen);
                ball.vector = fVector.getRandom();
                return true;
            }

            // Passedtop or bottom walls
            if ((ball.pos.y < topwall.line.p1.y - 2) || (ball.pos.y > bottomwall.line.p1.y + 2))
            {
                statusBoard.p1_Score++;
                ball.pos = fPoint.Cast(ScreenHandler.centerOfScreen);
                ball.vector = fVector.getRandom();
                return true;
            }


            // Passed Player2
            if (ball.pos.x == player1.pos.x + 10)
            {
                statusBoard.p2_Score++;
                ball.pos = fPoint.Cast(ScreenHandler.centerOfScreen);
                ball.vector = fVector.getRandom();
                return true;
            }

            return false;
        }

        //........................................................................................................

    }
}
