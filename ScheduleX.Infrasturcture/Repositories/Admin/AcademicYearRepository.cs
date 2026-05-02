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

            // 🔥 deactivate all existing years
            var activeYears = await _context.AcademicYears
                .Where(x => x.IsActive)
                .ToListAsync();

            foreach (var item in activeYears)
            {
                item.IsActive = false;
            }

            // new year becomes active
            year.IsActive = true;

            _context.AcademicYears.Add(year);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AcademicYear year)
        {
            _context.AcademicYears.Update(year);
            await _context.SaveChangesAsync();
        }

        
    }
}