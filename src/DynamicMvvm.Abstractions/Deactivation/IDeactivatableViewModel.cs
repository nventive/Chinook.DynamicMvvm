using System;
using System.Collections.Generic;
using System.Text;

namespace Chinook.DynamicMvvm.Deactivation
{
	/// <summary>
	/// Represents a ViewModel that implements deactivation.
	/// </summary>
	public interface IDeactivatableViewModel : IViewModel, IDeactivatable
	{
	}
}
