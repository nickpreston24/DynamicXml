using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicXml.Classes
{
    public struct Maybe<T>
    {
        private readonly IEnumerable<T> values;
        public bool HasValue => values != null && values.Any();
        public T Value => (HasValue) ? values.Single() : throw new InvalidOperationException($"Maybe<{typeof(T).Name}> does not have a value!");
        public static Maybe<T> None => new Maybe<T>(new T[0]);
        public static Maybe<IEnumerable<T>> Empty => new Maybe<IEnumerable<T>>(Enumerable.Repeat(new T[0], 0));

        private Maybe(IEnumerable<T> values) => this.values = values;

        public static Maybe<T> Some(T value) => (value != null) ? new Maybe<T>(new[] { value }) : throw new InvalidOperationException();

        public static Maybe<IEnumerable<T>> Some(IEnumerable<T> values) => (values != null) ?
            new Maybe<IEnumerable<T>>(new[] { values }) : throw new InvalidCastException();

        public Maybe<U> Map<U>(Func<T, U> mapper) => HasValue ? Maybe<U>.Some(mapper(Value)) : Maybe<U>.None;

        public U Case<U>(Func<T, U> some, Func<U> none) => HasValue ? some(Value) : none();
    }
}