using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Dtos;
using TaskBoard.Infrastructure.Persistence;
using static TaskBoard.Program;
namespace TaskBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProjectsController : ControllerBase
    {

        private readonly TaskBoardDbContext _dbContext;

        public ProjectsController(TaskBoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Project>> GetAll()
        {
            var projects = _dbContext.Projects.ToList();
            return Ok(projects);
        }

        [HttpPost]
        public ActionResult<Project> Create([FromBody] CreateProjectDto dto)
        {
            var owner = _dbContext.Users.Find(dto.OwnerId);
            if (owner == null)
                return NotFound($"User with ID {dto.OwnerId} not found");

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                OwnerId = dto.OwnerId,
                Owner = owner
            };

            _dbContext.Projects.Add(project);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetAll), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid id, [FromBody] CreateProjectDto dto)
        {
            var project = _dbContext.Projects.Find(id);
            if (project == null)
                return NotFound();

            project.Name = dto.Name;
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var project = _dbContext.Projects
                .Where(p => p.Id == id)
                .Include(p => p.Columns)
                    .ThenInclude(c => c.Tasks)
                .FirstOrDefault();

            if (project == null)
                return NotFound();

            _dbContext.Projects.Remove(project);
            _dbContext.SaveChanges();

            return NoContent(); // 204
        }

        [HttpGet("{id}/full")]
        public ActionResult GetFullProject(Guid id)
        {
            var project = _dbContext.Projects
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    Columns = p.Columns
                        .OrderBy(c => c.Order)
                        .Select(c => new
                        {
                            c.Id,
                            c.Name,
                            c.Order,
                            Tasks = c.Tasks
                                .OrderBy(t => t.Order)
                                .Select(t => new
                                {
                                    t.Id,
                                    t.Title,
                                    t.Description,
                                    t.Order
                                }).ToList()
                        }).ToList()
                })
                .FirstOrDefault();

            if (project == null)
                return NotFound();

            return Ok(project);
        }
    }
}