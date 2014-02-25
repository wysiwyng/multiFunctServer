using System;

namespace stdcomm
{
    public static class DebugMessage
    {
        public static void show(string message)
        {
            Message.setColor(ConsoleColor.White);
            Message.showMessage("debug", message);
            Message.resetColor();
        }
    }
}