using VesteEVolta.Models;

namespace VesteEVolta.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<List<TbCategory>> GetAllAsync();
}
