using System;

namespace TaskHandling
{
	public class CustomException : Exception
	{
		public CustomException(string exc) : base(exc)
		{

		}
	}
}
