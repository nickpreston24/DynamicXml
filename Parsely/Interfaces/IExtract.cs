namespace Parsely.Builders
{
    public interface IExtract<in T>
    {
        IRead Extract(string text); //text To Pocos

    }
}