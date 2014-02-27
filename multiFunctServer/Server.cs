using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
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
        private const int port = 3000;

        static void Main(string[] args)
        {
            Server server = new Server();
            InformationMessage.show("server created");
            server.start();
            InformationMessage.show("server started");
            String input = "";
            while (input != "exit")
                input = Console.ReadLine();
            server.stop();
            Thread.Sleep(1000);
            InformationMessage.show("press any key to continue...");
            Console.ReadLine();
        }

        private Server()
		{
            stdcomm.Message.prevColor = Console.ForegroundColor;
            DebugMessage.DebugEnabled = true;
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
            InformationMessage.show("server running on port " + port);
        }

        private void stop()
        {
            InformationMessage.show("stopping listener");
            listener.Stop();
            InformationMessage.show("stopping sender");
            sendThread.Abort();
            InformationMessage.show("stopping receivers");
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

                    InformationMessage.show("client connected with endpoint: " + client.TcpClient.Client.RemoteEndPoint.ToString());

                    Thread receiveThread = new Thread(receive);

                    receiveThread.Start(client);
                }
            }
            catch (ThreadAbortException e)
            {
                ErrorMessage.show("listen thread aborting, exception was:\r\n" + e.ToString());
            }
            catch (SocketException e)
            {
                ErrorMessage.show("SocketException in listen thread, maybe the socket shut down, exception was:\r\n" + e.ToString());
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
                        if (msg.To == null)
                        {
                            DebugMessage.show("sending message to all clients");
							ClientManager.sendToAll(msg.Body);
                        }
                        else
                        {
                            DebugMessage.show("sending message to client with ip: " + msg.To.TcpClient.Client.RemoteEndPoint);
                            msg.To.ClientStream.Write(msg.Body, 0, msg.Body.Length);
                        }
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                ErrorMessage.show("send thread aborting, exception was:\r\n" + e.ToString());
            }
            catch (SocketException e)
            {
                ErrorMessage.show("SocketException in send thread, exception was:\r\n" + e.ToString());
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

            string endpointAddress = tcpClient.Client.RemoteEndPoint.ToString();

            byte[] received = new byte[4096];
            int bytesRead;

            try
            {
                while (true)
                {
                    bytesRead = clientStream.Read(received, 0, received.Length);

                    if (bytesRead == 0)
                        break;

                    InformationMessage.show("received message from: " + endpointAddress);
                    DebugMessage.show("message length: " + bytesRead.ToString());

                    byte[] message = new byte[bytesRead];

                    Array.Copy(received, message, bytesRead);

                    msgQueue.Enqueue(Interpreter.interpret(message, client));

                    newData.Set();
                }
            }
            catch (ThreadAbortException e)
            {
                ErrorMessage.show("receive thread aborting, exception was:\r\n" + e.ToString());
            }
            catch (IOException e)
            {
                ErrorMessage.show("IOException in receive thread, maybe the socket shut down, exception was:\r\n" + e.ToString());
            }
			catch(ObjectDisposedException e)
			{
                ErrorMessage.show("ObjectDisposedException in receive thread, maybe the socket shut down, exception was:\r\n" + e.ToString());
			}
            finally
            {
				ClientManager.removeClient(client);
                InformationMessage.show("client " + endpointAddress + " disconnected, closing thread");
            }
        }
    }
}