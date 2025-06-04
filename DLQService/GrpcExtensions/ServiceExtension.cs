using DlqService.Grpc;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcExtensions
{
    public static class ServiceExtension
    {
        public static void AddGrpcService(this IServiceCollection services)
        {
            services.AddGrpcClient<Greeter.GreeterClient>(client =>
            {
                client.Address = new Uri("https://localhost:7090");
            });
            services.AddScoped<IGrpcServiceMethods, GrpcServiceMethods>();
        }
    }
}
