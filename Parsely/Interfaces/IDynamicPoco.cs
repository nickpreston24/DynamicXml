namespace Parsely.Builders
{
    public interface IDynamicPoco<in T>
         where T : class
    {
        IExtract<T> OnInstance(T instance);
    }
}