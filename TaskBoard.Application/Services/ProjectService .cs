using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.Interfaces;
using TaskBoard.Domain.Entities;
using TaskBoard.Application.Dtos;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly TaskBoardDbContext _db;

        public ProjectService(TaskBoardDbContext db) => _db = db;

        /* READs */
        public IEnumerable<Project> GetAll() => _db.Projects.ToList();

        public Project? Get(Guid id) => _db.Projects.Find(id);

        /* CREATE */
        public Project Create(CreateProjectDto dto)
        {
            var owner = _db.Users.Find(dto.OwnerId) ??
                        throw new KeyNotFoundException("Owner not found");

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                OwnerId = dto.OwnerId,
                Owner = owner
            };

            _db.Projects.Add(project);
            _db.SaveChanges();
            return project;
        }

        /* UPDATE */
        public void Update(Guid id, CreateProjectDto dto)
        {
            var project = _db.Projects.Find(id) ??
                          throw new KeyNotFoundException("Project not found");

            project.Name = dto.Name;
            _db.SaveChanges();
        }

        /* DELETE */
        public void Delete(Guid id)
        {
            var project = _db.Projects
                             .Include(p => p.Columns)
                             .ThenInclude(c => c.Tasks)
                             .FirstOrDefault(p => p.Id == id) ??
                          throw new KeyNotFoundException("Project not found");

            _db.Projects.Remove(project);
            _db.SaveChanges();
        }

        /* FULL TREE (DTO-read) */
        public ProjectFullDto? GetFull(Guid id) =>
            _db.Projects.Where(p => p.Id == id)
               .Select(p => new ProjectFullDto
               {
                   Id = p.Id,
                   Name = p.Name,
                   Columns = p.Columns.OrderBy(c => c.Order)
                       .Select(c => new ColumnDto
                       {
                           Id = c.Id,
                           Name = c.Name,
                           Order = c.Order,
                           Tasks = c.Tasks.OrderBy(t => t.Order)
                               .Select(t => new TaskReadDto
                               {
                                   Id = t.Id,
                                   Title = t.Title,
                                   Description = t.Description,
                                   Order = t.Order
                               })
                               .ToList()
                       })
                       .ToList()
               })
               .FirstOrDefault();
    }
}
