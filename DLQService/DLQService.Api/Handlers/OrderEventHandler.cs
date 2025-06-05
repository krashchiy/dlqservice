using Aapc.Eventing.Abstractions.Messages;
using DLQService.Api.Data;

namespace DLQService.Api.Handlers
{
    /// <summary>
    /// Order event handler for messaging
    /// </summary>
    /// <param name="logger"></param>
    public class OrderEventHandler(ILogger<OrderEventHandler> logger, QueueDbContext dbContext) : EventHandlerBase<OrderCreatedEvent>(logger)
    {
        public override string Source => "https://shopping.aapc.com";
        public override string MessageType => "Aapc.Shopping.ApiModel.Events.OrderCreatedEvent";

        protected override async Task<MessageProcessingResult> ProcessEventAsync(
            OrderCreatedEvent eventData,
            CancellationToken cancellationToken
        )
        {
            logger.LogInformation(
                "Received order OrderNumber: {OrderNumber} InvoiceId: {InvoiceId}",
                eventData.OrderNumber,
            eventData.InvoiceId
            );

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


            logger.LogInformation(
                "OrderNumber: {OrderNumber} successfully processed",
                eventData.OrderNumber
            );
            return await Task.FromResult(MessageProcessingResult.Succeeded());
        }
    }

    public class OrderCreatedEvent
    {
        public Guid InvoiceId { get; set; }
        public int OrderNumber { get; set; }
        public int CustomerId { get; set; }
        public bool IsPaid { get; set; }
    }
}
