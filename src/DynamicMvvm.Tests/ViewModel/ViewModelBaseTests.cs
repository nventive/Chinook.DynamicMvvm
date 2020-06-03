using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.ViewModel
{
	public class ViewModelBaseTests
	{
		[Fact]
		public void It_Can_Be_Created_Without_Parameters()
		{
			new ViewModelBase();
		}
	}
}
