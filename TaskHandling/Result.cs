using System;

namespace TaskHandling
{
	public class Result<T>
	{
		public T Value { get; set; }
		public bool Succeed { get; set; }

		public static Result<T> Failure()
		{
			return new Result<T> { Succeed = false };
		}

		internal static Result<T> Success(T result)
		{
			return new Result<T> { Value = result, Succeed = true };
		}
	}
}
