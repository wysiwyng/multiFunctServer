using System.Net.Sockets;
using serverExternals;

namespace server
{
    internal class Client
    {
        internal ServerClient ServerClient { get; private set; }

        internal TcpClient TcpClient{ get; private set; }

        internal byte ClientId { get { return ServerClient.ClientID; } set { ServerClient.ClientID = value; } }

        internal NetworkStream ClientStream { get { return TcpClient.GetStream(); } }

        internal Client(ServerClient serverClient, TcpClient tcpClient)
        {
            ServerClient = serverClient;
            TcpClient = tcpClient;
        }
    }
}

