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

    internal class Product : IEquatable<Product>
    {
        public string Name { get; set; }
        public decimal Cost { get; set; }

        public bool Equals(Product other)
        {
            return other != null
                && Name == other.Name
                && Cost == other.Cost;
        }
    }

    internal class Customer : IEquatable<Customer>
    {
        public string Name { get; set; }

        public string City { get; set; }

        public byte Age { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public bool Equals(Customer other)
        {
            return other != null
                && string.Equals(Name, other.Name)
                && string.Equals(State, other.State)
                && string.Equals(City, other.City)
                && string.Equals(Country, other.Country)
                && Age == other.Age;
        }
    }
}