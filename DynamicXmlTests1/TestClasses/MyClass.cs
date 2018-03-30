namespace DynamicXml.Extensions.Tests
{
    public class MyClass<T>
    {
        public object MyGenericTypeDefinitionInstance { get; set; } = typeof(T).GetDefault();
    }

}