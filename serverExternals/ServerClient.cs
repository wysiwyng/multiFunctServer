using System;

namespace serverExternals
{
	public class ServerClient
	{
		public int ClientID { get; set; }

		public byte[] Address { get; set; }

		public ServerClient()
		{
			Address = new byte[4];
		}

		public ServerClient(int id)
		{
			Address = new byte[4];
			ClientID = id;
		}

		public ServerClient(int id, byte[] ip)
		{
			Address = ip;
			ClientID = id;
		}

		public override string ToString()
		{
			return "";
		}
	}
}

