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
            //return await _context.Faculties
            //    .Include(x => x.Department)
            //    .ToListAsync();
            return await _context.Faculties
    .Include(x => x.Department)
    .Include(x => x.ExternalPermissions)
        .ThenInclude(p => p.Department)
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

        //public async Task ToggleStatusAsync(int id)
        //{
        //    var faculty = await _context.Faculties
        //        .FirstOrDefaultAsync(x => x.FacultyId == id);

        //    if (faculty == null)
        //        throw new Exception("Faculty not found");

        //    faculty.IsActive = !faculty.IsActive;

        //    // 🔥 If external faculty → also update permission table
        //    if (faculty.IsExternal)
        //    {
        //        var permissions = await _context.ExternalFacultyPermissions
        //            .Where(x => x.FacultyId == id)
        //            .ToListAsync();

        //        foreach (var p in permissions)
        //        {
        //            p.IsActive = faculty.IsActive;
        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //}
        public async Task ToggleStatusAsync(int id)
        {
            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(x => x.FacultyId == id);

            if (faculty == null)
                throw new Exception("Faculty not found");

            faculty.IsActive = !faculty.IsActive;

            // ALWAYS sync external permissions
            var permissions = await _context.ExternalFacultyPermissions
                .Where(x => x.FacultyId == id)
                .ToListAsync();

            foreach (var p in permissions)
            {
                p.IsActive = faculty.IsActive;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<(bool, string)> BulkInsertAsync(List<Faculty> list)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var validList = new List<Faculty>();

                foreach (var item in list)
                {
                    // ================= BASIC VALIDATION =================
                    if (string.IsNullOrWhiteSpace(item.FacultyName))
                        continue;

                    if (item.DepartmentId <= 0)
                        continue;

                    // ================= DUPLICATE CHECK =================
                    bool exists = await _context.Faculties.AnyAsync(x =>
                        (!string.IsNullOrEmpty(item.Email) && x.Email == item.Email) ||
                        (!string.IsNullOrEmpty(item.FacultyCode) && x.FacultyCode == item.FacultyCode)
                    );

                    if (exists)
                        continue;

                    validList.Add(item);
                }

                if (!validList.Any())
                    return (false, "No valid faculty records found");

                _context.Faculties.AddRange(validList);
                await _context.SaveChangesAsync();

                // ================= EXTERNAL FACULTY VALIDATION =================
                foreach (var faculty in validList)
                {
                    if (!faculty.IsExternal)
                        continue;

                    // MUST have allowed departments
                    if (string.IsNullOrWhiteSpace(faculty.AllowedDepartmentsCsv))
                        return (false, $"External faculty '{faculty.FacultyName}' must have allowed departments");

                    var departmentNames = faculty.AllowedDepartmentsCsv
                        .Split('|', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Distinct()
                        .ToList();

                    foreach (var deptName in departmentNames)
                    {
                        var department = await _context.Departments
                            .FirstOrDefaultAsync(x =>
                                x.DepartmentName.ToLower() == deptName.ToLower());

                        // ❌ invalid department → skip safely
                        if (department == null)
                            continue;

                        // prevent duplicate permission
                        bool alreadyExists = await _context.ExternalFacultyPermissions.AnyAsync(x =>
                            x.FacultyId == faculty.FacultyId &&
                            x.DepartmentId == department.DepartmentId);

                        if (!alreadyExists)
                        {
                            _context.ExternalFacultyPermissions.Add(new ExternalFacultyPermission
                            {
                                FacultyId = faculty.FacultyId,
                                DepartmentId = department.DepartmentId,
                                IsActive = true
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, $"{validList.Count} faculty inserted successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }
    }
}

