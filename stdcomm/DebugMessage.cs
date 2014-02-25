using System;

namespace stdcomm
{
    public static class DebugMessage
    {
        public static bool UserInput = false;
        public static void show(string message)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[" + DateTime.Now.ToLongTimeString() + "] ");
            Console.WriteLine(message);
            Console.ForegroundColor = prevColor;
        }
    }
}

