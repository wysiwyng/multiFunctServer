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
        private Client from;
        public Client From { get { return from; } }

        private Client to;
        public Client To { get { return to; } }

        private byte[] body;
        public byte[] Body { get { return body; } }

        public Message(Client from, Client to, byte[] body)
        {
            this.from = from;
            this.to = to;
            this.body = body;
        }
    }
}
