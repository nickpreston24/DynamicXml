using System;

namespace Shared.Classes
{
    public sealed class Product : IEquatable<Product>
    {
        public string Name { get; set; }
        public decimal Cost { get; set; }

        public bool Equals(Product other)
        {
            return other != null
                && Name.ToUpper() == other.Name.ToUpper()
                && Cost == other.Cost;
        }
    }
}