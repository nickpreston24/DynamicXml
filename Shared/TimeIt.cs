using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Shared
{
    public class TimeIt : IDisposable
    {
        private readonly string name;
        private Stopwatch watch;
        public TimeSpan Elapsed { get; private set; }

        public TimeIt([CallerMemberName] string name = "")
        {
            this.name = name;
            watch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            watch.Stop();
            Elapsed = watch.Elapsed;
            string message = string.Format("{0} took {1}", name, watch.Elapsed);
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}