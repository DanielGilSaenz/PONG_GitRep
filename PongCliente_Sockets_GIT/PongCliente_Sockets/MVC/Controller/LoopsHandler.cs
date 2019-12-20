using PongCliente_Sockets.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PongCliente_Sockets
{
    class LoopsHandler
    {

        private List<ConsoleKey> keysBuffer = new List<ConsoleKey>();
        private FrameRate frameRate;
        private Point middle;

        public LoopsHandler(FrameRate frameRate)
        {
            this.frameRate = frameRate;
        }

        /// <summary>Shows the menu and returns the selected option </summary>
        public int menuLoop(MenuObj mainMenu, ScreenHandler screenHandler)
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
        public void gameLoop(Player player1, Player player2, Ball ball, Wall wallTop, Wall wallBottom, 
            StatusBoard statusBoard, ScreenHandler screenHandler)
        {
            Task task1 = new Task(() => handleInput(ref keysBuffer, ref player1, ref player2, statusBoard));
            Task task2 = new Task(() => handlePhysics(ball, player1, player2, wallTop, wallBottom, statusBoard, screenHandler));

            task1.Start();
            task2.Start();
        }

        /// <summary> Does the math to know where everybody is and then draws them</summary>
        private void handlePhysics(Ball ball, Player player1, Player player2, Wall wallTop, Wall wallBottom, 
            StatusBoard statusBoard, ScreenHandler screenHandler)
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
                updatePlayerPos(ref keysBuffer, ref player1, ref player2);
                updateBall(ref ball, ref frameRate, player1, player2, wallTop, wallBottom, statusBoard);


                // Locks the other theads
                Locks.DRAWING = false;
                stopWatch.Stop();

                // Wait Till the next frame
                delay = frameRate.delayTillNextFrame - stopWatch.Elapsed;
                if (delay.Milliseconds > 0) Thread.Sleep(delay);
                frameRate.actualFrame++;
                
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
        private void handleInput(ref List<ConsoleKey> keyBuffer, ref Player p1, ref Player p2, StatusBoard scoreBoard)
        {
            while (scoreBoard.gameIsOver == false)
            {
                if(Locks.DRAWING == false)
                {
                    while (Console.KeyAvailable)
                    {
                        
                        ConsoleKey key = Console.ReadKey(true).Key;
                        if((key==p1.keyUp)||(key==p1.keyDown)||(key==p2.keyUp)||(key==p2.keyDown))
                        {
                            keyBuffer.Add(key);
                        }
                    }
                }
            }
        }

        /// <summary> Updates the players position according to the keys in the buffer</summary>
        private void updatePlayerPos(ref List<ConsoleKey> keyBuffer, ref Player p1, ref Player p2)
        {
            foreach(ConsoleKey c in keyBuffer)
            {
                p1.userUpdate(c);
                p2.userUpdate(c);
            }
            keyBuffer = new List<ConsoleKey>();
        }

        /// <summary> IDK</summary>
        private void updateBall(ref Ball ball, ref FrameRate FR, Player player1, Player player2, Wall wallTop, Wall WallBottom,
            StatusBoard statusBoard)
        {
            int dirX, dirY;

            int maxX, maxY;
            maxX = (int)ball.vector.x;
            maxY = (int)ball.vector.y;

            int i = 0; int j = 0;

            while (i < maxX || j < maxY)
            {
                HitboxHandler.handleHit(ref ball, ref WallBottom);
                HitboxHandler.handleHit(ref ball, ref wallTop);
                HitboxHandler.handleHit(ref ball, ref player1);
                HitboxHandler.handleHit(ref ball, ref player2);
                HitboxHandler.handleGoal(ref ball, ref player1, ref player2, ref statusBoard);

                if (i < maxX) 
                {

                    // Does a little math trick to get +1 || -1 || 0 from the vector
                    dirX = (int)(ball.vector.x / Math.Abs(ball.vector.x));
                    if (dirX != -1 && dirX != 1) dirX = 0;
                    ball.pos.x += (dirX);

                    i++;
                }

                if (j < maxX)
                { 

                    // Does a little math trick to get +1 || -1 || 0 from the vector
                    dirY = (int)(ball.vector.y / Math.Abs(ball.vector.y));
                    if (dirY != -1 && dirY != 1) dirY = 0;
                    ball.pos.y += (dirY);

                    j++;
                }
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
