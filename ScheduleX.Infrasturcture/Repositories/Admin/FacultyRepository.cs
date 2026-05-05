using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Core.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;

namespace ScheduleX.Infrastructure.Repositories.Admin
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly AppDbContext _context;

        public FacultyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Faculty>> GetAllAsync()
        {
            return await _context.Faculties
                .Include(x => x.Department)
                .ToListAsync();
        }

        public async Task AddAsync(Faculty faculty)
        {
            _context.Faculties.Add(faculty);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Faculty faculty)
        {
            _context.Faculties.Update(faculty);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);

            if (faculty == null)
                throw new Exception("Faculty not found");

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();
        }

        public async Task ToggleStatusAsync(int id)
        {
            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(x => x.FacultyId == id);

            if (faculty == null)
                throw new Exception("Faculty not found");

            faculty.IsActive = !faculty.IsActive;

            // 🔥 If external faculty → also update permission table
            if (faculty.IsExternal)
            {
                var permissions = await _context.ExternalFacultyPermissions
                    .Where(x => x.FacultyId == id)
                    .ToListAsync();

                foreach (var p in permissions)
                {
                    p.IsActive = faculty.IsActive;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

