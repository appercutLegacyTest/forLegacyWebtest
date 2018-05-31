using System;
using System.Threading;

namespace TraceContext
{
    class Program
    {
        internal static Random Rnd { get; private set; } = new Random();
        internal static bool InProgress { get; private set; }
        internal static Thread[] Threads { get; private set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Choose the task number:");
            Console.WriteLine("1 - Measure multiple calls.");
            Console.WriteLine("2 - Get Trace summary report.");
            var taskNumbers = new int[] { 1, 2};
            int taskNumber = 0;
            while (!int.TryParse(Console.ReadLine(), out taskNumber) || Array.IndexOf(taskNumbers, taskNumber)== -1)
            {
                Console.WriteLine("Incorrect value.");
            }

            switch (taskNumber)
            {
                case 1:
                    MeasureMultipleCalls();
                    break;
                case 2:
                    ReadUserParameters(out int threadsCount, out int totalSeconds);
                    MeasureMultipleCallsInThreads(threadsCount, totalSeconds);
                    break;
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public static void MeasureMultipleCalls()
        {
            var traceContext = new TraceContext();
            var testMethodRunner = new TestMethods();
            testMethodRunner.Test1000(traceContext, 100);
            testMethodRunner.Test1000(traceContext, 200);
            testMethodRunner.Test1000(traceContext, 300);

            Console.WriteLine(traceContext.ToString());
        }

        public static void MeasureMultipleCallsInThreads(int threadsCount, int totalSeconds)
        {
            
            var traceContext = new TraceContext();
            StartThreads(threadsCount, traceContext);
            Thread.Sleep(totalSeconds * 1000);
            StopThreads(traceContext);

            var summary = new TraceSummary();
            Console.WriteLine(summary.PrintReport(traceContext));
        }

        private static void ReadUserParameters(out int threadsCount, out int totalSeconds)
        {
            const string enterNumberString = "Enter the number of threads:";
            const string enterTimeString = "Enter the total time in seconds:";

            Console.Write(enterNumberString);
            while (!int.TryParse(Console.ReadLine(), out threadsCount) && threadsCount > 0)
            {
                Console.WriteLine("Incorrect value");
                Console.Write(enterNumberString);
            }

            Console.Write(enterTimeString);
            while (!int.TryParse(Console.ReadLine(), out totalSeconds) && totalSeconds > 0)
            {
                Console.WriteLine("Incorrect value");
                Console.Write(enterTimeString);
            }
        }


        public static void RunRandomMethods(Object obj)
        {
            var traceContext = (TraceContext)obj;
            var testMethodRunner = new TestMethods();
            var nextAdditionalDelay = Rnd.Next(0, 1001);
            var nextMethod = Rnd.Next(0, 3);

            while (InProgress)
            {
                switch (nextMethod)
                {
                    case 0:
                        testMethodRunner.Test1000(traceContext, nextAdditionalDelay);
                        break;
                    case 1:
                        testMethodRunner.Test2000(traceContext, nextAdditionalDelay);
                        break;
                    case 2:
                        testMethodRunner.Test3000(traceContext, nextAdditionalDelay);
                        break;
                }
            }
        }

        private static void StartThreads(int threadsCount, TraceContext traceContext)
        {
            InProgress = true;
            Threads = new Thread[threadsCount];

            for (var i = 0; i < threadsCount; i++)
            {
                var thread = new Thread(RunRandomMethods);
                thread.Start(traceContext);
                Threads[i] = thread;
            }
        }

        private static void StopThreads(TraceContext traceContext)
        {
            InProgress = false;

            for (var i = 0; i < Threads.Length; i++)
            {
                Threads[i].Abort();
            }

            TraceItem[] traceItems;
            lock (traceContext.TraceItemsList)
            {
                traceItems = traceContext.TraceItemsList.ToArray();
            }

            foreach (var item in traceItems)
            {
                if (!item.IsStopped)
                {
                    item.Cancel();
                }
            }
        }

       

    }
}
