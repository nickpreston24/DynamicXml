namespace Parsely.JsonBuilder
{
    public sealed class JsonBuilder<T> where T : class
    {
        // 1. Build POCO from JSON (same way as DynamicXMl)
        // 2. Build JSON from class instance (serialize)

        public string ToJson(string text, TextFormat inputFormat)
        {
            return text;
        }
    }

    //public interface IConvertToJson
    //{
    //    string ToJson(string text, TextFormat textFormat);
    //}

    //public interface IConvertToXml
    //{
    //    string ToXml(string text, TextFormat textFormat);
    //}
}