using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Linq;
using serverExternals;

namespace server
{
    class Server
    {
        private TcpListener listener;
        private Thread listenThread;
        private Thread sendThread;
        private AutoResetEvent newData;
        private Queue<Message> msgQueue;
        private volatile List<TcpClient> tcpClients;
        private List<ServerClient> serverClients;
        private const int port = 3000;

        static void Main(string[] args)
        {
            Server server = new Server();
            Console.WriteLine("server created");
            server.start();
            Console.WriteLine("server started");
            String input = "";
            while (input != "exit")
                input = Console.ReadLine();
            server.stop();
            Thread.Sleep(1000);
            Console.WriteLine("press any key to continue...");
            Console.ReadLine();
        }

        private Server()
        {
            listener = new TcpListener(IPAddress.Any, port);

            msgQueue = new Queue<Message>();

            tcpClients = new List<TcpClient>();
            serverClients = new List<ServerClient>();

            listenThread = new Thread(new ThreadStart(listen));
            sendThread = new Thread(new ThreadStart(send));

            newData = new AutoResetEvent(false);
        }

        private void start()
        {
            listenThread.Start();
            sendThread.Start();
            Console.WriteLine("server running on port " + port);
        }

        private void stop()
        {
            Console.WriteLine("stopping listener");
            listener.Stop();
            Console.WriteLine("stopping sender");
            sendThread.Abort();
            Console.WriteLine("stopping receivers");
            try
            {
                foreach (TcpClient client in tcpClients)
                    client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("caught exception while closing client: ");
                Console.WriteLine(e);
            }
        }

        private void listen()
        {
            listener.Start();
            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();

                    Console.Write("client connected with endpoint: ");
                    Console.WriteLine(client.Client.RemoteEndPoint.ToString());

                    IPEndPoint ep = (IPEndPoint)client.Client.RemoteEndPoint;
                    byte[] addressBytes = ep.Address.GetAddressBytes();

                    tcpClients.Add(client);
                    serverClients.Add(new ServerClient((byte)tcpClients.IndexOf(client), addressBytes));

                    Thread receiveThread = new Thread(new ParameterizedThreadStart(receive));

                    receiveThread.Start(client);
                }
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("listen thread aborting...");
            }
            catch (Exception e)
            {
                Console.WriteLine("exception in listen thread, exception was:");
                Console.WriteLine(e);
            }
            finally
            {
                listener.Stop();
                Console.WriteLine("exiting listen thread");
            }
        }

        private void send()
        {
            try
            {
                while (true)
                {
                    newData.WaitOne();

                    if (msgQueue.Count > 0)
                    {
                        Message msg = msgQueue.Dequeue();
                        NetworkStream stream;
                        if (msg.To == null)
                        {
                            Console.WriteLine("sending message to all clients");
                            foreach (TcpClient client in tcpClients)
                            {
                                stream = client.GetStream();
                                stream.Write(msg.Body, 0, msg.Body.Length);
                            }
                        }
                        else
                        {
                            Console.WriteLine("sending message to client with ip: " + msg.To.Client.RemoteEndPoint);
                            stream = msg.To.GetStream();
                            stream.Write(msg.Body, 0, msg.Body.Length);
                        }
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("send thread aborting...");
            }
            catch (SocketException e)
            {
                Console.WriteLine("exception in send thread, exception was:");
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("exiting send thread");
            }
        }

        private void receive(object tcpClient)
        {
            TcpClient client = (TcpClient)tcpClient;

            NetworkStream clientStream = client.GetStream();

            byte[] received = new byte[4096];
            int bytesRead;

            try
            {
                while (true)
                {
                    bytesRead = clientStream.Read(received, 0, received.Length);

                    if (bytesRead == 0)
                        break;

                    Console.WriteLine("received message from: " + client.Client.RemoteEndPoint.ToString());
                    Console.WriteLine("message length: " + bytesRead.ToString());

                    byte[] message = new byte[bytesRead];

                    Array.Copy(received, message, bytesRead);

                    msgQueue.Enqueue(interpret(message, client));

                    newData.Set();
                }
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("receive thread aborting...");
            }
            catch (IOException e)
            {
                Console.WriteLine("exception in receive thread, exception was:");
                Console.WriteLine(e);
            }
            finally
            {
                tcpClients.Remove(client);
                clientStream.Close();
                client.Close();
                Console.WriteLine("client disconnected, exiting receive thread");
            }
        }

        private Message interpret(byte[] message, TcpClient sender)
        {
            byte command = message [0];
            TcpClient receiver = null;
            byte[] temp;
            switch (command)
            {
                case Commands.BroadcastMessage:
                    temp = new byte[message.Length - 1];
                    Array.Copy(message, 1, temp, 0, message.Length - 1);
                    break;
                case Commands.SpecificMessage:
                    temp = new byte[message.Length - 2];
                    Array.Copy(message, 2, temp, 0, message.Length - 2);
                    byte id = message[1];
                    receiver = tcpClients[id];
                    break;
                case Commands.ListClients:
                    String respString = "";
                    foreach (ServerClient client in serverClients)
                    {
                        foreach (byte bla in client.serialize())
                            respString += (char)bla;
                        respString += "\r\n";
                    }
                    temp = new ASCIIEncoding().GetBytes(respString);
                    receiver = sender;
                    break;
                default:
                    temp = message;
                    break;
            }
            
            return new Message(sender, receiver, temp);
        }

        private TcpClient getClientByIpAddress(byte[] address)
        {
            foreach (TcpClient temp in tcpClients)
            {
                IPEndPoint ep = (IPEndPoint)temp.Client.RemoteEndPoint;

                if (address.SequenceEqual(ep.Address.GetAddressBytes()))
                {
                    return temp;
                }
            }

            return null;
        }
    }
}
