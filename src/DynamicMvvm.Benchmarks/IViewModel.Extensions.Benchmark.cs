using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DynamicMvvm.Benchmarks
{
	[MemoryDiagnoser]
	[MaxIterationCount(36)]
	[MaxWarmupCount(16)]
	public class ViewModelExtensionsBenchmark
	{
		internal static readonly IServiceProvider ServiceProvider = new HostBuilder()
			.ConfigureServices(serviceCollection => serviceCollection
				.AddSingleton<IDynamicCommandBuilderFactory, DynamicCommandBuilderFactory>()
				.AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>()
			)
			.Build()
			.Services;

		internal static readonly Type DynamicViewModelType = GetDynamicViewModelType();
		private const int PropertyCount = 32;
		private const int ViewModelCount = 1024 * 1024;
		private static string[] propertyNames = Enumerable.Range(0, PropertyCount).Select(i => $"Number{i}").ToArray();

		/// <summary>
		/// Generates a dynamic type that inherits from <see cref="ViewModelBase"/> and has <paramref name="propertyCount"/> properties.
		/// </summary>
		/// <param name="propertyCount">The amount of property defined in the dynamic class.</param>
		/// <returns>A dynamically generated type.</returns>
		internal static Type GetDynamicViewModelType(int propertyCount = PropertyCount)
		{
			// Create a new dynamic assembly
			AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
			AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

			// Create a new module within the assembly
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

			// Create a new type that inherits from ViewModelBase
			TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicClass", TypeAttributes.Public | TypeAttributes.Class, typeof(ViewModelBase));

			// Define a parameterless constructor
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(string), typeof(IServiceProvider) });
			ILGenerator constructorIL = constructorBuilder.GetILGenerator();
			constructorIL.Emit(OpCodes.Ldarg_0);
			constructorIL.Emit(OpCodes.Ldarg_1);
			constructorIL.Emit(OpCodes.Ldarg_2);
			constructorIL.Emit(OpCodes.Call, typeof(ViewModelBase).GetConstructor(new Type[] { typeof(string), typeof(IServiceProvider) })!);
			constructorIL.Emit(OpCodes.Ret);

			// There is a name conflict on IViewModelExtensions which requires us to use reflection to get it (because typeof(IViewModelExtensions) is ambiguous).
			var viewModelExtensionsType = Assembly.GetAssembly(typeof(IViewModel))!.GetType("Chinook.DynamicMvvm.IViewModelExtensions")!;

			for (int i = 0; i < propertyCount; i++)
			{
				// Define a 'NumberX' property with the specified getter and setter
				PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("Number" + i, PropertyAttributes.None, typeof(int), null);

				MethodBuilder getMethod = typeBuilder.DefineMethod("get_Number" + i, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(int), Type.EmptyTypes);
				ILGenerator getMethodIL = getMethod.GetILGenerator();
				getMethodIL.Emit(OpCodes.Ldarg_0);
				getMethodIL.Emit(OpCodes.Ldc_I4_S, 42); // Initial value
				getMethodIL.Emit(OpCodes.Ldstr, "Number" + i);
				var getMethodInfo = viewModelExtensionsType.GetMethods()
					.Where(m => m.Name == "Get" && m.IsGenericMethod)
					.First(m =>
					{
						var parameters = m.GetParameters();
						return parameters.Length == 3 && parameters[0].ParameterType == typeof(IViewModel) && parameters[2].ParameterType == typeof(string);
					});
				getMethodIL.EmitCall(OpCodes.Call, getMethodInfo.MakeGenericMethod(typeof(int)), null);
				getMethodIL.Emit(OpCodes.Ret);

				MethodBuilder setMethod = typeBuilder.DefineMethod("set_Number" + i, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { typeof(int) });
				ILGenerator setMethodIL = setMethod.GetILGenerator();
				setMethodIL.Emit(OpCodes.Ldarg_0);
				setMethodIL.Emit(OpCodes.Ldarg_1);
				setMethodIL.Emit(OpCodes.Ldstr, "Number" + i);
				var setMethodInfo = viewModelExtensionsType.GetMethods()
					.Where(m => m.Name == "Set" && m.IsGenericMethod)
					.First(m =>
					{
						var parameters = m.GetParameters();
						return parameters.Length == 3 && parameters[0].ParameterType == typeof(IViewModel) && parameters[2].ParameterType == typeof(string);
					});
				setMethodIL.EmitCall(OpCodes.Call, setMethodInfo.MakeGenericMethod(typeof(int)), null);
				setMethodIL.Emit(OpCodes.Ret);

				propertyBuilder.SetGetMethod(getMethod);
				propertyBuilder.SetSetMethod(setMethod);
			}

			// Create the type
			Type dynamicType = typeBuilder.CreateType();

			return dynamicType;
		}

		/// <summary>
		/// This vm always initializes new property instances when being invoked.
		/// </summary>
		private NeverInitiatedViewModel _neverInitiatedVM = new();

		private InitiatedViewModel _initiatedVM = new();

		private IViewModel[]? _vmsForPropertySetter;
		private int _i = 0;

		[GlobalSetup(Target = nameof(Set_Unresolved))]
		public void SetupVMForPropertySetter()
		{
			_i = 0;
			_vmsForPropertySetter = new IViewModel[ViewModelCount];
			for (int i = 0; i < ViewModelCount; i++)
			{
				_vmsForPropertySetter[i] = (IViewModel)Activator.CreateInstance(DynamicViewModelType, "ViewModelName", ServiceProvider)!;
			}
		}

		[Benchmark]
		public int GetFromValue_Unresolved()
		{
			return _neverInitiatedVM.Number;
		}

		[Benchmark]
		public int GetFromObservable_Unresolved()
		{
			return _neverInitiatedVM.ObservableNumber;
		}

		[Benchmark]
		public int GetFromValue_Resolved()
		{
			return _initiatedVM.Number;
		}

		[Benchmark]
		public int GetFromObservable_Resolved()
		{
			return _initiatedVM.ObservableNumber;
		}

		[Benchmark(OperationsPerInvoke = PropertyCount)]
		[MaxIterationCount(24)]
		public void Set_Unresolved()
		{
			var i = Interlocked.Increment(ref _i);
			var vm = _vmsForPropertySetter![i];
			for (int propertyIndex = 0; propertyIndex < PropertyCount; propertyIndex++)
			{
				vm!.Set(i, propertyNames[propertyIndex]);
			}
		}

		[Benchmark]
		public void Set_Resolved()
		{
			_initiatedVM.Number = 1;
		}
	}

	public sealed class NeverInitiatedViewModel : TestViewModelBase
	{
		public NeverInitiatedViewModel(IServiceProvider? serviceProvider = null)
			: base(serviceProvider ?? ViewModelExtensionsBenchmark.ServiceProvider)
		{
		}

		public int Number
		{
			get => this.Get<int>(initialValue: 42);
			set => this.Set(value);
		}

		public int ObservableNumber
		{
			get => this.GetFromObservable<int>(Observable.Never<int>(), initialValue: 0);
			set => this.Set(value);
		}
	}

	public sealed class InitiatedViewModel : TestViewModelWithProperty
	{
		public InitiatedViewModel()
		{
			ServiceProvider = ViewModelExtensionsBenchmark.ServiceProvider;

			Resolve(Number);
			Resolve(ObservableNumber);
		}

		public int Number
		{
			get => this.Get<int>(initialValue: 42);
			set => this.Set(value);
		}

		public int ObservableNumber
		{
			get => this.GetFromObservable<int>(Observable.Never<int>(), initialValue: 0);
			set => this.Set(value);
		}
	}

	public class TestViewModelWithProperty : TestViewModelBase
	{
		IDictionary<string, IDisposable> _disposables = new Dictionary<string, IDisposable>();

		protected void Resolve(object value)
		{
		}

		public override void AddDisposable(string key, IDisposable disposable)
		{
			_disposables[key] = disposable;
		}

		public override bool TryGetDisposable(string key, out IDisposable? disposable)
		{
			return _disposables.TryGetValue(key, out disposable);
		}
	}
}
