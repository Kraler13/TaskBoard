using Microsoft.AspNetCore.Mvc;
using TaskBoard.Infrastructure.Persistence;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly TaskBoardDbContext _dbContext;

        public UsersController(TaskBoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(_dbContext.Users.ToList());
        }

        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] User user)
        {
            user.Id = Guid.NewGuid();
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
    }
}
