using VesteEVolta.DTO;
using VesteEVolta.Repositories.Interfaces;
using VesteEVolta.Services.Interfaces;

namespace VesteEVolta.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public UserDTO GetUser(Guid id)
    {
        var user = _repository.GetById(id);

        if (user == null)
            throw new Exception("Usuário não existe.");

        return new UserDTO
        {
            Name = user.Name,
            Email = user.Email
        };
    }
}
