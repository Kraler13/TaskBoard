using TaskBoard.Application.Dtos;

namespace TaskBoard.Application.Interfaces
{
    public interface IAuthService
    {
        Guid Register(RegisterDto dto);
        string Login(LoginDto dto);
    }
}
