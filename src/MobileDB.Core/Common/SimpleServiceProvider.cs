#region Copyright (C) 2014 Dennis Bappert
// The MIT License (MIT)

// Copyright (c) 2014 Dennis Bappert

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

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
