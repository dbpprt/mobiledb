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

namespace MobileDB.Common.Factory
{
    public class Builder<T>
    {
        private readonly Dictionary<string, string> _tuples;

        public Builder(Dictionary<string, string> tuples)
        {
            _tuples = tuples;
        }

        public T Build()
        {
            var connectionString = ConnectionString;
            return (T) Activator.CreateInstance(typeof (T), connectionString);
        }

        public string ConnectionString
        {
            get
            {
                var segments = new List<string>();

                foreach (var tuple in _tuples)
                {
                    var segment = String.Format("{0}{1}{2}", tuple.Key, ConnectionStringConstants.SegmentSeperator,
                        tuple.Value);
                    segments.Add(segment);
                }

                var connectionString = string.Join(
                    ConnectionStringConstants.TupleSeperator.ToString(),
                    segments);
                return connectionString;
            }
        }
    }
}
