using VesteEVolta.Models;

namespace VesteEVolta.Repositories
{
    public interface ICategoryRepositories
    {
        List<TbCategory> GetAll();
        TbCategory? GetById(Guid id);
        TbCategory? GetByName(string name);

        void Add(TbCategory category);
        void Update(TbCategory category);
        void Delete(TbCategory category);

        void SaveChanges();
    }
}