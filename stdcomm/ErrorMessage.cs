using System;

namespace stdcomm
{
    public static class ErrorMessage
    {
        public static void show(string message)
        {
            Message.showMessage("error", message, ConsoleColor.DarkRed);
        }
    }
}

