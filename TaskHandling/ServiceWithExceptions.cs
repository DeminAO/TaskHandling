using System;
using System.Threading.Tasks;

namespace TaskHandling
{
	public class ServiceWithExceptions
	{
		public async Task ThrowException()
		{
			await Task.Delay(500);
			throw new Exception("does not work");
		}

		public Task SomeCompletionWorkAsync()
		{
			return Task.CompletedTask;
		}

		public async Task<int> ThrowOperationCanceledExceptionAsync()
		{
			throw new OperationCanceledException(nameof(OperationCanceledException));
		}

		public async Task<int> ThrowTaskCanceledExceptionAsync()
		{
			await Task.Delay(500);
			throw new TaskCanceledException(nameof(TaskCanceledException));
		}

		public async Task<int> ThrowStackOverflowExceptionAsync()
		{
			throw new StackOverflowException(nameof(StackOverflowException));
		}

		public async Task<int> ThrowCustomExceptionAsync()
		{
			await Task.Delay(500);
			throw new CustomException(nameof(CustomException));
		}

		public async Task<Result<int>> ThrowCustomExceptionWithResultAsync()
		{
			throw new CustomException(nameof(CustomException));
		}
	}
}
