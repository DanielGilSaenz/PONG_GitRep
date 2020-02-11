using PongCliente_Sockets.Async;
using PongCliente_Sockets.Menus;
using PongCliente_Sockets.MVC.Controller;
using PongCliente_Sockets.MVC.Model.Math_Objects;
using PongCliente_Sockets.MVC.Model.Serializable;
using PongCliente_Sockets.MVC.View;
using PongServidor_Sockets.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace PongCliente_Sockets.MVC.Controller
{
    class LoopsHandler
    {
        private bool wasF3Pressed;
        private bool debugOn = false;

        private List<Key> keysBuffer = new List<Key>();

        private List<Jugada> jugadasPendientes = new List<Jugada>();

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

        private ServerConfigParams serverConfigParams;

        private RecieverHandler recieverHandler;

        // I have to pass a List<object> by reference with all the objects used in this class and then try cast every one
        public LoopsHandler(List<object> gameObj)
        {
            menu = (MenuObj)gameObj[0];
            menuConfig = (MenuObj)gameObj[1];

            frameRate = (FrameRate)gameObj[2];
            screenHandler = (ScreenHandler)gameObj[3];

            topWall = (Wall)gameObj[4];
            bottomWall = (Wall)gameObj[5];

            player1 = (Player)gameObj[6];
            player2 = (Player)gameObj[7];

            ball = (Ball)gameObj[8];
            statusBoard = (StatusBoard)gameObj[9];

            serverConfigParams = (ServerConfigParams)gameObj[10];
        }

        public LoopsHandler()
        {
        }

        /// <summary>Shows the menu and returns the selected option </summary>
        public int menuLoop(MenuObj mainMenu)
        {
            while (true)
            {
                screenHandler.drawMenu(mainMenu);
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
        public void gameLoop(bool online)
        {
            if (online) { new Task(() => handleOnline()).Start(); }
            new Task(() => handleFrameByFrame(online)).Start();
        }

        /// <summary> Does the math to know where everybody is and then draws them</summary>
        private void handleFrameByFrame(bool online)
        {
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
                if (!ball.Compare(lastBall)) drawBall(ref ball, ref screenHandler, false);
                if (!player1.Compare(lastPlayer1)) drawPlayer(ref player1, ref screenHandler, false);
                if (!player2.Compare(lastPlayer2)) drawPlayer(ref player2, ref screenHandler, false);
                if (!statusBoard.Compare(lastBoard)) drawScoreboard(ref statusBoard, ref screenHandler, false);

                // debug purposes
                if (debugOn)
                {
                    if (!player1.Compare(lastPlayer1)) screenHandler.drawDebug(player1, 0, 0);
                    if (!player2.Compare(lastPlayer2)) screenHandler.drawDebug(player2, 0, 1);
                    if (!ball.Compare(lastBall)) screenHandler.drawDebug(ball, 0, 2);

                    screenHandler.drawDebug(player1, 0, 0);
                    screenHandler.drawDebug(player1, 0, 0);

                    screenHandler.drawDebug(player2, 0, 1);
                    screenHandler.drawDebug(player2, 0, 1);

                    screenHandler.drawDebug(ball, 0, 2);
                    screenHandler.drawDebug(ball, 0, 2);

                }

                // Keeps a register of the objects to erase later
                lastBall = (Ball)ball.Clone();
                lastPlayer1 = (Player)player1.Clone();
                lastPlayer2 = (Player)player2.Clone();
                lastBoard = (StatusBoard)statusBoard.Clone();


                // THEN DOES CALCULATIONS
                // Updates the ball to the new coordinates
                // Updates the players to the new coordinates
                handleInput(online);
                updateBall();


                // Unlocks the other theads
                Locks.DRAWING = false;
                stopWatch.Stop();

                // Wait Till the next frame
                delay = frameRate.delayTillNextFrame - stopWatch.Elapsed;
                if (delay.Milliseconds > 0) Thread.Sleep(delay);
                frameRate.actualFrame++;
                //Thread.Sleep(100);

                // To reset the actual frame if it is over the max FPS
                if (frameRate.actualFrame >= frameRate.FPS) frameRate.actualFrame = 1;
                stopWatch.Reset();

                // locks the other theads
                Locks.DRAWING = true;

                // Waits for the other theads to stop doing things
                while (Locks.READING) ;
                while (Locks.NETWORKING) ;
                stopWatch.Start();


                // Erases the previous objects if there are any change
                if (!ball.Compare(lastBall)) drawBall(ref lastBall, ref screenHandler, true);
                if (!player1.Compare(lastPlayer1)) drawPlayer(ref lastPlayer1, ref screenHandler, true);
                if (!player2.Compare(lastPlayer2)) drawPlayer(ref lastPlayer2, ref screenHandler, true);
                if (!statusBoard.Compare(lastBoard)) drawScoreboard(ref lastBoard, ref screenHandler, true);

                if (online) { if ((!ball.Compare(lastBall)) || (!player1.Compare(lastPlayer1))) sendJugada(); }
            }
        }


        /// <summary> Reads the keys while the screen is not drawing</summary>
        private void handleInput(bool online)
        {
            if (InputHandler.isKeyDown(Key.Escape)) {statusBoard.gameIsOver = true; return; }
            
            if(!online)
            {
                //handel player2
                if (InputHandler.isKeyDown(player2.keyUp)) player2.updatePos(player2.keyUp);
                else if (InputHandler.isKeyDown(player2.keyDown)) player2.updatePos(player2.keyDown);
                else { player2.resetMomentum(); }
            }
            else
            {
                updateJugadas();
            }

            //handel player1
            if (InputHandler.isKeyDown(player1.keyUp)) player1.updatePos(player1.keyUp);
            else if (InputHandler.isKeyDown(player1.keyDown)) player1.updatePos(player1.keyDown);
            else { player1.resetMomentum(); }
            

            //handel debug
            if (InputHandler.isKeyDown(Key.F3))
            {
                if (!wasF3Pressed)
                {
                    debugOn = !debugOn;
                    wasF3Pressed = true;
                }

                if (!debugOn)
                {
                    screenHandler.clearLines_V(0, 4);
                    drawScoreboard(ref statusBoard, ref screenHandler, false);
                }
            }
            else
            {
                wasF3Pressed = false;
            }

            //throw new NotImplementedException();
        }

        /// <summary>Acumulates a list of movements in a list</summary>
        private void handleOnline()
        {
            Byte[] bytes = new Byte[512];
            int count = 0;
            Jugada j = null;

            //serverConfigParams.tcpClient;
            NetworkStream stream = serverConfigParams.tcpClient.GetStream();
            recieverHandler = new RecieverHandler(stream, bytes, statusBoard);

            while (!statusBoard.gameIsOver)
            {

                if (!Locks.DRAWING)
                {
                    Locks.NETWORKING = true;
                    string str1 = recieverHandler.getMsg();

                    if (str1 != null)
                    {
                        try { j = (Jugada)JsonSerializer.Deserialize(str1, typeof(Jugada)); } catch { }
                        if(j != null)jugadasPendientes.Add(j);
                    }

                    bytes = new Byte[512];
                    Locks.NETWORKING = false;
                }
            }
        }

        /// <summary>Sends the current status of the objects to the server</summary>
        private void sendJugada()
        {
            NetworkStream stream = serverConfigParams.tcpClient.GetStream();

            Byte[] bytes = new Byte[512];
            int count = 0;
            Jugada j = null;

            new Task(() =>
            {
                string msg = new Jugada().getAttr(new Jugada(player1, ball));
                bytes = Encoding.ASCII.GetBytes(msg);
                stream.Write(bytes, 0, bytes.Length);
                bytes = new Byte[512];
            }).Start();
        }

        /// <summary>Handles player1 pos and updates ball position according to the server</summary>
        private void updateJugadas()
        {
            if(jugadasPendientes.Count > 0)
            {
                foreach(Jugada j in jugadasPendientes)
                {
                    player2.pos = (Point)j.player.pos.Clone();
                    ball = (Ball)j.ball.Clone();
                }
                jugadasPendientes.RemoveRange(0, jugadasPendientes.Count);
            }
        }

        /// <summary> Handles the ball hitbox and updates its position accordingly </summary>
        private void updateBall()
        {
            Line lineOfBall = new Line(Point.Cast(ball.pos), new Point((int)ball.pos.x + (int)ball.vector.x, (int)ball.pos.y + (int)ball.vector.y));

            // Gets all the points of the line
            var points = lineOfBall.getPoints();

            // Removes the first point, it is the ball current position
            points.RemoveAt(0);

            foreach (Point p in points)
            {
                //screenHandler.drawDebug(objs);
                ball.pos = fPoint.Cast(p);
                if (HitboxHandler.handleHit(ref ball, ref bottomWall)) break;
                if (HitboxHandler.handleHit(ref ball, ref topWall)) break;
                if (HitboxHandler.handleHit(ref ball, ref player1)) break;
                if (HitboxHandler.handleHit(ref ball, ref player2)) break;
                if (HitboxHandler.handleGoal(ref ball, ref player1, ref player2, ref topWall, ref bottomWall, ref statusBoard)) break;
            }
        }

        /// <summary> Draws the scoreBoard in to the screen </summary>
        private void drawScoreboard(ref StatusBoard statusBoard, ref ScreenHandler screenHandler, bool erase)
        {
            if (erase)
            {
                drawNumber(-1, statusBoard.pos, -1, ref screenHandler);
                drawNumber(-1, statusBoard.pos, 1, ref screenHandler);
            }
            else
            {
                drawNumber(statusBoard.p2_Score, statusBoard.pos, -1, ref screenHandler);
                drawNumber(statusBoard.p1_Score, statusBoard.pos, 1, ref screenHandler);
            }
            screenHandler.drawBlock(statusBoard.pos, Resources.blockTwoDots);
            // throw new NotImplementedException();
        }

        private void drawNumber(int number, Point pos, int direccion, ref ScreenHandler screenHandler)
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

        private void drawBall(ref Ball ball, ref ScreenHandler screenHandler, bool erase)
        {
            if (erase) screenHandler.drawPoint(Point.Cast(ball.pos), ConsoleColor.Black, Resources.cSpace);
            else screenHandler.drawPoint(Point.Cast(ball.pos), ConsoleColor.White, Resources.cBlock);
        }

        private void drawPlayer(ref Player player, ref ScreenHandler screenHandler, bool erase)
        {
            if (erase) screenHandler.drawLine(player.toLine(), ConsoleColor.White, Resources.cSpace);
            else screenHandler.drawLine(player.toLine(), ConsoleColor.White, Resources.cRect);
        }
    }
}
