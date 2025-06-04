using DLQService.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DLQService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderEventsController(QueueDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderEvent>>> GetAll(CancellationToken cancellationToken)
        {
            var events = await dbContext.OrderEvents.OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
            return Ok(events);
        }
    }
}