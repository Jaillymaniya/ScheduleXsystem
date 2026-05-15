using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TT
{
    public class SubjectSemesterRepository : ISubjectSemesterRepository
    {
        private readonly AppDbContext _context;

        public SubjectSemesterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubjectSemester>> GetAllAsync(
     int academicYearId,
     int courseId,
     int academicTermId)
        {
            try
            {
                var term = await _context.AcademicTerms
                    .FirstOrDefaultAsync(x =>
                        x.AcademicTermId == academicTermId);

                if (term == null)
                    return new();

                return await _context.SubjectSemesters
                    .Include(x => x.Subject)
                    .Include(x => x.Semester)
                    .ThenInclude(x => x.Course)
                    .Include(x => x.AcademicYear)
                    .Where(x =>
                        x.AcademicYearId == academicYearId &&
                        x.Semester.CourseId == courseId &&
                        x.Semester.SemesterPattern == term.SemesterPattern)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Semester>> GetSemestersAsync(
     int courseId,
     int academicTermId)
        {
            try
            {
                var term = await _context.AcademicTerms
                    .FirstOrDefaultAsync(x =>
                        x.AcademicTermId == academicTermId);

                if (term == null)
                    return new();

                return await _context.Semesters
                    .Where(x =>
                        x.CourseId == courseId &&
                        x.IsActive &&
                        x.SemesterPattern == term.SemesterPattern)
                    .OrderBy(x => x.SemesterNo)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<Subject>> GetSubjectsAsync(int courseId)
        {
            try
            {
                return await _context.Subjects
                    .Where(x =>
                        x.CourseId == courseId &&
                        x.IsActive)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        public async Task<(bool, string)> AddAsync(
            SubjectSemester model)
        {
            try
            {
                bool exists =
                    await _context.SubjectSemesters
                    .AnyAsync(x =>
                        x.AcademicYearId == model.AcademicYearId &&
                        x.SubjectId == model.SubjectId &&
                        x.SemesterId == model.SemesterId);

                if (exists)
                    return (false, "Already exists");

                _context.SubjectSemesters.Add(model);

                await _context.SaveChangesAsync();

                return (true, "Added successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> UpdateAsync(
            SubjectSemester model)
        {
            try
            {
                _context.SubjectSemesters.Update(model);

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
                var item =
                    await _context.SubjectSemesters.FindAsync(id);

                if (item == null)
                    return (false, "Not found");

                item.IsActive = false;

                await _context.SaveChangesAsync();

                return (true, "Deactivated");
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
                var item =
                    await _context.SubjectSemesters.FindAsync(id);

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

        public async Task<(bool, string)> BulkInsertAsync(
            List<SubjectSemester> list)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var validList = new List<SubjectSemester>();

                foreach (var item in list)
                {
                    bool exists =
                        await _context.SubjectSemesters
                        .AnyAsync(x =>
                            x.AcademicYearId == item.AcademicYearId &&
                            x.SubjectId == item.SubjectId &&
                            x.SemesterId == item.SemesterId);

                    if (exists)
                        continue;

                    validList.Add(item);
                }

                if (!validList.Any())
                    return (false, "No valid records");

                _context.SubjectSemesters.AddRange(validList);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return (true,
                    $"{validList.Count} records inserted");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return (false, ex.Message);
            }
        }

        public async Task<List<Course>>
            GetCoursesForCoordinatorAsync(int userId)
        {
            try
            {
                return await _context.TTCoordinatorCourses
                    .Where(x => x.UserId == userId)
                    .Select(x => x.Course)
                    .Where(x => x.IsActive)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }
    }
}