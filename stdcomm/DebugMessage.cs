using System;

namespace stdcomm
{
    public static class DebugMessage
    {
        public static void show(string message)
        {
            Message.showMessage("debug", message, ConsoleColor.DarkGreen);
        }
    }
}