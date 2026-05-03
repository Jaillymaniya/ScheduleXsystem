using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subject>> GetAllAsync(int courseId)
        {
            try
            {
                return await _context.Subjects
                    .Where(x => x.CourseId == courseId)
                    .Include(x => x.Course)
                    .ThenInclude(c => c.Department)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Course>> GetCoursesAsync()
        {
            try
            {
                return await _context.Courses.Where(x => x.IsActive).ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Course>> GetCoursesForCoordinatorAsync(int coordinatorId)
        {
            try
            {
                return await _context.TTCoordinatorCourses
                    .Where(x => x.UserId == coordinatorId)
                    .Select(x => x.Course)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            try
            {
                return await _context.Departments.Where(x => x.IsActive).ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Course>> GetCoursesByDepartmentAsync(int departmentId)
        {
            try
            {
                return await _context.Courses
                    .Where(x => x.DepartmentId == departmentId && x.IsActive)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        public async Task<(bool, string)> AddAsync(Subject subject)
        {
            try
            {
                bool exists = await _context.Subjects
                    .AnyAsync(x => x.SubjectName == subject.SubjectName && x.CourseId == subject.CourseId);

                if (exists)
                    return (false, "Subject already exists");

                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();

                return (true, "Subject added successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> UpdateAsync(Subject subject)
        {
            try
            {
                _context.Subjects.Update(subject);
                await _context.SaveChangesAsync();

                return (true, "Updated successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> SoftDeleteAsync(int id)
        {
            try
            {
                var item = await _context.Subjects.FindAsync(id);

                if (item == null)
                    return (false, "Not found");

                item.IsActive = false;
                await _context.SaveChangesAsync();

                return (true, "Deleted");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> ActivateAsync(int id)
        {
            try
            {
                var item = await _context.Subjects.FindAsync(id);

                if (item == null)
                    return (false, "Not found");

                item.IsActive = true;
                await _context.SaveChangesAsync();

                return (true, "Activated");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<bool> IsSubjectCodeExists(string code, int? subjectId)
        {
            try
            {
                return await _context.Subjects
                    .AnyAsync(x => x.SubjectCode == code && x.SubjectId != subjectId);
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            try
            {
                return await _context.Subjects
                    .Include(s => s.Course)
                    .ThenInclude(c => c.Department)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }
        public async Task<(bool, string)> BulkInsertAsync(List<Subject> list, int courseId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var validList = new List<Subject>();

                foreach (var item in list)
                {
                    item.CourseId = courseId;

                    // 🔥 DUPLICATE CHECK (DB)
                    bool exists = await _context.Subjects
                        .AnyAsync(x => x.SubjectCode == item.SubjectCode && x.CourseId == courseId);

                    if (exists)
                        continue; // skip duplicate

                    // 🔥 BASIC VALIDATION
                    if (string.IsNullOrWhiteSpace(item.SubjectName))
                        continue;

                    validList.Add(item);
                }

                if (!validList.Any())
                    return (false, "No valid records to insert");

                _context.Subjects.AddRange(validList);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, $"{validList.Count} subjects inserted successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }


    }
}