using System.IO;
using System.Threading.Tasks;

namespace Parsely.Builders
{
    public interface IRead
    {
        void ToFile(string filePath);

        Task ToFileAsync(string filePath);

        void ToStream(Stream stream);

        Task ToStreamAsync(Stream stream);
    }
}