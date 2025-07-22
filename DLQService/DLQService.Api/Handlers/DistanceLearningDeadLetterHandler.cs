using Aapc.Eventing.Abstractions.Handlers;
using Aapc.Eventing.Abstractions.Messages;
using Aapc.Shopping.ApiModel.Events;
using DLQService.Api.Data;
using OrderEvent = DLQService.Api.Data.OrderEvent;

namespace DLQService.Api.Handlers
{
    /// <summary>
    /// Order event handler for messaging
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    public class DistanceLearningDeadLetterHandler(ILogger<DistanceLearningDeadLetterHandler> logger, QueueDbContext dbContext) : EventHandlerBase<OrderCreatedEvent>(logger)
    {
        public override string Source => "https://shopping.aapc.com";
        public override string MessageType => typeof(OrderCreatedEvent).FullName;

        protected override async Task<MessageProcessingResult> ProcessEventAsync(
            OrderCreatedEvent eventData,
            CancellationToken cancellationToken
        )
        {
            logger.LogInformation("DistanceLearning DLQ Received OrderNumber: {OrderNumber} InvoiceId: {InvoiceId}", eventData.OrderNumber, eventData.InvoiceId);

            //use QueueDbContext to store the eventData in database
            var orderEntity = new OrderEvent
            {
                InvoiceId = eventData.InvoiceId,
                OrderNumber = eventData.OrderNumber,
                CustomerId = eventData.CustomerId,
                IsPaid = eventData.IsPaid,
                CreatedAt = DateTime.UtcNow
            };

            // Store in database
            dbContext.OrderEvents.Add(orderEntity);
            await dbContext.SaveChangesAsync(cancellationToken);


            logger.LogInformation("DistanceLearning DLQ OrderNumber: {OrderNumber} successfully processed", eventData.OrderNumber);
            return await Task.FromResult(MessageProcessingResult.Succeeded());
        }
    }

}
