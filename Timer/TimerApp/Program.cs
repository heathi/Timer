// Copyright 2020 Heath Isler
// This work is licensed under the terms of the MIT license.
// See `LICENSE` file for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TimerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest("1 Second Test", 1, 2);
            RunTest("3 Second Test", 3, 3);
            RunTest("5 Second Test", 5, 4);

            Console.ReadLine();
        }

        public static void RunTest(string testName, int seconds, int iterations)
        {
            async Task Function()
            {
                var timer = new AlternateTimer.Timer();
                timer.Elapsed += (s, e) => WriteToConsole(testName, s, e);
                Console.WriteLine($"{testName} Start time:   {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");

                timer.Start(seconds);

                Task.Delay(seconds * iterations * 1000 + 900).Wait();

                await Task.Run(() => timer.Stop());
                Console.WriteLine($"{testName} Stop time:    {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
            }

            Task.Run(Function);
        }

        private static void WriteToConsole(string testName, object sender, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Console.WriteLine($"{testName} Current time: {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
        }
    }
}