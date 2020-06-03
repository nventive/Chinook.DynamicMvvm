using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm.Tests.Helpers
{
	public class TestViewModel : ViewModelBase
	{
		private readonly Action _onDispose;

		public TestViewModel()
		{

		}

		public TestViewModel(
			string name = null,
			IServiceProvider serviceProvider = null,
			Action onDispose = null)
			: base(name, serviceProvider)
		{
			_onDispose = onDispose;
		}

		public TestEntity ReadWriteEntity
		{
			get => this.Get<TestEntity>();
			set => this.Set(value);
		}

		public TestEntity ReadEntity => this.Get<TestEntity>();

		protected override void Dispose(bool isDisposing)
		{
			base.Dispose(isDisposing);

			_onDispose?.Invoke();
		}
	}
}
