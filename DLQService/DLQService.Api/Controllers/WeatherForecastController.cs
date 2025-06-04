using GrpcExtensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DLQService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private readonly ILogger<WeatherForecastController> _logger = logger;

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get([FromServices] IGrpcServiceMethods grpcService, CancellationToken cancellationToken)
        {
            //string result = await grpcService.SayHelloAsync("gRPC Call", cancellationToken);

            var tasks =  Enumerable.Range(1, 5).Select(async index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = await grpcService.SayHelloAsync($"gRPC {Summaries[Random.Shared.Next(Summaries.Length)]}", cancellationToken)
            })
            .ToArray();

            var details = await Task.WhenAll(tasks);
            return details;
        }
    }
}
