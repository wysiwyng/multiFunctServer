namespace serverExternals
{
	public class ServerClient
	{
		public byte ClientID { get; set; }

		public byte[] Address { get; set; }

		public ServerClient()
		{
			Address = new byte[4];
		}

		public ServerClient(byte id)
		{
			Address = new byte[4];
			ClientID = id;
		}

		public ServerClient(byte id, byte[] ip)
		{
			Address = ip;
			ClientID = id;
		}

        public byte[] serialize()
        {
            return new byte[] { ClientID, Address[0], Address[1], Address[2], Address[3] };
        }
	}
}

