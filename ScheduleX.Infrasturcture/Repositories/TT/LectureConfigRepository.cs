using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ScheduleX.Infrastructure.Repositories.TT
{
    public class LectureConfigRepository : ILectureConfigRepository
    {
        private readonly AppDbContext _context;

        public LectureConfigRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubjectSemester>> GetSubjectsAsync(int semesterId, int academicYearId)
        {
            return await _context.SubjectSemesters
                .AsNoTracking()
        .Where(x =>
            x.SemesterId == semesterId &&
            x.AcademicYearId == academicYearId &&
            x.IsActive)
        .Include(x => x.Subject)
        .ToListAsync();
        }

        public async Task<List<SubjectLectureConfig>> GetBySemesterAsync(int semesterId, int academicYearId)
        {
            //return await _context.SubjectLectureConfigs
            //    .Include(x => x.SubjectSemester)
            //    .Where(x => x.SubjectSemester.SemesterId == semesterId
            //             && x.AcademicYearId == academicYearId
            //             && x.IsActive)
            //    .ToListAsync();
            var subjectSemesterIds = await _context.SubjectSemesters
                .AsNoTracking()
        .Where(x => x.SemesterId == semesterId && x.IsActive)
        .Select(x => x.SubjectSemesterId)
        .ToListAsync();

            return await _context.SubjectLectureConfigs
                .Include(x => x.SubjectSemester)
                .Where(x =>
                    subjectSemesterIds.Contains(x.SubjectSemesterId) &&
                    x.AcademicYearId == academicYearId &&
                    x.IsActive)
                .ToListAsync();
        }

        public async Task<SubjectLectureConfig?> GetBySubjectSemesterAsync(int subjectSemesterId, int academicYearId)
        {
            return await _context.SubjectLectureConfigs
                .FirstOrDefaultAsync(x =>
                    x.SubjectSemesterId == subjectSemesterId &&
                    x.AcademicYearId == academicYearId &&
                    x.IsActive);
        }

        public async Task AddAsync(SubjectLectureConfig entity)
        {
            await _context.SubjectLectureConfigs.AddAsync(entity);
        }

        public Task UpdateRangeAsync(List<SubjectLectureConfig> entities)
        {
            _context.SubjectLectureConfigs.UpdateRange(entities);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        public async Task<List<Semester>> GetSemestersAsync(
    int userId,
    int courseId,
    int academicYearId)
        {
            return await _context.Semesters
                .AsNoTracking()
                .Where(x =>
                    x.CourseId == courseId &&
                    x.IsActive)
                .OrderBy(x => x.SemesterNo)
                .ToListAsync();
        }


    public async Task<List<SubjectLectureConfig>> GetBySubjectSemesterListAsync(
    List<int> subjectSemesterIds,
    int academicYearId)
        {
            return await _context.SubjectLectureConfigs
                .AsNoTracking()
                .Where(x =>
                    subjectSemesterIds.Contains(x.SubjectSemesterId) &&
                    x.AcademicYearId == academicYearId &&
                    x.IsActive)
                .ToListAsync();
        }

    }
}
