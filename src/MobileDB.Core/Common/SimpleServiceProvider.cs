using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MobileDB.Common
{
    public class SimpleServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, Func<object>> _registrations =
            new Dictionary<Type, Func<object>>();

        public void Register<TService, TImplementation>()
            where TImplementation : TService
        {
            _registrations.Add(typeof (TService),
                () => GetInstance<TImplementation>());
        }

        public void Register<TService>(Func<TService> instanceCreator)
        {
            _registrations.Add(typeof (TService), () => instanceCreator());
        }

        public void Register<TService>()
        {
            _registrations.Add(typeof(TService),
                () => GetInstance<TService>());
        }

        public void RegisterSingle<TService>(TService instance)
        {
            _registrations.Add(typeof (TService), () => instance);
        }

        public void RegisterSingle<TService>(Func<TService> instanceCreator)
        {
            var lazy = new Lazy<TService>(instanceCreator);
            Register(() => lazy.Value);
        }

        public TService GetInstance<TService>()
        {
            return (TService) GetService(typeof (TService));
        }

        public object GetService(Type serviceType)
        {
            Func<object> creator;
            if (!_registrations.TryGetValue(serviceType, out creator))
            {
                if (!serviceType.GetTypeInfo().IsAbstract)
                {
                    return CreateConcreteType(serviceType);
                }

                throw new InvalidOperationException(
                    "No registration for " + serviceType);
            }

            return creator.Invoke();
        }

        private object CreateConcreteType(Type implementationType)
        {
            var ctors = implementationType.GetTypeInfo().DeclaredConstructors;
            var constructableConstructors = new Dictionary<ConstructorInfo, ParameterInfo[]>();

            foreach (var ctor in ctors)
            {
                var parameters = ctor.GetParameters();
                var constructable = false;

                foreach (var parameter in parameters)
                {
                    constructable = _registrations.ContainsKey(parameter.ParameterType);
                }

                if (constructable)
                {
                    constructableConstructors.Add(ctor, parameters);
                }
            }

            var selectedCtor =
                constructableConstructors.FirstOrDefault(
                    _ => _.Value.Count() == constructableConstructors.Max(c => c.Value.Count()));

            return selectedCtor.Key.Invoke(selectedCtor.Value.Select(_ => GetService(_.ParameterType)).ToArray());
        }
    }
}