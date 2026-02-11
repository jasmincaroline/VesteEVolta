using VesteEVolta.Models;
using VesteEVolta.Repositories.Interfaces;

namespace VesteEVolta.Repositories;

public class UserRepository : IUserRepository
{
    private readonly VesteEVoltaContext _context;

    public UserRepository(VesteEVoltaContext context)
    {
        _context = context;
    }

    public List<TbUser> GetAll()
    {
        return _context.TbUsers.ToList();
    }

    public TbUser? GetById(Guid id)
    {
        return _context.TbUsers.Find(id);
    }
}
