using System;
using System.Net.Sockets;
using serverExternals;

namespace server
{
    public class Client
    {
        public ServerClient ServerClient { get; private set; }

        public TcpClient TcpClient{ get; private set; }

        public Client(ServerClient serverClient, TcpClient tcpClient)
        {
            ServerClient = serverClient;
            TcpClient = tcpClient;
        }

        public void setClientId(byte id)
        {
            ServerClient.ClientID = id;
        }
    }
}

