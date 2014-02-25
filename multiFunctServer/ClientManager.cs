using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using serverExternals;
using stdcomm;

namespace server
{
    public static class ClientManager
    {
        public static List<Client> clients;

        static ClientManager()
        {
            clients = new List<Client>();
        }

        public static void addClient(Client client)
        {
            clients.Add(client);
        }

        public static Client addClient(TcpClient tcpClient)
        {
            IPEndPoint ep = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            byte[] addressBytes = ep.Address.GetAddressBytes();
            
            Client client = new Client(new ServerClient(0, addressBytes), tcpClient);
            clients.Add(client);
            byte id = (byte)clients.IndexOf(client);
            clients[id].setClientId(id);
            return clients[id];
        }

        public static void removeClient(Client client)
        {
            if(!clients.Contains(client))
                return;
            client.TcpClient.GetStream().Close();
            client.TcpClient.Close();
            clients.Remove(client);
        }

        public static void removeAllClients()
        {
            foreach (Client client in clients)
            {
                try
                {
                    client.TcpClient.Close();
                }
                catch(Exception e)
                {

                }
            }
            clients.Clear();
        }

        public static Client getClientByID(int id)
        {
            return clients[id];
        }

        public static void sendToAll(byte[] message)
        {
            foreach (Client client in clients)
            {
                client.TcpClient.GetStream().Write(message, 0, message.Length);
            }
        }

        public static byte[] serializeAll()
        {
            String respString = "";
            foreach (Client client in clients)
            {
                foreach (byte bla in client.ServerClient.serialize())
                    respString += (char)bla;
                respString += "\r\n";
            }
            return new ASCIIEncoding().GetBytes(respString);
        }
    }
}