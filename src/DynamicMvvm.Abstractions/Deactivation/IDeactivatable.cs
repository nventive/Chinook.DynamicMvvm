using System;
using System.Collections.Generic;
using System.Text;

namespace Chinook.DynamicMvvm.Deactivation
{
	/// <summary>
	/// Represents something that can be deactivated and reactivated.
	/// </summary>
	public interface IDeactivatable
	{
		/// <summary>
		/// Gets whether this object is deactivated.
		/// This is false by default.
		/// </summary>
		bool IsDeactivated { get; }

		/// <summary>
		/// Deactivates this object.
		/// </summary>
		void Deactivate();

		/// <summary>
		/// Reactivates this object.
		/// </summary>
		void Reactivate();
	}
}
