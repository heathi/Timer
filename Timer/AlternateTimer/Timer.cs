// Copyright 2020 Heath Isler
// This work is licensed under the terms of the MIT license.
// See `LICENSE` file for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlternateTimer
{
    public class Timer : IDisposable
    {
        public event EventHandler<CancellationToken> Elapsed;

        private CancellationTokenSource _tokenSource;

        private Task _timerTask;

        public void Start(int seconds)
        {
            _tokenSource?.Cancel();
            _timerTask?.Wait();

            _tokenSource = new CancellationTokenSource();
            _timerTask = Task.Run(() => CheckElapsedTime(seconds, _tokenSource.Token));
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
        }

        private async Task CheckElapsedTime(int seconds, CancellationToken cancellationToken)
        {
            var nextTime = DateTime.UtcNow.AddSeconds(seconds);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (DateTime.UtcNow >= nextTime)
                {
                    OnElapsed(cancellationToken);
                    nextTime = nextTime.AddSeconds(seconds);
                }

                var delayTime = Math.Max(nextTime.Millisecond - DateTime.UtcNow.Millisecond, 0);

                await Task.Delay(delayTime, cancellationToken);
            }
        }

        private void OnElapsed(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Elapsed?.Invoke(this, cancellationToken);
        }

        private void ReleaseUnmanagedResources()
        {
            // No Unmanaged Resources
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();

            if (disposing)
            {
                _tokenSource.Cancel();
                _timerTask.Wait();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Timer()
        {
            Dispose(false);
        }
    }
}
