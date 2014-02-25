using System;

namespace stdcomm
{
    public static class DebugMessage
    {
        public static bool DebugEnabled { get; set; }
        public static void show(string message)
        {
            if(DebugEnabled)
                Message.showMessage("debug", message, ConsoleColor.DarkGreen);
        }
    }
}