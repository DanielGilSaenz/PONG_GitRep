using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets.Async
{
    class FrameRate
    {
        public int FPS { get; set; }
        public int actualFrame { get; set; } = 1;
        public TimeSpan delayTillNextFrame  { get; set; }

        public List<int> delayHistory { get; set; } = new List<int>();

        public FrameRate(int fPS)
        {
            FPS = fPS;
            delayTillNextFrame = getDelay();
        }

        public TimeSpan getDelay()
        {
            return TimeSpan.FromMilliseconds(1000 / FPS);
        }

        public int getAvgFPS()
        {
            int total = 0;
            foreach(int i in delayHistory)
            {
                total += i;
            }
            if (delayHistory.Count != 0)
            {
                return total / delayHistory.Count;
            }
            else return 0;
        }
    }
}
