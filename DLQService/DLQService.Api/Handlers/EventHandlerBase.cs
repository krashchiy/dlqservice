using System.Text.Json;
using System.Text.Json.Serialization;
using Aapc.Eventing.Abstractions.Handlers;
using Aapc.Eventing.Abstractions.Messages;

namespace DLQService.Api.Handlers
{
    public abstract class EventHandlerBase<TEvent>(ILogger logger) : IMessageHandler
        where TEvent : class
    {
        public abstract string Source { get; }
        public abstract string MessageType { get; }

        public async Task<MessageProcessingResult> HandleAsync(
            string message,
            CancellationToken cancellationToken
        )
        {
            try
            {
                logger.LogDebug("Received message: {Message}", message);

                var eventData = DeserializeEvent<TEvent>(message);
                if (eventData is null)
                {
                    logger.LogError("Failed to parse {EventType} data", typeof(TEvent).Name);
                    return MessageProcessingResult.Failed($"Invalid {typeof(TEvent).Name} format");
                }

                return await ProcessEventAsync(eventData, cancellationToken);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to parse message as JSON");
                return MessageProcessingResult.Failed("Invalid JSON format");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing event");
                return MessageProcessingResult.Failed(ex.Message);
            }
        }

        protected virtual TEvent? DeserializeEvent<T>(string message)
            where T : class
        {
            return JsonSerializer.Deserialize<T>(message, JsonSerializerOptionsProvider.s_options)
                as TEvent;
        }

        protected abstract Task<MessageProcessingResult> ProcessEventAsync(
            TEvent eventData,
            CancellationToken cancellationToken
        );
    }

    public static class JsonSerializerOptionsProvider
    {
        public static readonly JsonSerializerOptions s_options = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };
    }
}
