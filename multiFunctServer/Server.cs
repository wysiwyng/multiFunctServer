using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Linq;
using serverExternals;
using stdcomm;

namespace server
{
    class Server
    {
        private TcpListener listener;
        private Thread listenThread;
        private Thread sendThread;
        private AutoResetEvent newData;
        private Queue<Message> msgQueue;
        private List<TcpClient> tcpClients;
        private List<ServerClient> serverClients;
        private const int port = 3000;

        static void Main(string[] args)
        {
            Server server = new Server();
            DebugMessage.show("server created");
            server.start();
            DebugMessage.show("server started");
			DebugMessage.UserInput = true;
            String input = "";
            while (input != "exit")
                input = Console.ReadLine();
            server.stop();
            Thread.Sleep(1000);
			DebugMessage.UserInput = false;
            DebugMessage.show("press any key to continue...");
            Console.ReadLine();
        }

        private Server()
		{
            listener = new TcpListener(IPAddress.Any, port);

			msgQueue = new Queue<Message>();

            listenThread = new Thread(listen);
            sendThread = new Thread(send);

            newData = new AutoResetEvent(false);
        }

        private void start()
        {
            listenThread.Start();
            sendThread.Start();
            DebugMessage.show("server running on port " + port);
        }

        private void stop()
        {
            DebugMessage.show("stopping listener");
            listener.Stop();
            DebugMessage.show("stopping sender");
            sendThread.Abort();
            DebugMessage.show("stopping receivers");
			ClientManager.removeAllClients();
        }

        private void listen()
        {
            listener.Start();
            try
            {
                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();

					Client client = ClientManager.addClient(tcpClient);

                    DebugMessage.show("client connected with endpoint: " + client.TcpClient.Client.RemoteEndPoint.ToString());

                    Thread receiveThread = new Thread(receive);

                    receiveThread.Start(client);
                }
            }
            catch (ThreadAbortException e)
            {
                DebugMessage.show("listen thread aborting...");
            }
            catch (SocketException e)
            {
                DebugMessage.show("SocketException in listen thread, maybe the socket shut down, exception was:");
                DebugMessage.show(e.ToString());
            }
            finally
            {
                listener.Stop();
                DebugMessage.show("exiting listen thread");
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
                            DebugMessage.show("sending message to all clients");
							ClientManager.sendToAll(msg.Body);
                        }
                        else
                        {
                            DebugMessage.show("sending message to client with ip: " + msg.To.TcpClient.Client.RemoteEndPoint);
                            stream = msg.To.TcpClient.GetStream();
                            stream.Write(msg.Body, 0, msg.Body.Length);
                        }
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                DebugMessage.show("send thread aborting...");
            }
            catch (SocketException e)
            {
                DebugMessage.show("exception in send thread, exception was:");
                DebugMessage.show(e.ToString());
            }
            finally
            {
                DebugMessage.show("exiting send thread");
            }
        }

        private void receive(object threadArgs)
		{
			Client client = (Client)threadArgs;

			TcpClient tcpClient = client.TcpClient;

            NetworkStream clientStream = tcpClient.GetStream();

            byte[] received = new byte[4096];
            int bytesRead;

            try
            {
                while (true)
                {
                    bytesRead = clientStream.Read(received, 0, received.Length);

                    if (bytesRead == 0)
                        break;

                    DebugMessage.show("received message from: " + tcpClient.Client.RemoteEndPoint.ToString());
                    DebugMessage.show("message length: " + bytesRead.ToString());

                    byte[] message = new byte[bytesRead];

                    Array.Copy(received, message, bytesRead);

                    msgQueue.Enqueue(interpret(message, client));

                    newData.Set();
                }
            }
            catch (ThreadAbortException e)
            {
                DebugMessage.show("receive thread aborting...");
            }
            catch (IOException e)
            {
                DebugMessage.show("IOException in receive thread, maybe the socket shut down, exception was:");
                DebugMessage.show(e.ToString());
            }
			catch(ObjectDisposedException e)
			{
				DebugMessage.show("ObjectDisposedException in receive thread, maybe the socket shut down, exception was:");
				DebugMessage.show(e.ToString());
			}
            finally
            {
				ClientManager.removeClient(client);
                DebugMessage.show("client disconnected, exiting receive thread");
            }
        }

        private Message interpret(byte[] message, Client sender)
        {
            byte command = message[0];
            Client receiver = null;
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
                    receiver = ClientManager.getClientByID(id);
                    break;
                case Commands.ListClients:
					temp = ClientManager.serializeAll();
                    receiver = sender;
                    break;
                default:
                    temp = message;
                    break;
            }
            
            return new Message(sender, receiver, temp);
        }
    }
}