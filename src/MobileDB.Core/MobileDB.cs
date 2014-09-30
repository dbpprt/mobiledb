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
using MobileDB.Common.Validation;
using MobileDB.Contracts;
using MobileDB.FileSystem;

namespace MobileDB
{
    public static class MobileDB
    {
        private static readonly List<Type> SupportedFileSystems;
        private static IEntityValidator _entityValidator;

        private static readonly object Sync;

        static MobileDB()
        {
            SupportedFileSystems = new List<Type>();
            Sync = new object();

            RegisterDefaultServices();
        }

        public static IEntityValidator Validator
        {
            get
            {
                lock (Sync)
                {
                    return _entityValidator;
                }
            }
            set
            {
                lock (Sync)
                {
                    _entityValidator = value;
                }
            }
        }

        private static void RegisterDefaultServices()
        {
            SupportedFileSystems.Add(typeof (MemoryFileSystem));
            _entityValidator = new EntityValidator();
        }

        public static void AddFileSystem(Type type)
        {
            lock (Sync)
            {
                if (!SupportedFileSystems.Contains(type))
                    SupportedFileSystems.Add(type);
            }
        }

        public static IEnumerable<Type> FileSystems()
        {
            lock (Sync)
            {
                return SupportedFileSystems.ToList();
            }
        }
    }
}