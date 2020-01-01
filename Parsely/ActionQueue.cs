using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parsely.Builders;

namespace Parsely
{
    public class ActionQueue : IRunTasks
    {
        Queue<Action> queue;

        public bool IsEmpty => queue.Count == 0;

        public ActionQueue() => queue = new Queue<Action>();

        public bool Contains(Action action) => queue.Contains(action);

        public Action Pop() => queue.Dequeue();

        public void Add(Action action) => queue.Enqueue(action);

        // public void Add(Func<object, bool> function)
        // {
        //     actions.Enqueue(function);
        // }

        public async Task RunAsync()
        {
            while (!IsEmpty)
            {
                var action = Pop();
                await Task.Run(action);
            }
        }
    }
}