using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TaskHandling
{
	public class MainWindowVM : BindableBase
	{
		private readonly Random rnd = new Random();
		private readonly ServiceWithExceptions someServiceWithExceptions = new ServiceWithExceptions();

		private string result;
		public string Result
		{
			get => result;
			set => SetProperty(ref result, value);
		}

		public ICommand CommandThrowException { get; private set; }
		public ICommand CommandSomeCompletionWorkAsync { get; private set; }
		public ICommand CommandThrowOperationCanceledExceptionAsync { get; private set; }
		public ICommand CommandThrowTaskCanceledExceptionAsync { get; private set; }
		public ICommand CommandThrowStackOverflowExceptionAsync { get; private set; }
		public ICommand CommandThrowCustomExceptionAsync { get; private set; }
		public ICommand CommandThrowCustomExceptionWithResultAsync { get; private set; }

		public MainWindowVM()
		{
			CommandThrowException = new DelegateCommand(() => _ = ThrowExceptionAsync());
			CommandSomeCompletionWorkAsync = new DelegateCommand(() => _ = SomeCompletionWorkAsync());
			CommandThrowOperationCanceledExceptionAsync = new DelegateCommand(() => _ = ThrowOperationCanceledExceptionAsync());
			CommandThrowTaskCanceledExceptionAsync = new DelegateCommand(() => _ = ThrowTaskCanceledExceptionAsync());
			CommandThrowStackOverflowExceptionAsync = new DelegateCommand(() => _ = ThrowStackOverflowExceptionAsync());
			CommandThrowCustomExceptionAsync = new DelegateCommand(() => _ = ThrowCustomExceptionAsync());
			CommandThrowCustomExceptionWithResultAsync = new DelegateCommand(() => _ = ThrowCustomExceptionWithResultAsync());
		}

		private async Task ThrowExceptionAsync()
		{
			await someServiceWithExceptions.ThrowException().OnFault<Exception>(e => Result = ToJson(e));
		}

		private async Task SomeCompletionWorkAsync()
		{
			await someServiceWithExceptions.SomeCompletionWorkAsync();
			Result = ToJson(rnd.Next());
		}

		private async Task ThrowOperationCanceledExceptionAsync()
		{
			Result = ToJson(await someServiceWithExceptions.ThrowOperationCanceledExceptionAsync().OnCancel(() => 0));
		}

		private async Task ThrowTaskCanceledExceptionAsync()
		{
			Result = ToJson(await someServiceWithExceptions.ThrowTaskCanceledExceptionAsync().OnCancel(() => 0));
		}

		private async Task ThrowStackOverflowExceptionAsync()
		{
			Result = ToJson(await someServiceWithExceptions.ThrowStackOverflowExceptionAsync()
				.OnFault<int, CustomException>(e => -8)
				.OnFault<int, StackOverflowException>(e => -7)
				.OnFault<int, Exception>(e => -9));
		}

		private async Task ThrowCustomExceptionAsync()
		{
			Result = ToJson(await someServiceWithExceptions.ThrowCustomExceptionAsync()
				.OnFault<int, CustomException>(e => -8));
		}

		private async Task ThrowCustomExceptionWithResultAsync()
		{
			Result = ToJson(await someServiceWithExceptions.ThrowCustomExceptionWithResultAsync()
				.OnFault<Result<int>, CustomException>(e => Result<int>.Failure(e)));
		}

		private string ToJson(object obj)
		{
			if (obj is null)
			{
				return string.Empty;
			}

			return JsonConvert.SerializeObject(obj);
		}
	}
}
