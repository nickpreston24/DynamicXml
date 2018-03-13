using System.Collections.Generic;

namespace TestClasses
{
    public class Store
    {
        //Todo:
        //public IEnumerable<Employee> Employees { get; set; }

        //Working:

        //public List<Product> Products { get; set; }
        public IList<Product> Products { get; set; }
        public Customer[] Customers { get; set; }
    }
}
