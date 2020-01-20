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

            //try
            //{
            cliente.Connect(localEndPoint);
            if (cliente.Connected) Console.WriteLine("Connected ");

            //Thread.Sleep(5000);
            long i = 0;
            while (true)
            {
                try { cliente.Send(msg); } catch (Exception e) { }
                Console.WriteLine("Msg sent " + i);
                //recibirRespuesta(cliente);
                Thread.Sleep(1);
                i++;
            }

            Console.ReadKey(true);
            //cliente.Shutdown(SocketShutdown.Both);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.StackTrace);
            //}

        }

        private static void recibirRespuesta(Socket conexionRemota)
        {
            Console.WriteLine("Waiting for response... ");
            Byte[] recibedBytes = new Byte[1024];
            int n_bytes = 0;
            string respuesta = "";

            n_bytes = conexionRemota.Receive(recibedBytes, recibedBytes.Length, SocketFlags.None);

            respuesta = Encoding.ASCII.GetString(recibedBytes, 0, n_bytes);

            Console.WriteLine(respuesta);

        }
    }
}
