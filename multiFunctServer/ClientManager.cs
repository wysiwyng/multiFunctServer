using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using serverExternals;
using stdcomm;

namespace server
{
    internal static class ClientManager
    {
        internal static List<Client> clients;

        static ClientManager()
        {
            clients = new List<Client>();
        }

        internal static void addClient(Client client)
        {
            clients.Add(client);
        }

        internal static Client addClient(TcpClient tcpClient)
        {
            IPEndPoint ep = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            byte[] addressBytes = ep.Address.GetAddressBytes();
            
            Client client = new Client(new ServerClient(0, addressBytes), tcpClient);
            clients.Add(client);
            byte id = (byte)clients.IndexOf(client);
            clients[id].ClientId = id;
            return clients[id];
        }

        internal static void removeClient(Client client)
        {
            if(!clients.Contains(client))
                return;
            client.TcpClient.Close();
            clients.Remove(client);
        }

        internal static void removeAllClients()
        {
            foreach (Client client in clients)
            {
                try
                {
                    client.TcpClient.Close();
                }
                catch(Exception e)
                {
                    ErrorMessage.show("exception while closing client:\r\n" + e.ToString());
                }
            }
            clients.Clear();
        }

        internal static Client getClientByID(int id)
        {
            return clients[id];
        }

        internal static void sendToAll(byte[] message)
        {
            foreach (Client client in clients)
            {
                client.TcpClient.GetStream().Write(message, 0, message.Length);
            }
        }

        internal static byte[] serializeAll()
        {
            byte[] resp = new byte[clients.Count * 5];
            int respIdx = 0;
            foreach (Client client in clients)
            {
                foreach (byte bla in client.ServerClient.serialize())
                    resp[respIdx++] = bla;
            }
            return resp;
        }
    }
}