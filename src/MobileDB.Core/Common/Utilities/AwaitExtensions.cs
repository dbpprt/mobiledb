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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MobileDB.Common.Utilities
{
    public static partial class AwaitExtensions
    {
        /// <summary>
        /// Causes the caller who awaits this method to
        /// switch off the Main thread. It has no effect if
        /// the caller is already off the main thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable that does the thread switching magic.</returns>
        public static TaskSchedulerAwaiter SwitchOffMainThreadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new TaskSchedulerAwaiter(
                SynchronizationContext.Current != null ? TaskScheduler.Default : null,
                cancellationToken);
        }

        public struct TaskSchedulerAwaiter : INotifyCompletion
        {
            private readonly TaskScheduler _taskScheduler;
            private CancellationToken _cancellationToken;

            internal TaskSchedulerAwaiter(TaskScheduler taskScheduler, CancellationToken cancellationToken)
            {
                _taskScheduler = taskScheduler;
                _cancellationToken = cancellationToken;
            }

            public TaskSchedulerAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted
            {
                get { return _taskScheduler == null; }
            }

            public void OnCompleted(Action continuation)
            {
                if (_taskScheduler == null)
                {
                    throw new InvalidOperationException("IsCompleted is true, so this is unexpected.");
                }

                Task.Factory.StartNew(
                    continuation,
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    this._taskScheduler);
            }

            public void GetResult()
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
