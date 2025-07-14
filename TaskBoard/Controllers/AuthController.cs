using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Dtos;
using TaskBoard.Application.Interfaces;

namespace TaskBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterDto dto)
        {
            var id = _auth.Register(dto);
            return Created($"api/users/{id}", new { id });
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto dto)
        {
            var token = _auth.Login(dto);
            return Ok(new { token });
        }
    }
}
