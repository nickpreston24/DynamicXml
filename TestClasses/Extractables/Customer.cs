namespace TestClasses
{        
    public partial class Customer
    {
        private string city;
        private uint age;
        private string state;

        public string Name { get; set; }
        public string City { get => city; set => city = value; }
        public uint Age { get => age; set => age = value; }
        public string State { get => state; set => state = value; }
    }
}
