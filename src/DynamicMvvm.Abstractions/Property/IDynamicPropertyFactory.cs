﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This interface is used to create <see cref="IDynamicProperty"/>.
	/// </summary>
	public interface IDynamicPropertyFactory
	{
		/// <summary>
		/// Creates a <see cref="IDynamicProperty"/> with the specified <paramref name="name"/> and <paramref name="initialValue"/>.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="name">Property name</param>
		/// <param name="initialValue">Initial value</param>
		/// <returns>IDynamicProperty</returns>
		IDynamicProperty Create<T>(
			string name,
			T initialValue = default
		);

		/// <summary>
		/// Creates a <see cref="IDynamicProperty"/> with the specified <paramref name="name"/> and <paramref name="initialValue"/>.
		/// This property will be updated once the <paramref name="source"/> task is complete.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="name">Property name</param>
		/// <param name="source">Property source</param>
		/// <param name="initialValue">Initial value</param>
		/// <returns>IDynamicProperty</returns>
		IDynamicProperty CreateFromTask<T>(
			string name,
			Func<CancellationToken, Task<T>> source,
			T initialValue = default
		);

		/// <summary>
		/// Creates a <see cref="IDynamicProperty"/> with the specified <paramref name="name"/> and <paramref name="initialValue"/>.
		/// This property will be updated when the <paramref name="source"/> observable pushes new values.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="name">Property name</param>
		/// <param name="source">Property source</param>
		/// <param name="initialValue">Initial value</param>
		/// <returns>IDynamicProperty</returns>
		IDynamicProperty CreateFromObservable<T>(
			string name,
			IObservable<T> source,
			T initialValue = default
		);
	}
}
