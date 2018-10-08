namespace DynamicXml.Extensions.Tests
{
    public class Wrapper<T>
    {
        public object MyGenericTypeDefinitionInstance { get; set; } = typeof(T).GetDefault();
    }

}