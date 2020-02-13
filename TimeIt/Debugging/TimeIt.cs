using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TimeIt.Debugging
{
    public class TimeIt : IDisposable
    {
        readonly string name;
        readonly TimeSpanUnit timeSpanUnit;
        readonly Stopwatch watch;
        TimeSpan elapsed;
        int units;
        Action callback;
        public TimeSpan Elapsed => elapsed;

        public static TimeIt Instance => Start();
        
        public static TimeIt Start(
            TimeSpanUnit timeSpanUnit = TimeSpanUnit.Milliseconds,
            Action callback = null,
            [CallerMemberName] string name = ""
        ) => new TimeIt(timeSpanUnit, callback, name);
        
        TimeIt(TimeSpanUnit timeSpanUnit = TimeSpanUnit.Milliseconds, Action callback = null, [CallerMemberName] string name = "")
        {
            this.name = name;
            this.timeSpanUnit = timeSpanUnit;
            watch = Stopwatch.StartNew();
        }

        static Dictionary<TimeSpanUnit, Func<TimeSpan, int>> spans = new Dictionary<TimeSpanUnit, Func<TimeSpan, int>>()
        {
            [TimeSpanUnit.Default] = timeSpan => timeSpan.Milliseconds,
            [TimeSpanUnit.Minutes] = timeSpan => timeSpan.Days,
            [TimeSpanUnit.Hours] = timeSpan => timeSpan.Hours,
            [TimeSpanUnit.Minutes] = timeSpan => timeSpan.Minutes,
            [TimeSpanUnit.Seconds] = timeSpan => timeSpan.Seconds,
            [TimeSpanUnit.Milliseconds] = timeSpan => timeSpan.Milliseconds,
            //[TimeSpanUnit.Ticks] = timeSpan => timeSpan.Ticks, //TODO: add a cast to long in this dictionary somehow
        };

        public void Dispose()
        {
            watch.Stop();
            elapsed = watch.Elapsed;
            units = spans[timeSpanUnit](elapsed);
            callback();
            Print();
        }

        void Print()
        {
            //Don't print zeroes:
            if (units == 0)
                return;

            Console.WriteLine(ToString());
            System.Diagnostics.Debug.WriteLine(ToString());
        }

        public override string ToString()
        {
            string unitName = Enum.GetName(typeof(TimeSpanUnit), timeSpanUnit);
            return $"{name} took {units} {unitName}";
        }

        public enum TimeSpanUnit
        {
            Default,
            Minutes,
            Hours,
            Seconds,
            Milliseconds,
            //Ticks,
        }
    }

    public static class TimeItExtensions
    {
        /* Basic support for diagnosing funcs, lambdas, etc. */
        public static void WithTimer(this Action action)
        {
            using (TimeIt.Start())
                action();
        }
        public static A WithTimer<A>(this Func<A> action)
        {
            using (TimeIt.Start())
                return action();
        }

        public static R WithTimer<A, R>(this Func<A, R> action, A value)
        {
            using (TimeIt.Start())
                return action(value);
        }
    }
}