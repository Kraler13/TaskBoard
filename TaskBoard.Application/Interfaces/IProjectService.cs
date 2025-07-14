using TaskBoard.Application.Dtos;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Interfaces
{
    public interface IProjectService
    {
        IEnumerable<Project> GetAll();
        Project? Get(Guid id);
        Project Create(CreateProjectDto dto);
        void Update(Guid id, CreateProjectDto dto);
        void Delete(Guid id);
        ProjectFullDto? GetFull(Guid id);
    }
}
