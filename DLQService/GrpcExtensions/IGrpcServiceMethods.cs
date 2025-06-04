
namespace GrpcExtensions
{
    public interface IGrpcServiceMethods
    {
        Task<string> SayHelloAsync(string name, CancellationToken cancellationToken);
    }
}
