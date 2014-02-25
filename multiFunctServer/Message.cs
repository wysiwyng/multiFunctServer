namespace server
{
    internal class Message
    {
        internal Client From { get; private set; }

        internal Client To { get; private set; }

        internal byte[] Body { get; private set; }

        internal Message(Client from, Client to, byte[] body)
        {
            From = from;
            To = to;
            Body = body;
        }
    }
}
