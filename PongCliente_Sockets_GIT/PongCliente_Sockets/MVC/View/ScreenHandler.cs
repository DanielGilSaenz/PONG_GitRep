using System;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PongCliente_Sockets
{
    class ScreenHandler
    {
        private int max_WindowWidth;
        public int max_W;

        private int max_WindowHeight;
        public int max_H;

        public int[,] screenBuffer { get; set; }

        public static Point centerOfScreen;

        public ScreenHandler()
        {
            Maximize();

            // Get the size of the screen after its been maximized
            max_WindowWidth = Console.WindowWidth;
            max_WindowHeight = Console.WindowHeight;

            // Initialize the Console
            Console.SetBufferSize(max_WindowWidth, max_WindowHeight);
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);

            // Initialize local variables for the arrays
            max_W = max_WindowWidth - 1;
            max_H = max_WindowHeight;

            // Initialize the array with the max dimensions
            screenBuffer = new int[max_W, max_H];

            centerOfScreen = new Point(max_W / 2, max_H / 2);
        }

        /// <summary> Draws the menu </summary>
        public void menu(MenuObj menu)
        {
            // Used to know the center of the screen
            int middle_W = max_W / 2;
            int middle_H = max_H / 2;

            // Used to get the selected word from the menu
            UserCursor cursor = new UserCursor(menu.Options[menu.selectedOption]);

            // This here is to center the words on the screen
            int n_filas = menu.Options.Length;

            // This ones measure the offset from the middle of the screen
            int leftOffset;
            int verticalOffset = n_filas / 2;

            // Variables used for a complex loop of iteration
            int j = 0;
            int i = 0;

            string temp;

            if(n_filas % 2 == 0)
            {
                Console.Clear();
                // Writes the top strings on the list and the middle one
                for (i = verticalOffset; i > 0; i--, j++)
                {
                    if (j == menu.selectedOption) temp = cursor.getSelected();
                    else temp = menu.Options[j];

                    leftOffset = temp.Length / 2;
                    Console.SetCursorPosition(middle_W - leftOffset, middle_H - i);
                    Console.Write(temp);
                }

                // Writes the rest of the strings
                for (i = 0; i < verticalOffset; i++, j++)
                {
                    if (j == menu.selectedOption) temp = cursor.getSelected();
                    else temp = menu.Options[j];

                    leftOffset = temp.Length / 2;
                    Console.SetCursorPosition(middle_W - leftOffset, middle_H + i);
                    Console.Write(temp);
                }
            }
            else
            {
                Console.Clear();
                // Writes the top strings on the list and the middle one
                for (i = verticalOffset; i >= 0; i--, j++)
                {
                    if (j == menu.selectedOption) temp = cursor.getSelected();
                    else temp = menu.Options[j];

                    leftOffset = temp.Length / 2;
                    Console.SetCursorPosition(middle_W - leftOffset, middle_H - i);
                    Console.Write(temp);
                }

                // Writes the rest of the strings
                for (i = 1; i <= verticalOffset; i++, j++)
                {
                    if (j == menu.selectedOption) temp = cursor.getSelected();
                    else temp = menu.Options[j];

                    leftOffset = temp.Length / 2;
                    Console.SetCursorPosition(middle_W - leftOffset, middle_H + i);
                    Console.Write(temp);
                }
            }
            
        }

        /// <summary>Draws a line into the screen, 
        /// allows to configure color of the line and the brush tip used
        /// this is the most efficient for drawing lines</summary>
        public void drawLine(Line line, ConsoleColor color, char pixel)
        {
            int min, max;
            int diffX, diffY;
            int x1, y1;
            int x2, y2;


            if (line.p1.y > line.p2.y)  { y1 = line.p2.y; y2 = line.p1.y; }
            else                        { y1 = line.p1.y; y2 = line.p2.y; }

            if (line.p1.x > line.p2.x)  { x1 = line.p2.x; x2 = line.p1.x; }
            else                        { x1 = line.p1.x; x2 = line.p2.x; }

            diffX = x2 - x1;
            diffY = y2 - y1;

            double m = line.Slope();
            if(double.IsInfinity(m))
            {
                min = y1; max = y2;
                for (int i = min; i < max; i++)
                {
                    Console.SetCursorPosition(x1, i);
                    Console.ForegroundColor = color;
                    Console.Write(pixel);
                }
            }
            else
            {
                if(diffX > diffY)
                {
                    min = x1; max = x2;
                    for (int height, i = min; i < max; i++)
                    {
                        height = line.getFromEquation_Y(i, m);
                        Console.SetCursorPosition(i, height);
                        Console.ForegroundColor = color;
                        Console.Write(pixel);
                    }
                }
                else
                {
                    min = y1; max = y2;
                    for (int left, i = min; i < max; i++)
                    {
                        left = (int)line.getFromEquation_X(i, m);
                        Console.SetCursorPosition(left, i);
                        Console.ForegroundColor = color;
                        Console.Write(pixel);
                    }
                }
            }
        }

        /// <summary> Draws a point into the screen</summary>
        public void drawPoint(Point p, ConsoleColor color, char pixel)
        {
            Console.SetCursorPosition(p.x, p.y);
            Console.ForegroundColor = color;
            Console.Write(pixel);
        }

        /// <summary> Draws a char array into a position in the screen, 
        /// pos is the center of the block </summary>
        public void drawBlock(Point pos, char[,] block)
        {
            // Sets the cursor on the top left corner
            int left = pos.x - block.GetLength(0) / 2;
            int top = pos.y - block.GetLength(0) / 2;
            Console.SetCursorPosition(left, top);

            for (int i = 0; i < block.GetLength(0); i++)
            {
                Console.SetCursorPosition(left, top + i);
                for (int j = 0; j < block.GetLength(1); j++)
                {
                    Console.Write(block[i, j]);
                }
            }
            //throw new NotImplementedException();
        }

        /// <summary> Draws the local char buffer into the screen, 
        /// the least efficient of all, only draws in black & white</summary>
        public void drawBuffer()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            string concatLine = "";

            for (int i = 0; i < max_H; i++)
            {
                for (int j = 0; j < max_W; j++)
                {
                    //if(CharBuffer[j, i] != 0)
                    {
                        
                        concatLine += (char)screenBuffer[j, i];
                        //Console.SetCursorPosition(j, i);
                        //Console.Write((char)CharBuffer[j, i]);
                    }
                }

                Console.SetCursorPosition(0, i);
                Console.Write(concatLine);
                concatLine = "";

                /*
                concatLine += (char)CharBuffer[j, i];
                Console.SetCursorPosition(0, i);
                Console.Write(concatLine);
                concatLine = "";
                */
            }
        }

        /// <summary> Maximizes the console </summary>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);
        private static void Maximize()
        {
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3
        }


    }
}
