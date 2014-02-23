using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace server
{
    class Message
    {
        private TcpClient from;
        public TcpClient From { get { return from; } }

        private TcpClient to;
        public TcpClient To { get { return to; } }

        private byte[] body;
        public byte[] Body { get { return body; } }

        public Message(TcpClient from, TcpClient to, byte[] body)
        {
            this.from = from;
            this.to = to;
            this.body = body;
        }
    }
}
