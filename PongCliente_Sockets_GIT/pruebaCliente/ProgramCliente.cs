using PongCliente_Sockets.MVC.Model.Math_Objects;
using PongCliente_Sockets.MVC.Model.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pruebaCliente
{
    class ProgramCliente
    {
        static void Main(string[] args)
        {
            Player player = new Player(Key.W, Key.S, 800, 10, new Point(200, 20), 3);

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 80);
            Socket cliente = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Byte[] msg = Encoding.ASCII.GetBytes(player.getAttr(player) + Environment.NewLine);
            cliente.Connect(localEndPoint);

            while (!cliente.Connected) ;

            long i = 0;
            //while (true)
            {
                try { cliente.Send(msg, 1024, SocketFlags.None); } catch (Exception e) { }
                Console.WriteLine("Msg sent " + i);
                Thread.Sleep(500);
                cliente.Close();
                i++;
            }
        }
    }
}
