using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using client;

namespace testClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("server ip> ");
            Client client = new Client(System.Net.IPAddress.Parse(Console.ReadLine()), 3000);
            client.openConnection();

            client.MessageReceived += client_MessageReceived;

            client.listClients();

            Console.ReadLine();

            client.sendMessage(new byte[] { (byte)'b', (byte)'l', (byte)'a' });

            Console.Write("press any key to exit...");
            Console.ReadLine();
            client.closeConnection();
        }

        static void client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine(e.Timestamp.ToString());
            foreach(byte temp in e.Message)
                Console.WriteLine(temp.ToString());
        }


    }
}
