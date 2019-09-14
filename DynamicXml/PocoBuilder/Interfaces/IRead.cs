using System.IO;
using System.Threading.Tasks;

namespace DynamicXml
{
    public interface IRead
    {
        void ToFile(string filePath);

        Task ToFileAsync(string filePath);

        void ToStream(Stream stream);

        Task ToStreamAsync(Stream stream);
    }
}