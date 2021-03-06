namespace TaskHandling
{
	public class Result<T>
	{
		public T Value { get; set; }
		public bool Succeed { get; set; }
	}
}
