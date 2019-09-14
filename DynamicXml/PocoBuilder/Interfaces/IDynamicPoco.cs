namespace DynamicXml
{
    public interface IDynamicPoco<in T>
         where T : class
    {
        ISerializeAction<T> OnInstance(T instance);
    }
}