using System;

namespace stdcomm
{
    public static class Message
    {
        private static ConsoleColor prevColor;
        public static void setColor(ConsoleColor color)
        {
            prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public static void resetColor()
        {
            Console.ForegroundColor = prevColor;
        }

        public static void showMessage(string type, string message)
        {
            Console.Write(type + ": [" + DateTime.Now.ToString("HH:mm:ss.ffff") + "] ");
            Console.WriteLine(message);
        }
    }
}

