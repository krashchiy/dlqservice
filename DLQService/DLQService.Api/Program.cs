using GrpcExtensions;
using Microsoft.EntityFrameworkCore;
using Aapc.Eventing.Abstractions.Producer;
using Aapc.Eventing.Aws;
using Aapc.Eventing.Aws.DependencyInjection;
using DLQService.Api.Data;
using DLQService.Api.Handlers;

namespace DLQService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            EnsureDatabaseMigrated(app);

            Configure(app);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<QueueDbContext>(options =>
                options.UseSqlite("Data Source=shoppingmessages.db"));
            services.AddGrpcService();
            AddEventingServices(services, configuration);
        }

        private static void EnsureDatabaseMigrated(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
            db.Database.Migrate();
        }

        private static void Configure(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        private static void AddEventingServices(IServiceCollection services, IConfiguration configuration)
        {
            const string serviceName = "DLQServiceConfig";
            MessagingConfiguration messagingConfig =
                configuration
                    .GetSection($"{serviceName}:AWS:MessagingConfiguration")
                    .Get<MessagingConfiguration>() ?? new MessagingConfiguration();

            if (messagingConfig.IsEnabled)
            {
                services.AddAapcAwsEventing(
                    serviceName,
                    configuration
                );
                services.AddScoped<OrderEventHandler>();
                services.AddHostedService<EventConsumerService>();
            }
            else
            {
                services.AddSingleton<IMessageProducer, DummyMessageProducer>();
            }
        }
    }
}