using PongCliente_Sockets.Async;
using PongCliente_Sockets.Menus;
using PongCliente_Sockets.MVC.Controller;
using PongCliente_Sockets.MVC.Model.Math_Objects;
using PongCliente_Sockets.MVC.Model.Serializable;
using PongCliente_Sockets.MVC.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PongCliente_Sockets.MVC.Controller
{
    class LoopsHandler
    {

        private List<ConsoleKey> keysBuffer = new List<ConsoleKey>();

        private MenuObj menu;
        private MenuObj menuConfig;

        private FrameRate frameRate;
        private ScreenHandler screenHandler;

        private Wall topWall;
        private Wall bottomWall;

        private Player player1;
        private Player player2;
        private Ball ball;
        private StatusBoard statusBoard;

        // I have to pass a List<object> by reference with all the objects used in this class and then try cast every one
        public LoopsHandler(List<object> gameObj)
        {
            menu =          (MenuObj)gameObj[0];
            menuConfig =    (MenuObj)gameObj[1];

            frameRate =     (FrameRate)gameObj[2];
            screenHandler = (ScreenHandler)gameObj[3];

            topWall =       (Wall)gameObj[4];
            bottomWall =    (Wall)gameObj[5];

            player1 =       (Player)gameObj[6];
            player2 =       (Player)gameObj[7];

            ball =          (Ball)gameObj[8];
            statusBoard =   (StatusBoard)gameObj[9];

            //gameObj.Add(menu);
            //gameObj.Add(menuConfig);

            //gameObj.Add(frameRate);
            //gameObj.Add(screenHandler);

            //gameObj.Add(topWall);
            //gameObj.Add(bottomWall);

            //gameObj.Add(player1);
            //gameObj.Add(player2);

            //gameObj.Add(ball);
            //gameObj.Add(statusBoard);
        }

        /// <summary>Shows the menu and returns the selected option </summary>
        public int menuLoop(MenuObj mainMenu)
        {
            while(true)
            {
                screenHandler.menu(mainMenu);
                ConsoleKey userKey = Console.ReadKey(true).Key;
                if (userKey == ConsoleKey.UpArrow) mainMenu.optionUp();
                if (userKey == ConsoleKey.DownArrow) mainMenu.optionDown();

                if (userKey == ConsoleKey.Enter)
                {
                    return mainMenu.selectedOption;
                }
            }
        }

        /// <summary> Distributes the work betwen the async tasks </summary>
        public void gameLoop()
        {
            Task task1 = new Task(() => handleInput());
            Task task2 = new Task(() => handlePhysics());

            task1.Start();
            task2.Start();
        }

        /// <summary> Does the math to know where everybody is and then draws them</summary>
        private void handlePhysics()
        {
            // Initializes the locks
            Locks.READING = true;

            // Initializes the copies of the objects, these are used to
            // keep track of the changes and only draw once wvery change
            Ball lastBall = null;
            Player lastPlayer1 = null;
            Player lastPlayer2 = null;
            StatusBoard lastBoard = null;

            // Creates and initializes the objects to keep the framerate 
            // as constant as the machine allows
            Stopwatch stopWatch = new Stopwatch();
            TimeSpan delay;

            stopWatch.Start();
            while (statusBoard.gameIsOver == false)
            {
                // Locks the other theads
                Locks.DRAWING = true;

                // Draws the objects if there has been any change
                if (!ball.Compare(lastBall))           drawBall(ball, screenHandler, false);
                if (!player1.Compare(lastPlayer1))     drawPlayer(player1, screenHandler, false);
                if (!player2.Compare(lastPlayer2))     drawPlayer(player2, screenHandler, false);
                if (!statusBoard.Compare(lastBoard))   drawScoreboard(statusBoard, screenHandler, false);


                // Keeps a register of the objects to erase later
                lastBall = (Ball)ball.Clone();
                lastPlayer1 = (Player)player1.Clone();
                lastPlayer2 = (Player)player2.Clone();
                lastBoard = (StatusBoard)statusBoard.Clone();


                // THEN DO CALCULATIONS
                // Ball = new Ball new Coordinates
                // Player = new Player new Coordinates
                updatePlayerPos();
                updateBall();


                // Locks the other theads
                Locks.DRAWING = false;
                stopWatch.Stop();

                // Wait Till the next frame
                delay = frameRate.delayTillNextFrame - stopWatch.Elapsed;
                if (delay.Milliseconds > 0) Thread.Sleep(delay);
                frameRate.actualFrame++;
                //Thread.Sleep(1000);

                // To reset the actual frame if it is over the max FPS
                if (frameRate.actualFrame >= frameRate.FPS) frameRate.actualFrame = 1;
                stopWatch.Reset();

                // Unlocks the other theads
                stopWatch.Start();
                Locks.DRAWING = true;

                // Erases the previous objects if there are any change
                if (!ball.Compare(lastBall))            drawBall(lastBall, screenHandler, true);
                if (!player1.Compare(lastPlayer1))      drawPlayer(lastPlayer1, screenHandler, true);
                if (!player2.Compare(lastPlayer2))      drawPlayer(lastPlayer2, screenHandler, true);
                if (!statusBoard.Compare(lastBoard))    drawScoreboard(lastBoard, screenHandler, true);
            }
        }

        /// <summary> Reads the keys while the screen is not drawing</summary>
        private void handleInput()
        {
            while (statusBoard.gameIsOver == false)
            {
                if(Locks.DRAWING == false)
                {
                    while (Console.KeyAvailable)
                    {
                        ConsoleKey key = Console.ReadKey(true).Key;
                        if ((key == player1.keyUp) || (key == player1.keyDown) || (key == player2.keyUp) || (key == player2.keyDown))
                        {
                            keysBuffer.Add(key);
                        }
                        if (Locks.DRAWING == true) break;
                    }
                }
            }
        }

        /// <summary> Updates the players position according to the keys in the buffer</summary>
        private void updatePlayerPos()
        {
            foreach(ConsoleKey c in keysBuffer)
            {
                player1.userUpdate(c);
                player2.userUpdate(c);
            }
            keysBuffer = new List<ConsoleKey>();
        }

        /// <summary> IDK</summary>
        private void updateBall()
        {
            Line lineOfBall = new Line(Point.Cast(ball.pos), new Point((int)ball.pos.x + (int)ball.vector.x, (int)ball.pos.y + (int)ball.vector.y));

            // Gets all the points of the line
            var points = lineOfBall.getPoints();

            // Removes the first point, it is the ball current position
            points.RemoveAt(0);

            foreach(Point p in points)
            {
                ball.pos = fPoint.Cast(p);
                if (HitboxHandler.handleHit(ref ball, ref bottomWall)) break;
                if (HitboxHandler.handleHit(ref ball, ref topWall)) break;
                if (HitboxHandler.handleHit(ref ball, ref player1)) break;
                if (HitboxHandler.handleHit(ref ball, ref player2)) break;
                if (HitboxHandler.handleGoal(ref ball, ref player1, ref player2, ref statusBoard)) break;
            }

            //throw new NotImplementedException();
        }

        /// <summary> Draws the scoreBoard in to the screen </summary>
        private void drawScoreboard(StatusBoard statusBoard, ScreenHandler screenHandler, bool erase)
        {
            if(erase)
            {
                drawNumber(-1, statusBoard.pos, -1, screenHandler);
                drawNumber(-1, statusBoard.pos, 1, screenHandler);
            }
            else
            {
                drawNumber(statusBoard.p2_Score, statusBoard.pos, -1, screenHandler);
                drawNumber(statusBoard.p1_Score, statusBoard.pos, 1, screenHandler);
            }
            screenHandler.drawBlock(statusBoard.pos, Resources.blockTwoDots);
            // throw new NotImplementedException();
        }

        private void drawNumber(int number, Point pos, int direccion, ScreenHandler screenHandler)
        {
            int offset = ((1 + 2) * direccion);
            Point unitPos = new Point(pos.x + ((1 + 2) * direccion), pos.y);
            Point tensPos = new Point(unitPos.x + ((1 + 1 + 2) * direccion), pos.y);
            Point hundredsPos = new Point(tensPos.x + ((1 + 1 + 2) * direccion), pos.y);
            int tens, hundreds;

            if (direccion > 0)
            {
                Point temp;
                temp = (Point)unitPos.Clone();
                unitPos = (Point)hundredsPos.Clone();
                hundredsPos = (Point)temp.Clone();
            }

            if (number < 100)
            {
                screenHandler.drawBlock(hundredsPos, Resources.block0);
                if (number < 10)
                {
                    screenHandler.drawBlock(tensPos, Resources.block0);
                    screenHandler.drawBlock(unitPos, Resources.getBlock(number));
                }
                else
                {
                    tens = number / 10;
                    number = number % 10;
                    screenHandler.drawBlock(tensPos, Resources.getBlock(tens));
                    screenHandler.drawBlock(unitPos, Resources.getBlock(number));
                }
            }
            else
            {
                
                hundreds = number / 100;
                tens = (number / 10) - (hundreds * 10);
                number = number % 10;
                screenHandler.drawBlock(hundredsPos, Resources.getBlock(hundreds));
                screenHandler.drawBlock(tensPos, Resources.getBlock(tens));
                screenHandler.drawBlock(unitPos, Resources.getBlock(number));
            }

            //throw new NotImplementedException();
        }

        private void drawBall(Ball ball, ScreenHandler screenHandler, bool erase)
        {
            if(erase) screenHandler.drawPoint(Point.Cast(ball.pos), ConsoleColor.Black, Resources.cSpace);
            else screenHandler.drawPoint(Point.Cast(ball.pos), ConsoleColor.White, Resources.cBlock);
        }

        private void drawPlayer(Player player, ScreenHandler screenHandler, bool erase)
        {
            if(erase) screenHandler.drawLine(player.toLine(), ConsoleColor.White, Resources.cSpace);
            else screenHandler.drawLine(player.toLine(), ConsoleColor.White, Resources.cRect);
        }
    }
}
