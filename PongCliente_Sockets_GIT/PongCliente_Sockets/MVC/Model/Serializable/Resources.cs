using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets
{
    class Resources
    {
        public static int[] rndVectorValues = new int[] { -4, -3, -2, -1, 1, 2, 3, 4 };

        public const char cWall = '░';
        public const char cRect = '█';
        public const char cBlock = '■';
        public const char cSpace= ' ';

        public const int blockSize_X = 3;
        public const int blockSize_Y = 5;

        public static char[,] blockEmpty = new char[,]
        {
            { cSpace, cSpace, cSpace },
            { cSpace, cSpace, cSpace },
            { cSpace, cSpace, cSpace },
            { cSpace, cSpace, cSpace },
            { cSpace, cSpace, cSpace },
        };

        public static char[,] block1 = new char[,]
        {
            { cRect, cRect, cSpace },
            { cSpace, cRect, cSpace },
            { cSpace, cRect, cSpace },
            { cSpace, cRect, cSpace },
            { cRect, cRect, cRect },
        };

        public static char[,] block2 = new char[,]
        {
            { cRect, cRect, cRect },
            { cSpace, cSpace, cRect },
            { cRect, cRect, cRect },
            { cRect, cSpace, cSpace },
            { cRect, cRect, cRect },
        };

        public static char[,] block3 = new char[,]
        {
            { cRect, cRect, cRect },
            { cSpace, cSpace, cRect },
            { cSpace, cRect, cRect },
            { cSpace, cSpace, cRect },
            { cRect, cRect, cRect },
        };

        public static char[,] block4 = new char[,]
        {
            { cRect, cSpace, cRect },
            { cRect, cSpace, cRect },
            { cRect, cRect, cRect },
            { cSpace, cSpace, cRect },
            { cSpace, cSpace, cRect },
        };

        public static char[,] block5 = new char[,]
        {
            { cRect, cRect, cRect },
            { cRect, cSpace, cSpace },
            { cRect, cRect, cRect },
            { cSpace, cSpace, cRect },
            { cRect, cRect, cRect },
        };

        public static char[,] block6 = new char[,]
        {
            { cRect, cRect, cRect },
            { cRect, cSpace, cSpace },
            { cRect, cRect, cRect },
            { cRect, cSpace, cRect },
            { cRect, cRect, cRect },
        };

        public static char[,] block7 = new char[,]
        {
            { cRect, cRect, cRect },
            { cSpace, cSpace, cRect },
            { cSpace, cSpace, cRect },
            { cSpace, cSpace, cRect },
            { cSpace, cSpace, cRect },
        };

        public static char[,] block8 = new char[,]
        {
            { cRect, cRect, cRect },
            { cRect, cSpace, cRect },
            { cRect, cRect, cRect },
            { cRect, cSpace, cRect },
            { cRect, cRect, cRect },
        };

        public static char[,] block9 = new char[,]
        {
            { cRect, cRect, cRect },
            { cRect, cSpace, cRect },
            { cRect, cRect, cRect },
            { cSpace, cSpace, cRect },
            { cSpace, cSpace, cRect },
        };

        public static char[,] block0 = new char[,]
        {
            { cRect, cRect, cRect },
            { cRect, cSpace, cRect },
            { cRect, cSpace, cRect },
            { cRect, cSpace, cRect },
            { cRect, cRect, cRect },
        };

        public static char[,] blockTwoDots = new char[,]
        {
            { cSpace, cSpace, cSpace },
            { cSpace, cRect, cSpace },
            { cSpace, cSpace, cSpace },
            { cSpace, cRect, cSpace },
            { cSpace, cSpace, cSpace },
        };

        public static char[,] getBlock(int x)
        {
            switch(x)
            {
                case -1: return blockEmpty; 
                case 0: return block0; 
                case 1: return block1; 
                case 2: return block2; 
                case 3: return block3; 
                case 4: return block4; 
                case 5: return block5; 
                case 6: return block6; 
                case 7: return block7; 
                case 8: return block8; 
                case 9: return block9;
                default: return null; 
            }

        }
    }
}
