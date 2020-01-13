using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PruebaServidor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Indicamos nuestra ip local
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            // Establecemos el EndPoint con la dirección local y el puerto 80:
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 80);
            // Create a TCP/IP socket.
            Socket servidor = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // Asocia el endPoint indicado (ip y puerto) al socket:
                servidor.Bind(localEndPoint);
                // Establece la longitud máxima de la cola de conexiones
                // pendientes que puede tener el servidor antes de empezar a rechazar conexiones:
                servidor.Listen(10);
                Console.WriteLine("Waiting for a connection..." + Environment.NewLine);
                // Comienza a escuchar para recibir conexiones entrantes:
                while (true)
                {
                    // El programa se suspende mientras está esperando la entrada de una conexión:
                    Socket conexionRemota = servidor.Accept();
                    if(conexionRemota.Connected) Console.WriteLine("Connected ");
                    //AtenderPeticion(conexionRemota);
                    Task.Run(() => AtenderPeticion(conexionRemota));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void AtenderPeticion(Socket conexionRemota)
        {
            // Data buffer para almacenar los datos recibidos
            Byte[] bytes = new Byte[1024];
            string datos = null;
            int bytesRec = 0;

            // Recepción de los datos:
            bytesRec = conexionRemota.Receive(bytes);
            datos += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            // Comprobar condición de parada de procesado de la información:

            // Mostramos el texto recibido:
            Console.WriteLine("Text received: " + Environment.NewLine + datos);
            // Mensaje que enviaremos de respuesta al cliente:
            Byte[] msg = Encoding.ASCII.GetBytes("Petición Recibida Correctamente");
            // Envío de mensaje de respuesta al cliente:
            conexionRemota.Send(msg);
            // Apagado del socket:
            conexionRemota.Shutdown(SocketShutdown.Both);
            // Cierre y libera los recursos del socket:
            conexionRemota.Close();
        }
    }
}
