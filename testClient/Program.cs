using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using serverExternals;

namespace testClient
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {

                TcpClient client = new TcpClient();
                client.Connect(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 3000);
                Console.WriteLine("connected");
                Console.Write("ip > ");
                String[] ip = Console.ReadLine().Split('.');
                byte[] addressBytes = new byte[4];
                for (int i = 0; i < 4; i++)
                    addressBytes[i] = Convert.ToByte(ip[i]);
                NetworkStream clientStream = client.GetStream();
                byte[] message = new byte[4096];
                Console.Write("type: > ");
                message[0] = Convert.ToByte(Console.ReadLine());
                Array.Copy(addressBytes, 0, message, 1, 4);
                Console.Write("message: > ");
                byte[] byteString = new ASCIIEncoding().GetBytes(Console.ReadLine());
                Array.Copy(byteString, 0, message, 5, byteString.Length);
                clientStream.Write(message, 0, byteString.Length + 5);
                byte[] buffer = new byte[4096];

                int bytesRead = clientStream.Read(buffer, 0, buffer.Length);
                Console.WriteLine(new ASCIIEncoding().GetString(buffer, 0, bytesRead));
                clientStream.Close();
                client.Close();
                Console.Write("press enter, exit to quit > ");
                if (Console.ReadLine() == "exit") return;
            }
        }
    }
}
