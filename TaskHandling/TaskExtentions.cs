using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TaskHandling
{
	public static class TaskExtentions
	{
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

		public static async Task<TResult> OnFault<TResult, TException>(this Task<TResult> task, Func<TException, TResult> onFaulted)
		{
			var continuationTask = await task.ContinueWith<Task<TResult>>((t, obj) =>
			{
				if (t.IsFaulted && t.Exception.Flatten().InnerExceptions.OfType<TException>().FirstOrDefault() is TException exception)
				{
					return Task.FromResult(onFaulted(exception));
				}

				return t;
			}, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.FromCurrentSynchronizationContext());

			return await continuationTask;
		}
		
		public static async Task OnFault<TException>(this Task task, Action<TException> onFaulted)
		{
			var continuationTask = await task.ContinueWith<Task>((t, obj) =>
			{
				if (t.IsFaulted && t.Exception.Flatten().InnerExceptions.OfType<TException>().FirstOrDefault() is TException exception)
				{
					onFaulted(exception);
					return Task.CompletedTask;
				}

				return t;

			}, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.FromCurrentSynchronizationContext());

			await continuationTask;
		}
	}
}
