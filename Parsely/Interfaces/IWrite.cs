using System.IO;
using System.Threading.Tasks;

namespace Parsely.Builders
{
    public interface IWrite
    {
        IRead ToFile(string filePath);

        // Task ToFileAsync(string filePath);

        IRead ToStream(Stream stream);

        // Task ToStreamAsync(Stream stream);
    }
}