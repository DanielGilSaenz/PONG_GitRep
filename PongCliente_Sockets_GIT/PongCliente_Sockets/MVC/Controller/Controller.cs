using PongCliente_Sockets.Async;
using PongCliente_Sockets.Menus;
using PongCliente_Sockets.MVC.Model.Math_Objects;
using PongCliente_Sockets.MVC.Model.Serializable;
using PongCliente_Sockets.MVC.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PongCliente_Sockets.MVC.Controller
{
    class Controller
    {
        public MenuObj menu;
        public MenuObj menuConfig;

        public ScreenHandler screenHandler;
        public LoopsHandler loopsHandler;

        public Wall topWall;
        public Wall bottomWall;

        public Player player1;
        public Player player2;
        public Ball ball;
        public StatusBoard statusBoard;

        private List<object> gameObj;

        public Controller()
        {
            gameObj = initDefault();
        }

        public void showMenu()
        {
            // GOTO Label
            begining:

                loopsHandler = new LoopsHandler(gameObj);

                int selected = loopsHandler.menuLoop(menu);

                // Option selected is "play"
                if (selected == 0)
                {
                    // Clears the menu and draws the top and bottom walls
                    Console.Clear();
                    screenHandler.drawLine(topWall.line, ConsoleColor.White, Resources.cRect);
                    screenHandler.drawLine(bottomWall.line, ConsoleColor.White, Resources.cRect);

                    // Does the loop that handles the game
                    loopsHandler.gameLoop();
                }

                if (selected == 1) goto begining;
                if (selected == 3) return;
        }

        /// <summary> Initializes the objects of the playground </summary>
        private List<object> initDefault()
        {
            // This list is used to make reference to teh objects
            List<object> gameObj = new List<object>();

            // We create the menus
            menu = new MenuObj(new string[] { "Jugar", "Configuracion", "Salir" }, null, false);
            menuConfig = new MenuObj(new string[] { "Nombre", "tipo bola", "Velocidad", "Tamaño players", "Salir" }, null, true);

            // Initialize the graphics and the controller
            FrameRate frameRate = new FrameRate(6);
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



            return gameObj;
        }

    }
}
