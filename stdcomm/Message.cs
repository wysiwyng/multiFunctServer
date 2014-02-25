using System;

namespace stdcomm
{
    public static class Message
    {
        public static ConsoleColor prevColor;

        private static object messageLock = new object();

        internal static void showMessage(string type, string message, ConsoleColor color)
        {
            lock (messageLock)
            {
                Console.ForegroundColor = color;
                Console.Write(type + ": [" + DateTime.Now.ToString("HH:mm:ss.ffff") + "] ");
                Console.ForegroundColor = prevColor;
                Console.WriteLine(message);
            }
        }
    }
}

