using VesteEVolta.DTO;

namespace VesteEVolta.Services.Interfaces;

public interface IUserService
{
    UserDTO GetUser(Guid id);
}
