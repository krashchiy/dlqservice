using Microsoft.EntityFrameworkCore;

namespace DLQService.Api.Data
{
    // Data/OrderEvent.cs
    public class QueueDbContext(DbContextOptions<QueueDbContext> options) : DbContext(options)
    {
        public DbSet<OrderEvent> OrderEvents { get; set; }
    }

}
