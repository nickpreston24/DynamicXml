namespace DynamicXml
{
    public interface ISerializeAction<in T>
    {
        IRead Extract(string text); //text To Pocos

        IWrite Serialize(T text); //Pocos to text
    }
}