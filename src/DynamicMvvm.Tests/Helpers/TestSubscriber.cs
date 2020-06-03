using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm.Tests.Helpers
{
	public class TestSubscriber<T> : IObserver<T>
	{
		private readonly Action<T> _onNext;
		private readonly Action<Exception> _onError;
		private readonly Action _onCompleted;

		public TestSubscriber(Action<T> onNext = null, Action<Exception> onError = null, Action onCompleted = null)
		{
			_onNext = onNext;
			_onError = onError;
			_onCompleted = onCompleted;
		}

		public void OnNext(T value) => _onNext?.Invoke(value);

		public void OnError(Exception error) => _onError?.Invoke(error);

		public void OnCompleted() => _onCompleted?.Invoke();
	}
}
