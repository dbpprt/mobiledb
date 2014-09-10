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
