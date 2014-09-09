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
using System.Linq;
using System.Reflection;
using MobileDB.Common.Attributes;
using MobileDB.Exceptions;

namespace MobileDB.Common.Utilities
{
    public static class IdentityHelper
    {
        public static object GetKeyFromEntity(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var type = obj.GetType();

            var identityProperty = type.GetKeyFromEntityType();

            return identityProperty == null
                ? null
                : identityProperty.GetValue(obj);
        }

        public static void SetEntityKey(this object obj, object key)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var type = obj.GetType();

            var identityProperty = type.GetKeyFromEntityType();
            identityProperty.SetValue(obj, key);
        }

        public static PropertyInfo GetKeyFromEntityType(this Type type)
        {
            var properties = type.GetRuntimeProperties();
            PropertyInfo identityProperty;
            try
            {
                identityProperty = properties
                    .GroupBy(attr => attr.GetCustomAttribute<IdentityAttribute>())
                    .Where(group => @group != null && @group.Key != null)
                    .Where(prop => prop.Any())
                    .Select(prop => prop.SingleOrDefault())
                    .SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw new IdentityAttributeMismatchException("You must only specify one primary key per entity");
            }

            return identityProperty;
        }
    }
}
