using System;
using System.Collections.Generic;

namespace Parsely
{
    public class ActionQueue
    {
        Queue<Action> actions;

        public bool IsEmpty => (actions.Count == 0);

        public ActionQueue() => actions = new Queue<Action>();

        public bool Contains(Action action) => actions.Contains(action);

        public Action Pop() => actions.Dequeue();

        public void Add(Action action) => actions.Enqueue(action);

        // public void Add(Func<object, bool> function)
        // {
        //     actions.Enqueue(function);
        // }
    }
}