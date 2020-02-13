using System;
using System.Linq;

namespace Parsely.Test.Extractables
{
    internal class Store : IEquatable<Store>
    {
        public string Name { get; set; } = string.Empty;
        public Product[] Products { get; set; } = { };
        public Customer[] Customers { get; set; } = { };

        public bool Equals(Store other)
        {
            return Products == null || Customers == null || string.IsNullOrWhiteSpace(Name)
                || other != null
                && Products.SequenceEqual(other?.Products)
                && Customers.SequenceEqual(other?.Customers)
                && Name.Equals(other?.Name);
        }

        public override bool Equals(object store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));
            
            if (store.GetType() != GetType())
                return false;

            return ReferenceEquals(this, store) || Equals(store as Store);
        }

        public override string ToString()
        {
            return $"{Name}\n Products: {Products?.Length ?? 0}\n Customers: {Customers?.Length ?? 0}";
        }
    }
}