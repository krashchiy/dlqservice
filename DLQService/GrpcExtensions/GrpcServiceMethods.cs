using DlqService.Grpc;

namespace GrpcExtensions
{
    public class GrpcServiceMethods(Greeter.GreeterClient grpcClient) : IGrpcServiceMethods
    {
        public async Task<string> SayHelloAsync(string name, CancellationToken cancellationToken)
        {
            var result = await grpcClient.SayHelloAsync(new HelloRequest { Name = name }, cancellationToken: cancellationToken);
            return result.Message;
        }
    }
}
