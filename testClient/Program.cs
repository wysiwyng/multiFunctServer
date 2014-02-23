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
            TcpClient client = new TcpClient();
            client.Connect(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 3000);
            Console.WriteLine("connected");
            String[] ip = Console.ReadLine().Split('.');
            Console.WriteLine("sending bla to specified ip");
            byte[] addressBytes = new byte[4];
            for (int i = 0; i < 4; i++)
                addressBytes[i] = Convert.ToByte(ip[i]);
            NetworkStream clientStream = client.GetStream();
            byte[] message = { Commands.ListClients, addressBytes[0], addressBytes[1], addressBytes[2], addressBytes[3], (byte)'b', (byte)'l', (byte)'a' };
            clientStream.Write(message, 0, message.Length);
            byte[] buffer = new byte[4096];

            int bytesRead = clientStream.Read(buffer, 0, buffer.Length);
            Console.WriteLine(new ASCIIEncoding().GetString(buffer, 0, bytesRead));
            clientStream.Close();
            client.Close();
            Console.ReadLine();
        }
    }
}
