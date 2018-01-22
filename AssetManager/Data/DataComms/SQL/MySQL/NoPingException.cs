using System;
namespace AssetManager.Data.DataComms
{
    public class NoPingException : Exception
	{

		public NoPingException()
		{
		}

		public NoPingException(string message) : base(message)
		{
		}

		public NoPingException(string message, Exception inner) : base(message, inner)
		{
		}

	}
}
