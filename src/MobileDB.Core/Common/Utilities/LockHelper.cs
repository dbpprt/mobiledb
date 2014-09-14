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
using System.Threading;

namespace MobileDB.Common.Utilities
{
    internal static class LockHelper
    {
        public static ReadLockHelper ReadLock(this ReaderWriterLockSlim readerWriterLock)
        {
            return new ReadLockHelper(readerWriterLock);
        }

        public static UpgradeableReadLockHelper UpgradableReadLock(this ReaderWriterLockSlim readerWriterLock)
        {
            return new UpgradeableReadLockHelper(readerWriterLock);
        }

        public static WriteLockHelper WriteLock(this ReaderWriterLockSlim readerWriterLock)
        {
            return new WriteLockHelper(readerWriterLock);
        }

        public struct ReadLockHelper : IDisposable
        {
            private readonly ReaderWriterLockSlim _readerWriterLock;

            public ReadLockHelper(ReaderWriterLockSlim readerWriterLock)
            {
                readerWriterLock.EnterReadLock();
                _readerWriterLock = readerWriterLock;
            }

            public void Dispose()
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        public struct UpgradeableReadLockHelper : IDisposable
        {
            private readonly ReaderWriterLockSlim _readerWriterLock;

            public UpgradeableReadLockHelper(ReaderWriterLockSlim readerWriterLock)
            {
                readerWriterLock.EnterUpgradeableReadLock();
                _readerWriterLock = readerWriterLock;
            }

            public void Dispose()
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }
        }

        public struct WriteLockHelper : IDisposable
        {
            private readonly ReaderWriterLockSlim _readerWriterLock;

            public WriteLockHelper(ReaderWriterLockSlim readerWriterLock)
            {
                readerWriterLock.EnterWriteLock();
                _readerWriterLock = readerWriterLock;
            }

            public void Dispose()
            {
                _readerWriterLock.ExitWriteLock();
            }
        }
    }
}

