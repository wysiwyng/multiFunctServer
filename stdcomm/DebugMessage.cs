using System;

namespace stdcomm
{
    public static class DebugMessage
    {
        public static void show(string message)
        {
            Message.setColor(ConsoleColor.DarkGreen);
            Message.showMessage("debug", message);
            Message.resetColor();
        }
    }
}