using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories
{
    public class AcademicYearRepository : IAcademicYearRepository
    {
        private readonly AppDbContext _context;

        public AcademicYearRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AcademicYear>> GetAllAsync()
            => await _context.AcademicYears.ToListAsync();

        public async Task AddAsync(AcademicYear year)
        {
            bool exists = await _context.AcademicYears
                .AnyAsync(x => x.YearName == year.YearName);

            if (exists)
                throw new Exception("Academic year already exists");

            _context.AcademicYears.Add(year);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AcademicYear year)
        {
            _context.AcademicYears.Update(year);
            await _context.SaveChangesAsync();
        }

        public async Task ToggleStatusAsync(int id)
        {
            var item = await _context.AcademicYears.FindAsync(id);

            if (item == null)
                throw new Exception("Academic year not found");

            item.IsActive = !item.IsActive;
            await _context.SaveChangesAsync();
        }
    }
}