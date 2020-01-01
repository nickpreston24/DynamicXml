using System.Threading.Tasks;

namespace Parsely.Builders
{
    public interface IRunTasks<T>
    {
        Task<T> RunAsync();
    }

    public interface IRunTasks
    {
        Task RunAsync();
    }
}