// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/License.md
// See reference: https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/Microsoft.Toolkit.Uwp/Extensions/DispatcherQueueExtensions.cs

using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using System.Runtime.CompilerServices;

namespace Chinook.DynamicMvvm.Extensions
{
	/// <summary>
	/// This class exposes extensions methods on <see cref="DispatcherQueue"/>.
	/// </summary>
	internal static class DispatcherQueueExtensions
	{
		/// <summary>
		/// Invokes a given function on the target <see cref="DispatcherQueue"/> and returns a
		/// <see cref="Task"/> that completes when the invocation of the function is completed.
		/// </summary>
		/// <param name="taskCompletionSource">The <see cref="TaskCompletionSource"/> that will set the result or Exception depending on the result of the invocation</param>
		/// <param name="handler">The <see cref="DispatcherQueueHandler"/> to invoke.</param>
		/// <returns>A <see cref="DispatcherQueueHandler"/> that completes when the invocation of <paramref name="handler"/> is over.</returns>
		internal static DispatcherQueueHandler InvokeHandler(DispatcherQueueHandler handler, TaskCompletionSource<object?> taskCompletionSource)
		{
			try
			{
				handler.Invoke();

				taskCompletionSource.SetResult(null);
			}
			catch (Exception e)
			{
				taskCompletionSource.SetException(e);
			}

			return handler;
		}

		/// <summary>
		/// Invokes a given function on the target <see cref="DispatcherQueue"/> and returns a
		/// <see cref="Task"/> that completes when the invocation of the function is completed.
		/// </summary>
		/// <param name="dispatcher">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
		/// <param name="handler">The <see cref="DispatcherQueueHandler"/> to invoke.</param>
		/// <param name="priority">The priority level for the function to invoke.</param>
		/// <returns>A <see cref="Task"/> that completes when the invocation of <paramref name="handler"/> is over.</returns>
		/// <remarks>If the current thread has access to <paramref name="dispatcher"/>, <paramref name="handler"/> will be invoked directly.</remarks>
		internal static Task RunAsync(this DispatcherQueue dispatcher, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal, DispatcherQueueHandler handler = default)
		{
			// Run the function directly when we have thread access.
			// Also reuse Task.CompletedTask in case of success,
			// to skip an unnecessary heap allocation for every invocation.
			if (dispatcher.HasThreadAccess)
			{
				try
				{
					handler.Invoke();

					return Task.CompletedTask;
				}
				catch (Exception e)
				{
					return Task.FromException(e);
				}
			}

			return TryEnqueueAsync(dispatcher, handler, priority);
		}

		internal static Task TryEnqueueAsync(DispatcherQueue dispatcher, DispatcherQueueHandler handler, DispatcherQueuePriority priority)
		{
			var taskCompletionSource = new TaskCompletionSource<object>();
			DispatcherQueueHandler callback = InvokeHandler(handler, taskCompletionSource);

			if (!dispatcher.TryEnqueue(priority, callback))
			{
				taskCompletionSource.SetException(GetEnqueueException("Failed to enqueue the operation"));
			}

			return taskCompletionSource.Task;
		}

		/// <summary>
		/// Creates an <see cref="InvalidOperationException"/> to return when an enqueue operation fails.
		/// </summary>
		/// <param name="message">The message of the exception.</param>
		/// <returns>An <see cref="InvalidOperationException"/> with a specified message.</returns>
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static InvalidOperationException GetEnqueueException(string message)
		{
			return new InvalidOperationException(message);
		}
	}
}
