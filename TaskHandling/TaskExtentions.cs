using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TaskHandling
{
	public static class TaskExtentions
	{
		public static Task HandleTask(this Task task, Action onCanceled = null, Action<string> onFaulted = null)
		{
			return task.ContinueWith(t =>
			{
				if (t.IsCanceled)
				{
					onCanceled?.Invoke();
					return;
				}

				if (t.IsFaulted)
				{
					var error = string.Join("; ", t.Exception.InnerExceptions.Select(x => x.Message));
					onFaulted?.Invoke(error);
					return;
				}

			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		public static async Task<T> HandleTask<T>(this Task<T> task, Func<T> onCanceled = null, Func<string, T> onFaulted = null)
		{
			var continuationTask = await task.ContinueWith<Task<T>>(t =>
			{
				if (t.IsCanceled && onCanceled != null)
				{
					return Task.FromResult(onCanceled.Invoke());
				}

				if (t.IsFaulted && onFaulted != null)
				{
					var error = string.Join("; ", t.Exception.InnerExceptions.Select(x => x.Message));
					return Task.FromResult(onFaulted.Invoke(error));
				}

				return t;
			}, TaskScheduler.FromCurrentSynchronizationContext());

			return await continuationTask;
		}

		public static async Task<T> OnCancel<T>(this Task<T> task, Func<T> onCanceled)
		{
			var continuationTask = await task.ContinueWith<Task<T>>((t, obj) =>
			{
				if (t.IsCanceled)
				{
					return Task.FromResult(onCanceled());
				}

				return t;
			}, TaskContinuationOptions.OnlyOnCanceled, TaskScheduler.FromCurrentSynchronizationContext());

			return await continuationTask;
		}

		public static async Task<T> OnFault<T>(this Task<T> task, Func<string, T> onFaulted)
		{
			var continuationTask = await task.ContinueWith<Task<T>>((t, obj) =>
			{
				if (t.IsFaulted)
				{
					var error = string.Join("; ", t.Exception.InnerExceptions.Select(x => x.Message));

					return Task.FromResult(onFaulted(error));
				}

				return t;
			}, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.FromCurrentSynchronizationContext());

			return await continuationTask;
		}

		public static async Task<Result<T>> HandleFault<T>(this Task<Result<T>> task)
		{
			var continuationTask = await task.ContinueWith<Task<Result<T>>>((t, obj) =>
			{
				if (t.IsFaulted)
				{
					var error = string.Join("; ", t.Exception.InnerExceptions.Select(x => x.Message));

					return Task.FromResult(Result<T>.Failure());
				}

				return t;
			}, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.FromCurrentSynchronizationContext());

			return await continuationTask;
		}

		public static Task<Result<T>> HandleFault<T>(this Task<T> task)
		{
			return task.ContinueWith<Result<T>>((t, obj) =>
			{
				if (t.IsFaulted)
				{
					var error = string.Join("; ", t.Exception.InnerExceptions.Select(x => x.Message));

					return Result<T>.Failure();
				}

				return Result<T>.Success(t.Result);
			}, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
