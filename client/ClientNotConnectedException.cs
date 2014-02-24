using System;
using System.Runtime.Serialization;

namespace client
{
	[Serializable]
	public class ClientNotConnectedException : Exception
	{
	    public ClientNotConnectedException()
	        : base() { }
	    
	    public ClientNotConnectedException(string message)
	        : base(message) { }
	    
	    public ClientNotConnectedException(string format, params object[] args)
	        : base(string.Format(format, args)) { }
	    
	    public ClientNotConnectedException(string message, Exception innerException)
	        : base(message, innerException) { }
	    
	    public ClientNotConnectedException(string format, Exception innerException, params object[] args)
	        : base(string.Format(format, args), innerException) { }
	    
	    protected ClientNotConnectedException(SerializationInfo info, StreamingContext context)
	        : base(info, context) { }
	}
}

