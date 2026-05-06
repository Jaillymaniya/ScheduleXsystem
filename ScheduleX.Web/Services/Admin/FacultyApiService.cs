using ScheduleX.Core.Entities;
using ScheduleX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Interfaces.Admin;


namespace ScheduleX.Web.Services.Admin
{
    public class FacultyApiService
    {
        private readonly IFacultyRepository _repo;
        private readonly AppDbContext _context;

        public FacultyApiService(IFacultyRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<List<Faculty>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<(bool success, string message)> CreateAsync(Faculty faculty)
        {
            try
            {
                await _repo.AddAsync(faculty);

                // 🔥 External Faculty auto insert
                if (faculty.IsExternal)
                {
                    _context.ExternalFacultyPermissions.Add(new ExternalFacultyPermission
                    {
                        FacultyId = faculty.FacultyId,
                        DepartmentId = faculty.DepartmentId,
                        IsActive = true
                    });

                    await _context.SaveChangesAsync();
                }

                return (true, "Faculty added successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<(bool success, string message)> UpdateAsync(Faculty faculty)
        {
            try
            {
                var existingFaculty = await _context.Faculties
                    .FirstOrDefaultAsync(x => x.FacultyId == faculty.FacultyId);

                if (existingFaculty == null)
                    return (false, "Faculty not found");

                // ✅ Update fields manually
                existingFaculty.FacultyName = faculty.FacultyName;
                existingFaculty.FacultyCode = faculty.FacultyCode;
                existingFaculty.Email = faculty.Email;
                existingFaculty.Phone = faculty.Phone;
                existingFaculty.DepartmentId = faculty.DepartmentId;
                existingFaculty.IsExternal = faculty.IsExternal;
                existingFaculty.MaxLecturesPerDay = faculty.MaxLecturesPerDay;

                var existingPermission = await _context.ExternalFacultyPermissions
                    .FirstOrDefaultAsync(x => x.FacultyId == faculty.FacultyId);

                // ✅ Handle external logic
                if (!faculty.IsExternal)
                {
                    if (existingPermission != null)
                    {
                        _context.ExternalFacultyPermissions.Remove(existingPermission);
                    }
                }
                else
                {
                    if (existingPermission == null)
                    {
                        _context.ExternalFacultyPermissions.Add(new ExternalFacultyPermission
                        {
                            FacultyId = faculty.FacultyId,
                            DepartmentId = faculty.DepartmentId,
                            IsActive = true
                        });
                    }
                }

                // ✅ SINGLE SAVE
                await _context.SaveChangesAsync();

                return (true, "Faculty updated successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<(bool success, string message)> DeleteAsync(int id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                return (true, "Faculty deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool success, string message)> ToggleStatusAsync(int id)
        {
            try
            {
                await _repo.ToggleStatusAsync(id);
                return (true, "Status updated");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> BulkUpload(List<Faculty> list)
        {
            try
            {
                return await _repo.BulkInsertAsync(list);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}