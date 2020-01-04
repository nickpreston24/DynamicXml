using System;
using System.Linq;

namespace Parsely.Test.Extractables
{
    internal class Store : IEquatable<Store>
    {
        public string Name { get; set; }
        public Product[] Products { get; set; }
        public Customer[] Customers { get; set; }

        public bool Equals(Store other)
        {
            return Products == null || Customers == null || string.IsNullOrWhiteSpace(Name)
                || other != null
                && Products.SequenceEqual(other?.Products)
                && Customers.SequenceEqual(other?.Customers)
                && Name.Equals(other?.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;

            return ReferenceEquals(this, obj) || Equals(obj as Store);
        }

        public override string ToString()
        {
            return $"{Name}\n Products: {Products?.Length ?? 0}\n Customers: {Customers?.Length ?? 0}";
        }
    }
}