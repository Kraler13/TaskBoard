using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Application.Dtos;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskBoardDbContext _dbContext;

        public TasksController(TaskBoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public ActionResult<TaskItem> Create([FromBody] CreateTaskDto dto)
        {
            var column = _dbContext.Columns.Find(dto.ColumnId);
            if (column == null)
                return NotFound($"Column with ID {dto.ColumnId} not found");

            var maxOrder = _dbContext.Tasks
    .Where(t => t.ColumnId == dto.ColumnId)
    .Select(t => t.Order)
    .DefaultIfEmpty(-1)
    .Max();

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Order = maxOrder + 1,
                ColumnId = dto.ColumnId,
                Column = column
            };

            _dbContext.Tasks.Add(task);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        [HttpGet("/api/columns/{columnId}/tasks")]
        public ActionResult<IEnumerable<TaskItem>> GetByColumn(Guid columnId)
        {
            var tasks = _dbContext.Tasks
                .Where(t => t.ColumnId == columnId)
                .OrderBy(t => t.Order)
                .ToList();

            return Ok(tasks);
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid id, [FromBody] CreateTaskDto dto)
        {
            var task = _dbContext.Tasks.Find(id);
            if (task == null)
                return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.ColumnId = dto.ColumnId;
            task.Order = dto.Order;
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var task = _dbContext.Tasks
                .FirstOrDefault(c => c.Id == id);

            if (task == null)
                return NotFound();

            _dbContext.Tasks.Remove(task);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/move")]
        public ActionResult Move(Guid id, [FromBody] MoveTaskDto dto)
        {
            var task = _dbContext.Tasks.Find(id);
            if (task == null)
                return NotFound();

            var targetColumn = _dbContext.Columns
                .Include(c => c.Tasks)
                .FirstOrDefault(c => c.Id == dto.ColumnId);

            if (targetColumn == null)
                return NotFound("Target column not found");

            if (task.ColumnId != dto.ColumnId)
            {
                var oldColumn = _dbContext.Columns
                    .Include(c => c.Tasks)
                    .FirstOrDefault(c => c.Id == task.ColumnId);

                if (oldColumn != null)
                {
                    foreach (var t in oldColumn.Tasks.Where(t => t.Order > task.Order))
                        t.Order--;
                }

                task.ColumnId = dto.ColumnId;
            }

            foreach (var t in targetColumn.Tasks.Where(t => t.Order >= dto.Order && t.Id != task.Id))
            {
                t.Order++;
            }

            task.Order = dto.Order;

            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpGet("{id}")]
        public ActionResult<TaskItem> Get(Guid id)
        {
            var task = _dbContext.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
                return NotFound();

            return Ok(task);
        }
    }
}