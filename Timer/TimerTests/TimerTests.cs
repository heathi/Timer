// Copyright 2020 Heath Isler
// This work is licensed under the terms of the MIT license.
// See `LICENSE` file for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimerTests
{
    [TestClass]
    public class TimerTests
    {
        [DataTestMethod]
        [DataRow(1, 5, 100)]
        [DataRow(1, 3, 100)]
        [DataRow(3, 1, 100)]
        public void SingleTimerTest(int duration, int iterations, int tolerance)
        {
            int count = 0;
            var delayTimeInMs = duration * iterations * 1000 + tolerance;

            var timer = new AlternateTimer.Timer();

            timer.Elapsed += (s, t) => TimerCallBack(ref count, s, t);
            timer.Start(duration);

            Task.Delay(delayTimeInMs).Wait();

            timer.Stop();

            Assert.AreEqual(iterations, count);
        }

        [DataTestMethod]
        [DataRow(1, 2, 5, 100)]
        [DataRow(1, 3, 3, 100)]
        [DataRow(3, 1, 1, 100)]
        public void TimerStartOverwrittenTest(int duration, int overwrite, int iterations, int tolerance)
        {
            int count = 0;
            var delayTimeInMs = duration * iterations * 1000 + tolerance;

            var timer = new AlternateTimer.Timer();

            timer.Elapsed += (s, t) => TimerCallBack(ref count, s, t);
            timer.Start(duration);
            timer.Start(overwrite);

            Task.Delay(delayTimeInMs).Wait();

            timer.Stop();

            var expected = delayTimeInMs / (overwrite * 1000);
            Assert.AreEqual(expected, count);
        }

        [DataTestMethod]
        [DataRow(3, 100)]
        [DataRow(1, 100)]
        public void MultipleTimerTest(int iterations, int tolerance)
        {
            var tasks = new List<Task>();
            var durations = new[] {1, 3, 5};

            foreach (var duration in durations)
            {
                var task = LaunchTimer(duration, iterations, tolerance);
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        [DataTestMethod]
        [DataRow(7, 100)]
        [DataRow(13, 100)]
        public void RandomizedDelayMultipleTimerTest(int iterations, int tolerance)
        {
            var tasks = new List<Task>();
            var durations = new[] { 1, 3, 5 };
            var random = new Random();

            foreach (var duration in durations)
            {
                var task = LaunchTimer(duration, iterations, tolerance);
                tasks.Add(task);

                var msDelay = random.Next(999);
                Task.Delay(msDelay).Wait();
            }

            Task.WaitAll(tasks.ToArray());
        }

        private async Task LaunchTimer(int duration, int iterations, int tolerance)
        {
            var count = 0;
            var delayTimeInMs = duration * iterations * 1000 + tolerance;

            var timer = new AlternateTimer.Timer();

            timer.Elapsed += (s, t) => TimerCallBack(ref count, s, t);
            timer.Start(duration);

            await Task.Delay(delayTimeInMs);
            timer.Stop();

            Assert.AreEqual(iterations, count);
        }

        [DataTestMethod]
        [DataRow(1, 5, 100)]
        public void TimerTestNotStarted(int duration, int iterations, int tolerance)
        {
            int count = 0;
            var delayTimeInMs = duration * iterations * 1000 + tolerance;

            var timer = new AlternateTimer.Timer();

            timer.Elapsed += (s, t) => TimerCallBack(ref count, s, t);
            timer.Stop();

            Task.Delay(delayTimeInMs).Wait();

            Assert.AreEqual(0, count);
        }

        public void TimerCallBack(ref int count, object sender, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            count++;
        }
    }
}
