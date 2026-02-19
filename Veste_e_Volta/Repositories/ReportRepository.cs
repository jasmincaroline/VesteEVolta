using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;

namespace VesteEVolta.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly PostgresContext _context;

        public ReportRepository(PostgresContext context)
        {
            _context = context;
        }

        public async Task<TbReport> CreateAsync(TbReport report)
        {
            _context.TbReports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public Task<List<TbReport>> GetAllAsync()
        {
            return _context.TbReports.ToListAsync();
        }

        public Task<TbReport?> GetByIdAsync(Guid id)
        {
            return _context.TbReports
                .FirstOrDefaultAsync(r => r.ReportId == id);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
