using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TaskHandling
{
	public static class TaskExtentions
	{
		public static Task HandleExceptionsWithShowingMessage(this Task task)
		{
			return task.ContinueWith(t =>
			{
				if (t.IsCanceled)
				{
					MessageBox.Show("Задача отменена");
					return;
				}

				if (t.IsFaulted)
				{
					var error = string.Join("; ", t.Exception.InnerExceptions.Select(x => x.Message));
					MessageBox.Show(error);
					return;
				}

			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		public static Task<T> HandleExceptionsWithShowingMessage<T>(this Task<T> task)
		{
			return task.ContinueWith<T>(t =>
			{
				if (t.IsCanceled)
				{
					MessageBox.Show("Задача отменена");
					return default;
				}

				if (t.IsFaulted)
				{
					var error = string.Join("; ", t.Exception.InnerExceptions.Select(x => x.Message));
					MessageBox.Show(error);
					return default;
				}

				return t.Result;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		public static Task<Result<T>> HandleExceptionsWithShowingMessage<T> (this Task<Result<T>> task)
		{
			return task.ContinueWith<Result<T>>(t =>
			{
				if (t.IsCanceled)
				{
					MessageBox.Show("Задача отменена");
					return new Result<T> { Succeed = false };
				}

				if (t.IsFaulted)
				{
					var error = string.Join("; ", t.Exception.InnerExceptions.Select(x => x.Message));
					MessageBox.Show(error);
					return new Result<T> { Succeed = false };
				}

				return t.Result;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
