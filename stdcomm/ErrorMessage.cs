using System;

namespace stdcomm
{
    public static class ErrorMessage
    {
        public static void show(string message)
        {
            Message.setColor(ConsoleColor.DarkRed);
            Message.showMessage("error", message);
            Message.resetColor();
        }
    }
}

