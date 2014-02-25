using System;
using System.Net.Sockets;
using serverExternals;

namespace server
{
    public class ReceiveThreadArgs
    {
        public TcpClient tcpClient{ get; set; }
        public ServerClient serverClient{ get; set; }
        public ReceiveThreadArgs(TcpClient tcp, ServerClient server)
        {
            tcpClient = tcp;
            serverClient = server;
        }
    }
}

