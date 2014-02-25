using System;

namespace stdcomm
{
    public static class InformationMessage
    {
        public static void show(string message)
        {
            Message.setColor(ConsoleColor.DarkYellow);
            Message.showMessage("info", message);
            Message.resetColor();
        }
    }
}

