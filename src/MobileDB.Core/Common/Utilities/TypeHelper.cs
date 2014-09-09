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
using System.Text.RegularExpressions;

namespace MobileDB.Common.Utilities
{
    // http://stackoverflow.com/questions/2276384/loading-a-generic-type-by-name-when-the-generics-arguments-come-from-multiple-as
    internal static class TypeHelper
    {
        public static string ToFriendlyName(this Type type)
        {
            return type.Name.Pluralize().ToLower();
        }

        public static string ToFriendlyNameFromGeneric(this Type type)
        {
            var thingyType = type.GenericTypeArguments.Single();
            return thingyType.ToFriendlyName();
        }

        /// <summary>
        ///     Gets the type associated with the specified name.
        /// </summary>
        /// <param name="typeName">Full name of the type.</param>
        /// <param name="type">The type.</param>
        /// <param name="customAssemblies">Additional loaded assemblies (optional).</param>
        /// <returns>Returns <c>true</c> if the type was found; otherwise <c>false</c>.</returns>
        public static bool TryGetTypeByName(string typeName, out Type type, params Assembly[] customAssemblies)
        {
            if (typeName.Contains("Version=")
                && !typeName.Contains("`"))
            {
                // remove full qualified assembly type name
                typeName = typeName.Substring(0, typeName.IndexOf(','));
            }

            type = Type.GetType(typeName);

            if (type == null)
            {
                type = GetTypeFromAssemblies(typeName, customAssemblies);
            }

            // try get generic types
            if (type == null
                && typeName.Contains("`"))
            {
                var match = Regex.Match(typeName, "(?<MainType>.+`(?<ParamCount>[0-9]+))\\[(?<Types>.*)\\]");

                if (match.Success)
                {
                    var genericParameterCount = int.Parse(match.Groups["ParamCount"].Value);
                    var genericDef = match.Groups["Types"].Value;
                    var typeArgs = new List<string>(genericParameterCount);
                    foreach (Match typeArgMatch in Regex.Matches(genericDef, "\\[(?<Type>.*?)\\],?"))
                    {
                        if (typeArgMatch.Success)
                        {
                            typeArgs.Add(typeArgMatch.Groups["Type"].Value.Trim());
                        }
                    }

                    var genericArgumentTypes = new Type[typeArgs.Count];
                    for (var genTypeIndex = 0; genTypeIndex < typeArgs.Count; genTypeIndex++)
                    {
                        Type genericType;
                        if (TryGetTypeByName(typeArgs[genTypeIndex], out genericType, customAssemblies))
                        {
                            genericArgumentTypes[genTypeIndex] = genericType;
                        }
                        else
                        {
                            // cant find generic type
                            return false;
                        }
                    }

                    var genericTypeString = match.Groups["MainType"].Value;
                    Type genericMainType;
                    if (TryGetTypeByName(genericTypeString, out genericMainType))
                    {
                        // make generic type
                        type = genericMainType.MakeGenericType(genericArgumentTypes);
                    }
                }
            }

            return type != null;
        }

        private static Type GetTypeFromAssemblies(string typeName, params Assembly[] customAssemblies)
        {
            if (customAssemblies != null
                && customAssemblies.Length > 0)
            {
                foreach (var type in customAssemblies
                    .Select(assembly => assembly.GetType(typeName))
                    .Where(type => type != null))
                {
                    return type;
                }
            }

            return ServiceLocator.Stores().FirstOrDefault(_ => _.FullName == typeName);
        }
    }
}
