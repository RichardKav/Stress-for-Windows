using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WindowsStress
{
    /**
     * The aim of this program is to load the cpu to a specific load. It is similar in that regard to the Linux tool Stress
     */
    class StressForWindows
    {
        /**
         * The parameters passed should be CPU Usage, Core count and finally duration
         */
        static void Main(string[] args)
        {
            /*
             * The code here is a modifed (to be parameterizable) version of the code at: 
             * http://stackoverflow.com/questions/2514544/simulate-steady-cpu-load-and-spikes
             * another interesting article covering this topic is:
             * http://stackoverflow.com/questions/5577098/how-to-run-cpu-at-a-given-load-cpu-utilization
             */

            // Test if input arguments were supplied:
            if (args.Length < 3)
            {
                System.Console.WriteLine("Please enter the cpu usage 0-100, core count and duration in seconds.");
                System.Console.WriteLine("Usage: StressForWindows <cpu usage> <all|0-n> <duration seconds>");
            }
            int cpuUsage = 100;
            int duration = 60000; //time in milliseconds
            int targetThreadCount = Environment.ProcessorCount;

            if (args.Length == 0 || !int.TryParse(args[0], out cpuUsage))
            {
                System.Console.WriteLine("Defaulting to 100% usage.");
            }
            //If thread count is not a number i.e. the word all then max processor count is used
            if (args.Length >= 2) {
                int.TryParse(args[1], out targetThreadCount);
                if (targetThreadCount == 0)
                {
                    targetThreadCount = Environment.ProcessorCount;
                }
            } else {
                System.Console.WriteLine("Defaulting all cores. Count = {0}.", Environment.ProcessorCount);
            }
            if (args.Length >= 3 && int.TryParse(args[2], out duration))
            {
                duration = duration * 1000; //convert seconds into milliseconds.
            } else {
                System.Console.WriteLine("Defaulting to 60 seconds.");
            }
            System.Console.WriteLine("Parameters: CPU: {0},Thread Count: {1}, Duration: {2}",cpuUsage, targetThreadCount, duration /  1000);
            List<Thread> threads = new List<Thread>();
            //Ensure the current process takes presendence thus (hopefully) holidng the utilisation steady
            Process Proc = Process.GetCurrentProcess();
            Proc.PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
            long AffinityMask = (long)Proc.ProcessorAffinity;
            for (int i = 0; i < targetThreadCount; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(CPUKill));
                t.Start(cpuUsage);
                threads.Add(t);
            }
            Thread.Sleep(duration);
            foreach (var t in threads)
            {
                t.Abort();
            }
        }

        public static void CPUKill(object cpuUsage)
        {
            Parallel.For(0, 1, new Action<int>((int i) =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (true)
                {
                    if (watch.ElapsedMilliseconds > (int)cpuUsage)
                    {
                        Thread.Sleep(100 - (int)cpuUsage);
                        watch.Reset();
                        watch.Start();
                    }
                }
            }));

        }

    }
}
