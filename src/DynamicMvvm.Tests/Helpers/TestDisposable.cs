using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm.Tests.Helpers
{
	public class TestDisposable : IDisposable
	{
		private readonly Action _onDispose;

		public TestDisposable(Action onDispose = null)
		{
			_onDispose = onDispose;
		}

		public void Dispose() => _onDispose?.Invoke();
	}
}
