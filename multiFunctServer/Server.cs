using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace server
{
    class Server
    {
        private TcpListener listener;
        
        private Thread listenThread;
        private Thread sendThread;
        private List<Thread> receiveThreads;

        private AutoResetEvent newData;

        private Queue<Message> msgQueue;

        private volatile List<TcpClient> clients;

        private int port = 3000;

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
            Console.ReadLine();
        }

        private Server()
        {
            listener = new TcpListener(IPAddress.Any, port);

            msgQueue = new Queue<Message>();

            clients = new List<TcpClient>();
            receiveThreads = new List<Thread>();

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
            //listenThread.Abort();
            listener.Stop();
            Console.WriteLine("stopping sender");
            sendThread.Abort();
            Console.WriteLine("stopping receivers");
            try
            {
                foreach (TcpClient client in clients)
                    client.Close();
            }
            catch (Exception e)
            {

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

                    Console.WriteLine("client connected");
                    Console.WriteLine(client.Client.RemoteEndPoint);

                    clients.Add(client);

                    Thread receiveThread = new Thread(new ParameterizedThreadStart(receive));

                    receiveThreads.Add(receiveThread);
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
                            foreach (TcpClient client in clients)
                            {
                                stream = client.GetStream();
                                stream.Write(msg.Body, 0, msg.Body.Length);
                            }
                        }
                        else
                        {
                            stream = msg.To.GetStream();
                            stream.Write(msg.Body, 0, msg.Body.Length);
                        }
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("send thread aborting, closing all connections...");
            }
            catch (Exception e)
            {
                Console.WriteLine("error in send thread, error was:");
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

            byte[] message = new byte[4096];
            int bytesRead;
            
            try
            {
                while (true)
                {
                    bytesRead = clientStream.Read(message, 0, message.Length);

                    if (bytesRead == 0) break;

                    Console.WriteLine("received client message");
                    Console.WriteLine("message length: " + bytesRead.ToString());

                    TcpClient receiver = null;

                    if (message[0] == 255)
                    {
                        byte[] byteAddress = new byte[4];
                        for (int i = 0; i < 4; i++)
                            byteAddress[i] = message[i + 1];
                        IPAddress ipAdress = new IPAddress(byteAddress);
                        foreach (TcpClient temp in clients)
                        {
                            IPEndPoint ep = (IPEndPoint)temp.Client.RemoteEndPoint;
                            if (ep.Address == ipAdress)
                            {
                                receiver = temp;
                                break;
                            }
                        }
                    }

                    Message msg = new Message(client, receiver, message);

                    msgQueue.Enqueue(msg);

                    newData.Set();
                }
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("receive thread caught abort, exiting...");
            }
            catch (Exception e)
            {
                Console.WriteLine("exception in receive thread, error was:");
                Console.WriteLine(e);
            }
            finally
            {
                clients.Remove(client);
                clientStream.Close();
                client.Close();
                Console.WriteLine("client disconnected");
            }
        }
    }
}
