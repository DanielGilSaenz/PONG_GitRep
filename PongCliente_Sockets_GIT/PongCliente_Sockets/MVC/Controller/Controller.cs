using PongCliente_Sockets.Async;
using PongCliente_Sockets.Menus;
using PongCliente_Sockets.MVC.Model.Math_Objects;
using PongCliente_Sockets.MVC.Model.Serializable;
using PongCliente_Sockets.MVC.View;
using PongServidor_Sockets.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PongCliente_Sockets.MVC.Controller
{
    class Controller
    {
        public MenuObj menu;
        public MenuObj menuConfig;
        private FrameRate frameRate;
        public ScreenHandler screenHandler;
        public LoopsHandler loopsHandler;

        public Wall topWall;
        public Wall bottomWall;

        public Player player1;
        public Player player2;
        public Ball ball;
        public StatusBoard statusBoard;

        public ServerConfigParams serverConfigParams;

        private RecieverHandler recieverHandler;

        private List<object> gameObj;

        public Controller()
        {
            gameObj = initDefault();
        }

        public void showMenu()
        {
        // GOTO Label
        begining:

            reloadHandler(gameObj);

            int selected = loopsHandler.menuLoop(menu);

            // Option selected is "play"
            if (selected == 0)
            {
                reloadHandler(gameObj);

                if (serverConfigParams.mode == ServerConfigParams.Mode.ONLINE)
                {
                    TcpClient tempClient;
                    if (!connectToServer(out tempClient))
                    {
                        goto begining;
                    }
                    else if(tempClient != null)
                    {
                        serverConfigParams.tcpClient = tempClient;
                        reloadHandler(gameObj);

                        // Clears the menu and draws the top and bottom walls
                        Console.Clear();
                        screenHandler.drawLine(topWall.line, ConsoleColor.White, Resources.cRect);
                        screenHandler.drawLine(bottomWall.line, ConsoleColor.White, Resources.cRect);

                        // Does the loop that handles the game
                        loopsHandler.gameLoop(true);
                    }
                    else
                    {
                        goto begining;
                    }
                }
                else
                {
                    // Clears the menu and draws the top and bottom walls
                    Console.Clear();
                    screenHandler.drawLine(topWall.line, ConsoleColor.White, Resources.cRect);
                    screenHandler.drawLine(bottomWall.line, ConsoleColor.White, Resources.cRect);

                    // Does the loop that handles the game
                    loopsHandler.gameLoop(false);
                }
            }

            else if (selected == 1)
            {
            configMenu:
                selected = loopsHandler.menuLoop(menuConfig);

                switch (selected)
                {
                    case 0: menu_changeIP(); goto configMenu;
                    case 1: menu_changeMode(); goto configMenu;
                    case 2: menu_changeFPS(); goto configMenu;
                    case 3: menu_changePlayerSize(); goto configMenu;
                    case 4: goto begining;
                }
            }
            else if (selected == 2) { statusBoard.gameIsOver = true; return; }
        }

        private bool connectToServer(out TcpClient client)
        {
            MenuObj waitingMenu = new MenuObj(new string[] { "Waiting to connect to the server", "", "~" }, null, false);
            waitingMenu.selectedOption = 5;

            string[] waitingCursor = new string[] { "\\", "|", "/", "~" };

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int i = 0;
            screenHandler.drawMenu(waitingMenu);

            Int32 port = serverConfigParams.PORT;
            try { client = new TcpClient(serverConfigParams.IP, port); }
            catch(ArgumentNullException)
            {
                waitingMenu.Options[0] = "Error on the IP, change it and try again";
                screenHandler.drawMenu(waitingMenu);
                Console.ReadKey(true);
                client = null;
                return false;
            }
            catch(ArgumentOutOfRangeException)
            {
                waitingMenu.Options[0] = "Error on the IP, change it and try again";
                screenHandler.drawMenu(waitingMenu);
                Console.ReadKey(true);
                client = null;
                return false;
            }
            catch (SocketException)
            {
                waitingMenu.Options[0] = "Error on the server, try again later";
                screenHandler.drawMenu(waitingMenu);
                Console.ReadKey(true);
                client = null;
                return false;
            }
            waitingMenu.Options[0] = "Waiting to find a match";
            NetworkStream stream = client.GetStream();
            Byte[] data = new Byte[256];

            recieverHandler = new RecieverHandler(stream, data);
            string msg;

            bool matchFound = false;
            int seconds = 0;
            while (!matchFound)
            {
                if (!recieverHandler.isRunning())
                {
                    waitingMenu.Options[0] = "Error the server has disconnected";
                    screenHandler.drawMenu(waitingMenu);
                    while(Console.ReadKey().Key != ConsoleKey.Enter);
                    matchFound = false;
                    break;
                }

                Int32 n_bytes = 0;
                msg = recieverHandler.getMsg();

                if (msg == "MatchFound")
                {
                    matchFound = true;
                    break;
                }

                // Changes the screen wating icon
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    waitingMenu.Options[2] = waitingCursor[i];
                    screenHandler.drawMenu(waitingMenu);
                    if (i < waitingCursor.Length - 1) i++;
                    else i = 0;

                    stopwatch.Reset();
                    stopwatch.Start();
                    seconds++;
                }

                // Conection timeout
                if (seconds >= serverConfigParams.TIMEOUT)
                {
                    waitingMenu.Options[0] = "Error, too long before response, try again later";
                    screenHandler.drawMenu(waitingMenu);
                    while (Console.ReadKey().Key != ConsoleKey.Enter) ;
                    matchFound = false;
                    break;
                }
            }
            return matchFound;
        }

        /// <summary> Initializes the objects of the playground </summary>
        private List<object> initDefault()
        {
            // We create the menus
            menu = new MenuObj(new string[] { "Jugar", "Configuracion", "Salir" }, null, false);
            menuConfig = new MenuObj(new string[] { "IP del servidor", "Modo Online/Offline", "FPS", "Tamaño players", "Salir" }, null, true);

            // Initialize the graphics and the controller
            frameRate = new FrameRate(16);
            screenHandler = new ScreenHandler();

            // This is the offset on top and bottom of the walls
            int vOffset = 7;

            // Initialize the top wall
            topWall = new Wall(
                new Line(
                    new Point(30, vOffset),
                    new Point(screenHandler.max_W - 30, vOffset)
                    ));

            // Initialize the bottom wall
            bottomWall = new Wall(
                new Line(
                    new Point(30, screenHandler.max_H - vOffset),
                    new Point(screenHandler.max_W - 30, screenHandler.max_H - vOffset)
                    ));

            // Initialize the Player1
            player1 = new Player(
                Key.Up, Key.Down,
                bottomWall.line.p2.y - 1, topWall.line.p2.y + 1,
                new Point(topWall.line.p2.x, screenHandler.max_H / 2), 3
                );

            // Initialize the Player1
            player2 = new Player(
                Key.W, Key.S,
                bottomWall.line.p1.y - 1, topWall.line.p1.y + 1,
                new Point(topWall.line.p1.x, screenHandler.max_H / 2), 3
                );

            // Initialize the Ball
            ball = new Ball(new fPoint(screenHandler.max_W / 2, screenHandler.max_H / 2), new fVector(4, 1));

            // Initialize the scoreBoard
            statusBoard = new StatusBoard(new Point(screenHandler.max_W / 2, 3), 0, 0, 10);

            serverConfigParams = new ServerConfigParams();

            // This list is used to make reference to teh objects
            gameObj = new List<object>();

            gameObj.Add(menu);
            gameObj.Add(menuConfig);

            gameObj.Add(frameRate);
            gameObj.Add(screenHandler);

            gameObj.Add(topWall);
            gameObj.Add(bottomWall);

            gameObj.Add(player1);
            gameObj.Add(player2);

            gameObj.Add(ball);
            gameObj.Add(statusBoard);

            gameObj.Add(serverConfigParams);

            return gameObj;
        }

        /// <summary> Allows the user to change the server IP </summary>
        private void menu_changeIP()
        {
        begining:
            //loopsHandler.changeIP(serverConfigParams);
            string ipString = screenHandler.changeValueOf(serverConfigParams.IP, "Server IP");
            IPAddress iPAddress;
            if (!IPAddress.TryParse(ipString, out iPAddress))
            {
                Console.WriteLine("Error of format in the IP");
                Console.ReadKey(true);
                goto begining;
            }
            serverConfigParams.IP = ipString;
            reloadHandler(gameObj);
        }

        /// <summary>Allows the user to change the mode betwen online and offline</summary>
        private void menu_changeMode()
        {
            serverConfigParams.mode = screenHandler.changeValueOf(serverConfigParams.mode, "Connection config");
            reloadHandler(gameObj);
        }

        /// <summary>Allows the user to change the FPS</summary>
        private void menu_changeFPS()
        {
            int fps = screenHandler.changeValueOf(frameRate.FPS, "FPS");
            frameRate = new FrameRate(fps);
            reloadHandler(gameObj);
        }

        /// <summary>Allows the user to change the size of the players</summary>
        private void menu_changePlayerSize()
        {
            int size = player1.size;
            size = screenHandler.changeValueOf(size, "Player size");
            player1.changeSize(size);
            player2.changeSize(size);
            reloadHandler(gameObj);
        }

        private void reloadHandler(List<object> gameObj)
        {
            // Critical line
            // This list is used to make reference to teh objects
            gameObj = new List<object>();

            gameObj.Add(menu);
            gameObj.Add(menuConfig);

            gameObj.Add(frameRate);
            gameObj.Add(screenHandler);

            gameObj.Add(topWall);
            gameObj.Add(bottomWall);

            gameObj.Add(player1);
            gameObj.Add(player2);

            gameObj.Add(ball);
            gameObj.Add(statusBoard);

            gameObj.Add(serverConfigParams);
            loopsHandler = new LoopsHandler(gameObj);
        }


    }
}
