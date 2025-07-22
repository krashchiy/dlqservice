using System.ComponentModel.DataAnnotations;
using Aapc.Shopping.ApiModel.Events;
using DLQService.Api.Handlers;

namespace DLQService.Api.Data
{
    public class OrderEvent: OrderCreatedEvent
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
