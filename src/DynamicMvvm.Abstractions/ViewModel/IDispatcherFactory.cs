namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This factory is used to abstract the creation of <see cref="IDispatcher"/> objects.
	/// </summary>
	public interface IDispatcherFactory
	{
		/// <summary>
		/// Creates a new <see cref="IDispatcher"/> using the provided <paramref name="view"/> reference.
		/// </summary>
		/// <param name="view">The native view object.</param>
		/// <returns>A new <see cref="IDispatcher"/> instance.</returns>
		IDispatcher Create(object view);
	}
}
