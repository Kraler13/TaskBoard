using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Dtos;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColumnsController : ControllerBase
    {
        private readonly TaskBoardDbContext _dbContext;

        public ColumnsController(TaskBoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public ActionResult Create([FromBody] CreateColumnDto dto)
        {
            var project = _dbContext.Projects.Find(dto.ProjectId);
            if (project == null)
                return NotFound();

            var maxOrder = _dbContext.Columns
    .Where(c => c.ProjectId == dto.ProjectId)
    .Select(c => (int?)c.Order)
    .Max() ?? -1;

            var column = new Column
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Order = maxOrder + 1,
                ProjectId = dto.ProjectId,
                Project = project
            };

            _dbContext.Columns.Add(column);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = column.Id }, column);
        }

        [HttpGet("/api/projects/{projectId}/columns")]
        public ActionResult<IEnumerable<Column>> GetByProject(Guid projectId)
        {
            var columns = _dbContext.Columns
                .Where(c => c.ProjectId == projectId)
                .OrderBy(c => c.Order)
                .ToList();

            return Ok(columns);
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid id, [FromBody] CreateColumnDto dto)
        {
            var column = _dbContext.Columns.Find(id);
            if (column == null)
                return NotFound();

            column.Name = dto.Name;
            column.Order = dto.Order;
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var column = _dbContext.Columns
                .Include(c => c.Tasks)
                .FirstOrDefault(c => c.Id == id);

            if (column == null)
                return NotFound();

            _dbContext.Columns.Remove(column);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpGet("{id}")]
        public ActionResult<Column> Get(Guid id)
        {
            var column = _dbContext.Columns.FirstOrDefault(c => c.Id == id);
            if (column == null)
                return NotFound();

            return Ok(column);
        }
    }
}