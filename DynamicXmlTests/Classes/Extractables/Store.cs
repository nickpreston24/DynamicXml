using System;
using System.Linq;

namespace DynamicXmlTests
{
    internal class Store : IEquatable<Store>
    {
        public string Name { get; set; }
        public Product[] Products { get; set; }
        public Customer[] Customers { get; set; }

        public bool Equals(Store other)
        {
            return other != null
                && Products.SequenceEqual(other.Products)
                && Customers.SequenceEqual(other.Customers)
                /*&& Name.Equals(other.Name)*/;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals(obj as Store);
        }

        public override string ToString()
        {
            return $"{Name}\n Products: {Products.Length}\n Customers: {Customers.Length}";
        }
    }
}