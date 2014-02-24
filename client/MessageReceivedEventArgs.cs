using System;

namespace client
{
	public class MessageReceivedEventArgs : EventArgs
	{
		public byte[] Message { get; set; }
		public DateTime Timestamp { get; set; }
	}
}

