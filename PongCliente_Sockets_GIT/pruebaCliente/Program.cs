using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pruebaCliente
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 80);

            Socket cliente = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Byte[] msg = Encoding.ASCII.GetBytes("Me llamo cliente y este es un mensaje de prueba: 001");

            try
            {
                cliente.Connect(localEndPoint);
                if (cliente.Connected) Console.WriteLine("Connected ");

                //Thread.Sleep(5000);

                cliente.Send(msg);
                Console.WriteLine("Msg sent ");

                
                recibirRespuesta(cliente);

                Console.ReadKey(true);
                //cliente.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

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
