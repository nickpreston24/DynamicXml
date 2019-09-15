using System;

namespace Shared.Classes
{
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