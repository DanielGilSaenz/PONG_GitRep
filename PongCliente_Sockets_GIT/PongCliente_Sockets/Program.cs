using System;
using System.Windows.Input;
using PongCliente_Sockets.MVC.Controller;
using PongCliente_Sockets.MVC.Model.Serializable;
using System.Diagnostics;

namespace PongCliente_Sockets
{
    
    class Program
    {
        public static StatusBoard statusBoard;

        [STAThread]
        static void Main(string[] args)
        {
            // Initialize the controller and gets the statusBoard to know if the game is over
            Controller controller = new Controller();
            statusBoard = controller.statusBoard;

            // Shows the main menu and gets the option selected (must be changed)
            controller.showMenu();

            InputHandler inputHandler = new InputHandler(controller.player1, controller.player2);

            while (statusBoard.gameIsOver == false)
            {
                inputHandler.handleKeyboard();
            }
            
        }
    }
}
