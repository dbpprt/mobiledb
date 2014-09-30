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
using MobileDB.Exceptions;

namespace MobileDB.Common
{
    public class ConnectionString
    {
        private readonly string _connectionString;
        private readonly Dictionary<string, string> _connectionStringTuples;

        public ConnectionString(
            string connectionString
            )
        {
            _connectionString = connectionString;

            var segments = connectionString.Split(ConnectionStringConstants.TupleSeperator);
            _connectionStringTuples = segments
                .Select(segment => segment.Split(ConnectionStringConstants.SegmentSeperator))
                .ToDictionary(parts => parts.First().ToLowerInvariant().Trim(), parts => parts.Last().Trim());
        }

        public override string ToString()
        {
            return _connectionString;
        }

        public string GetPart(string key)
        {
            string value;
            if (!_connectionStringTuples.TryGetValue(key, out value))
            {
                throw new InvalidConnectionStringException(
                    String.Format("ConnectionString must provide a valid {0} segment",
                        key),
                    _connectionString);
            }

            return value;
        }
    }
}