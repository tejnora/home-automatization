using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Tools
{
    public class AsyncSemaphore
    {
        static readonly Task _completed;
        readonly Queue<TaskCompletionSource<bool>> m_waiters = new Queue<TaskCompletionSource<bool>>();
        int _currentCount;

        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
            _currentCount = initialCount;
        }

        static AsyncSemaphore()
        {
            _completed = Task.Factory.StartNew(() => { });
        }

        public Task WaitAsync()
        {
            lock (m_waiters)
            {
                if (_currentCount > 0)
                {
                    --_currentCount;
                    return _completed;
                }
                var waiter = new TaskCompletionSource<bool>();
                m_waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (m_waiters)
            {
                if (m_waiters.Count > 0)
                    toRelease = m_waiters.Dequeue();
                else
                    ++_currentCount;
            }
            toRelease?.SetResult(true);
        }
    }
}