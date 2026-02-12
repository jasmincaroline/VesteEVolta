using VesteEVolta.Models;

namespace VesteEVolta.Repositories.Interfaces;
public interface IUserRepository
{
    List<TbUser> GetAll();
    TbUser? GetById(Guid id);
}
