using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using serverExternals;

namespace client
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private Thread receiveThread;
        private bool open;
        private IPAddress address;

        public IPAddress Address
        {
            get { return address; }
            set
            {
                if (tcpClient.Connected)
                    closeConnection();
                address = value;
            }
        }

        private int port;

        public int Port
        {
            get { return port; }
            set
            {
                if (tcpClient.Connected)
                    closeConnection();
                port = value;
            }
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public Client(IPAddress serverAddress, int serverPort)
        {
            open = false;
            tcpClient = new TcpClient();
            address = serverAddress;
            port = serverPort;
            receiveThread = new Thread(new ThreadStart(receive));
        }

        public void openConnection()
        {
            tcpClient.Connect(address, port);
            clientStream = tcpClient.GetStream();
            receiveThread.Start();
            open = true;
        }

        public void closeConnection()
        {
            open = false;
            clientStream.Close();
            if (tcpClient.Connected)
                tcpClient.Close();
        }

        private void receive()
        {
            byte[] received = new byte[4096];
            int bytesRead = 0;

            try
            {
                while (true)
                {
                    bytesRead = clientStream.Read(received, 0, received.Length);

                    if (bytesRead == 0)
                        break;

                    MessageReceivedEventArgs args = new MessageReceivedEventArgs();

                    Array.Copy(received, args.Message, bytesRead);
                    args.Timestamp = DateTime.Now;

                    onMessageReceived(args);
                }
            } catch (IOException e)
            {
                Console.WriteLine("IOException in receive thread, maybe the connection was lost/closed");
            } finally
            {
                open = false;
            }
        }

        protected virtual void onMessageReceived(MessageReceivedEventArgs e)
        {
            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
            if (handler != null)
                handler(this, e);
        }

        private void send(byte[] message)
        {
            if (!open)
                throw new ClientNotConnectedException("Client is not connected!");
            clientStream.Write(message, 0, message.Length);
        }

        public void sendMessage(byte[] message)
        {
            byte[] temp = new byte[message.Length + 1];
            temp [0] = Commands.BroadcastMessage;
            message.CopyTo(temp, 1);
            send(temp);
        }

        public void sendMessage(byte[] message, IPAddress receiver)
        {
            byte[] temp = new byte[message.Length + 5];
            temp [0] = Commands.SpecificMessage;
            receiver.GetAddressBytes().CopyTo(temp, 1);
            message.CopyTo(temp, 5);
            send(temp);
        }
    }
}

