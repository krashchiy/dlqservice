using System.ComponentModel.DataAnnotations;
using DLQService.Api.Handlers;

namespace DLQService.Api.Data
{
    public class OrderEvent: OrderCreatedEvent
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt => DateTime.UtcNow;
    }
}
