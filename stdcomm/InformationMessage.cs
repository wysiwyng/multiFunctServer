using System;

namespace stdcomm
{
    public static class InformationMessage
    {
        public static void show(string message)
        {
            Message.showMessage("info", message, ConsoleColor.DarkGray);
        }
    }
}

