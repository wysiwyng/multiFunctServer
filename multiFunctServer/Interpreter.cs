using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using serverExternals;

namespace server
{
    internal static class Interpreter
    {
        internal static Message interpret(byte[] message, Client sender)
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
