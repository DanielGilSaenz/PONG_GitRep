using PongCliente_Sockets.MVC.Model.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PruebaServidor
{
    class ProgramServer
    {
        static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 80);
            Socket servidor = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            servidor.Bind(localEndPoint);
            servidor.Listen(10);

            Socket conexionRemota = servidor.Accept();
            if (conexionRemota.Connected) Console.WriteLine("Connected ");
            Task.Run(() => AtenderPeticion(conexionRemota));

            while (true) ;
        }

        private static void AtenderPeticion(Socket conexionRemota)
        {
            Byte[] bytes = new Byte[1024];
            string datos = null;
            int bytesRec = 0;

            long i = 0;
            //while (conexionRemota.Connected)
            {
                bytesRec = conexionRemota.Receive(bytes, 1024, SocketFlags.None);
                datos += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (datos.IndexOf(Environment.NewLine) > 0) datos = datos.Substring(0, datos.IndexOf(Environment.NewLine));
                Player player2 = new Player();
                try
                {
                    player2 = (Player)JsonSerializer.Deserialize(datos, typeof(Player), null);
                    Console.WriteLine(player2.getAttr(player2));

                    Byte[] msg = Encoding.ASCII.GetBytes("Petición Recibida Correctamente");
                    conexionRemota.Send(msg);
                    i++;
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
                if (datos.IndexOf(Environment.NewLine) <= 0) datos = "";
            }
        }
    }
}
