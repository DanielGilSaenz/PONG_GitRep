using PongCliente_Sockets.Pantalla;
using PongCliente_Sockets.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets
{
    class LoopsHandler
    {
        public int menuLoop(MenuObj mainMenu, ScreenHandler screenHandler)
        {
            while(true)
            {
                screenHandler.menu(mainMenu);
                ConsoleKey userKey = getUserKey();
                if (userKey == ConsoleKey.UpArrow) mainMenu.optionUp();
                if (userKey == ConsoleKey.DownArrow) mainMenu.optionDown();

                if (userKey == ConsoleKey.Enter)
                {
                    return mainMenu.selectedOption;
                }
            }
        }

        public async void jugarLoop(Player player1, Player player2, Ball bola, int [,] screenBuffer, ScreenHandler screenHandler)
        {
            Task task1 = new Task(() => handlePlayer(player1));
            Task task2 = new Task(() => handlePlayer(player2));
            Task task3 = new Task(() => handlePhysics(bola, screenBuffer, player1, player2));
            Task task4 = new Task(() => screenHandler.drawBuffer());
        }

        private void handlePhysics(Ball bola, object screenBuffer, Player player1, Player player2)
        {
            throw new NotImplementedException();
        }

        private void handlePlayer(Player player)
        {
            while(true)
            {
                player.userUpdate(getUserKey());
            }
        }


        public void configLoop(MenuObj menu, ScreenHandler screenHandler)
        {
            while (true)
            {
                screenHandler.menu(menu);
                ConsoleKey userKey = getUserKey();
                if (userKey == ConsoleKey.UpArrow) menu.optionUp();
                if (userKey == ConsoleKey.DownArrow) menu.optionDown();

                if (userKey == ConsoleKey.Enter)
                {
                    switch (menu.selectedOption)
                    {
                        case 0: return;
                        case 1: return;
                        case 2: return;
                        case 3: return;
                    }
                }
            }
        }

        private ConsoleKey getUserKey()
        {
            return Console.ReadKey(false).Key; ;
        }
    }
}
