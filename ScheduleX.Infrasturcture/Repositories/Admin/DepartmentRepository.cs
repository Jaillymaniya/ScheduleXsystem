using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task AddAsync(Department department)
        {
            try
            {
                bool exists = await _context.Departments
                    .AnyAsync(x => x.DepartmentName == department.DepartmentName);

                if (exists)
                    throw new Exception("Department already exists");

                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding department", ex);
            }
        }

        public async Task UpdateAsync(Department department)
        {
            try
            {
                _context.Departments.Update(department);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating department", ex);
            }
        }

        public async Task ToggleStatusAsync(int id)
        {
            try
            {
                var dept = await _context.Departments.FindAsync(id);

                if (dept == null)
                    throw new Exception("Department not found");

                dept.IsActive = !dept.IsActive;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating status", ex);
            }
        }
    }
}