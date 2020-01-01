namespace Parsely.Builders
{
    public interface IExtract<out T>
    {
        T Extract(string text); //text To Pocos
    }
}