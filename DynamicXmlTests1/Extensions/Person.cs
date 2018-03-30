namespace DynamicXml.Extensions.Tests
{
    public class Person : IPerson
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public override string ToString() => $"Name: {Name}\nAge: {Age}\n";
    }
}