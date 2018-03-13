namespace TestClasses
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
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
