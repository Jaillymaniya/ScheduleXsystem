using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TT
{
    public class SubjectFacultyRepository
        : ISubjectFacultyRepository
    {
        private readonly AppDbContext _context;

        public SubjectFacultyRepository(
            AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // SEMESTERS
        // =====================================================

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
                        x.SemesterPattern == term.SemesterPattern)
                    .OrderByDescending(x => x.SemesterNo)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // DIVISIONS
        // =====================================================

        public async Task<List<Division>> GetDivisionsAsync(
     int academicYearId,
     int courseId,
     int semesterId)
        {
            try
            {
                return await _context.Divisions
                    .Where(x =>
                        x.AcademicYearId == academicYearId &&
                        x.CourseId == courseId &&
                        x.SemesterId == semesterId &&
                        x.IsActive)
                    .OrderBy(x => x.DivisionName)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // SUBJECT SEMESTERS
        // =====================================================

        public async Task<List<SubjectSemester>>
            GetSubjectSemestersAsync(
                int academicYearId,
                int semesterId)
        {
            try
            {
                return await _context.SubjectSemesters
                    .Include(x => x.Subject)

                    .Where(x =>
                        x.AcademicYearId ==
                        academicYearId &&

                        x.SemesterId ==
                        semesterId)

                    .OrderBy(x =>
                        x.Subject.SubjectName)

                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // FACULTIES
        // =====================================================

        public async Task<List<Faculty>> GetFacultiesAsync(
            int courseId)
        {
            try
            {
                var departmentId =
                    await _context.Courses
                    .Where(x => x.CourseId == courseId)
                    .Select(x => x.DepartmentId)
                    .FirstOrDefaultAsync();

                return await _context.Faculties
                    .Where(x =>
                        x.IsActive &&

                        (
                            x.DepartmentId ==
                            departmentId

                            ||

                            x.ExternalPermissions.Any(p =>
                                p.DepartmentId ==
                                departmentId &&
                                p.IsActive)
                        ))
                    .OrderBy(x =>
                        x.FacultyName)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // DEPARTMENTS
        // =====================================================

        public async Task<List<Department>>
            GetDepartmentsAsync()
        {
            try
            {
                return await _context.Departments
                    .OrderBy(x => x.DepartmentName)
                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // EXTERNAL FACULTIES
        // =====================================================

        public async Task<List<Faculty>>
            GetExternalFacultiesAsync(
                int departmentId)
        {
            try
            {
                return await _context.Faculties

                    .Where(x =>
                        x.IsExternal &&

                        x.ExternalPermissions.Any(p =>
                            p.DepartmentId ==
                            departmentId &&
                            p.IsActive))

                    .OrderBy(x =>
                        x.FacultyName)

                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }

        // =====================================================
        // FACULTY BY EMAIL
        // =====================================================

        public async Task<Faculty?>
            GetFacultyByEmailAsync(
                string email)
        {
            try
            {
                return await _context.Faculties
                    .FirstOrDefaultAsync(x =>
                        x.Email != null &&
                        x.Email.ToLower() ==
                        email.ToLower());
            }
            catch
            {
                return null;
            }
        }

        // =====================================================
        // FACULTY PERMISSION
        // =====================================================

        public async Task<bool>
            IsFacultyAllowedAsync(
                int facultyId,
                int departmentId)
        {
            try
            {
                var faculty =
                    await _context.Faculties
                    .FirstOrDefaultAsync(x =>
                        x.FacultyId == facultyId);

                if (faculty == null)
                    return false;

                // SAME DEPARTMENT
                if (faculty.DepartmentId ==
                    departmentId)
                    return true;

                // EXTERNAL PERMISSION
                return await _context
                    .ExternalFacultyPermissions
                    .AnyAsync(x =>
                        x.FacultyId ==
                        facultyId &&

                        x.DepartmentId ==
                        departmentId &&

                        x.IsActive);
            }
            catch
            {
                return false;
            }
        }

        // =====================================================
        // TABLE
        // =====================================================

        public async Task<List<SubjectFaculty>>
     GetAllAsync(
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

                return await _context.SubjectFaculties

                    .Include(x => x.SubjectSemester)
                    .ThenInclude(x => x.Subject)

                    .Include(x => x.SubjectSemester)
                    .ThenInclude(x => x.Semester)

                    .Include(x => x.Division)

                    .Include(x => x.Faculty)

                    .Where(x =>
                        x.AcademicYearId == academicYearId &&

                        x.SubjectSemester.Semester.CourseId == courseId &&

                        x.SubjectSemester.Semester.SemesterPattern
                            == term.SemesterPattern)

                    .OrderByDescending(x =>
                        x.SubjectSemester.Semester.SemesterNo)

                    .ThenBy(x =>
                        x.IsActive ? 0 : 1)

                    .ToListAsync();
            }
            catch
            {
                return new();
            }
        }
        // =====================================================
        // ADD
        // =====================================================

        public async Task<(bool, string)>
            AddAsync(
                SubjectFaculty model)
        {
            try
            {
                // DUPLICATE CHECK
                bool exists =
                    await _context.SubjectFaculties
                    .AnyAsync(x =>

                        x.AcademicYearId ==
                        model.AcademicYearId &&

                        x.SubjectSemesterId ==
                        model.SubjectSemesterId &&

                        x.DivisionId ==
                        model.DivisionId &&

                        x.FacultyId ==
                        model.FacultyId &&

                        x.TeachingType ==
                        model.TeachingType);

                if (exists)
                    return (false,
                        "Faculty already assigned");

                // VALIDATE EXTERNAL FACULTY
                var division =
                    await _context.Divisions
                    .Include(x => x.Semester)
                    .ThenInclude(x => x.Course)
                    .FirstOrDefaultAsync(x =>
                        x.DivisionId ==
                        model.DivisionId);

                if (division == null)
                    return (false,
                        "Division not found");

                int departmentId =
                    division.Semester.Course
                    .DepartmentId;

                bool allowed =
                    await IsFacultyAllowedAsync(
                        model.FacultyId,
                        departmentId);

                if (!allowed)
                    return (false,
                        "Faculty not allowed for this department");

                _context.SubjectFaculties
                    .Add(model);

                await _context.SaveChangesAsync();

                return (true,
                    "Faculty assigned successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // =====================================================
        // UPDATE
        // =====================================================

        public async Task<(bool, string)>
            UpdateAsync(
                SubjectFaculty model)
        {
            try
            {
                var old =
                    await _context.SubjectFaculties
                    .FirstOrDefaultAsync(x =>
                        x.SubjectFacultyId ==
                        model.SubjectFacultyId);

                if (old == null)
                    return (false,
                        "Record not found");

                bool exists =
                    await _context.SubjectFaculties
                    .AnyAsync(x =>

                        x.SubjectFacultyId !=
                        model.SubjectFacultyId &&

                        x.AcademicYearId ==
                        model.AcademicYearId &&

                        x.SubjectSemesterId ==
                        model.SubjectSemesterId &&

                        x.DivisionId ==
                        model.DivisionId &&

                        x.FacultyId ==
                        model.FacultyId &&

                        x.TeachingType ==
                        model.TeachingType);

                if (exists)
                    return (false,
                        "Duplicate assignment");

                old.SubjectSemesterId =
                    model.SubjectSemesterId;

                old.DivisionId =
                    model.DivisionId;

                old.FacultyId =
                    model.FacultyId;

                old.TeachingType =
                    model.TeachingType;

                await _context.SaveChangesAsync();

                return (true,
                    "Updated successfully");
            }
            catch (Exception ex)
            {
                return (false,
                    ex.Message);
            }
        }

        // =====================================================
        // TOGGLE
        // =====================================================

        public async Task<(bool, string)>
            ToggleAsync(int id)
        {
            try
            {
                var item =
                    await _context.SubjectFaculties
                    .FirstOrDefaultAsync(x =>
                        x.SubjectFacultyId == id);

                if (item == null)
                    return (false,
                        "Record not found");

                item.IsActive =
                    !item.IsActive;

                await _context.SaveChangesAsync();

                return (true,
                    item.IsActive
                    ? "Activated"
                    : "Deactivated");
            }
            catch (Exception ex)
            {
                return (false,
                    ex.Message);
            }
        }

        // =====================================================
        // BULK INSERT
        // =====================================================

        public async Task<(bool, string)>
            BulkInsertAsync(
                List<SubjectFaculty> list)
        {
            try
            {
                if (!list.Any())
                    return (false,
                        "No records found");

                foreach (var item in list)
                {
                    bool exists =
                        await _context.SubjectFaculties
                        .AnyAsync(x =>

                            x.AcademicYearId ==
                            item.AcademicYearId &&

                            x.SubjectSemesterId ==
                            item.SubjectSemesterId &&

                            x.DivisionId ==
                            item.DivisionId &&

                            x.FacultyId ==
                            item.FacultyId &&

                            x.TeachingType ==
                            item.TeachingType);

                    if (exists)
                        continue;

                    _context.SubjectFaculties
                        .Add(item);
                }

                await _context.SaveChangesAsync();

                return (true,
                    "CSV uploaded successfully");
            }
            catch (Exception ex)
            {
                return (false,
                    ex.Message);
            }
        }
    }
}