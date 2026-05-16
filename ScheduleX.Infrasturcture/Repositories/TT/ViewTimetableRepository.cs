using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TT
{
    public class ViewTimetableRepository : IViewTimetableRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ViewTimetableRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TimeTableBatch?> GetBatchWithTemplateAsync(int batchId, int userId)
        {
            // 1. Manually build the DbContextOptions on the fly
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            // 2. Create a totally isolated instance of your context just for this query
            using var isolatedContext = new AppDbContext(optionsBuilder.Options);

            // 3. Run the query using the isolated context instance
            return await isolatedContext.TimeTableBatches
                .Include(x => x.TimeTableTemplate)
                // Add any other .Include() calls your original method had here
                .SingleOrDefaultAsync(x => x.BatchId == batchId && x.CreatedByUserId == userId);
        }

        public async Task<List<TimeTableBatch>> GetCoordinatorBatchesAsync(
            int userId,
            int academicYearId,
            int courseId,
            int academicTermId)
        {
            return await _context.TimeTableBatches
                .Include(x => x.AcademicYear)
                .Include(x => x.Course)
                .Include(x => x.AcademicTerm)
                .Include(x => x.TimeTableTemplate)
                .Include(x => x.TimeTableEntries)
                .Where(x =>
                    x.CreatedByUserId == userId &&
                    x.AcademicYearId == academicYearId &&
                    x.CourseId == courseId &&
                    x.AcademicTermId == academicTermId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        //public async Task<TimeTableBatch?> GetBatchWithTemplateAsync(
        //    int batchId,
        //    int userId)
        //{
        //    return await _context.TimeTableBatches
        //        .Include(x => x.TimeTableTemplate)
        //        .FirstOrDefaultAsync(x =>
        //            x.BatchId == batchId &&
        //            x.CreatedByUserId == userId);
        //}

        public async Task<List<TimeTableEntry>> GetEntriesByBatchAsync(
            int batchId,
            int userId)
        {
            return await _context.TimeTableEntries
                .Include(x => x.TimeSlot)
                    .ThenInclude(x => x.BreakRule)
                .Include(x => x.TimeTableBatch)
                .Include(x => x.TimeSlot)
                .Include(x => x.Semester)
                .Include(x => x.Division)
                .Include(x => x.Room)
                .Include(x => x.SubjectSemester)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.SubjectSemester)
                    .ThenInclude(x => x.SubjectFaculties)
                        .ThenInclude(x => x.Faculty)
                .Where(x =>
                    x.BatchId == batchId &&
                    x.TimeTableBatch.CreatedByUserId == userId)
                .OrderBy(x => x.Semester.SemesterNo)
                .ThenBy(x => x.Division.DivisionName)
                .ThenBy(x => x.DayOfWeek)
                .ThenBy(x => x.TimeSlot.SlotNo)
                .ToListAsync();
        }

        public async Task<TimeTableBatch?> GetBatchWithTemplate(int batchId)
        {
            return await _context.TimeTableBatches
                .Include(x => x.TimeTableTemplate)
                .FirstOrDefaultAsync(x => x.BatchId == batchId);
        }

        public async Task<List<TimeTableEntry>> GetEntriesByBatch(int batchId)
        {
            return await _context.TimeTableEntries
                .Include(x => x.TimeSlot)
                    .ThenInclude(x => x.BreakRule)
                .Include(x => x.Semester)
                .Include(x => x.Division)
                .Include(x => x.Room)
                .Include(x => x.SubjectSemester)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.SubjectSemester)
                    .ThenInclude(x => x.SubjectFaculties)
                        .ThenInclude(x => x.Faculty)
                .Where(x => x.BatchId == batchId)
                .OrderBy(x => x.Semester.SemesterNo)
                .ThenBy(x => x.Division.DivisionName)
                .ThenBy(x => x.DayOfWeek)
                .ThenBy(x => x.TimeSlot.SlotNo)
                .ToListAsync();
        }

        public async Task<bool> DeleteBatchAsync(int batchId, int userId)
        {
            var batch = await _context.TimeTableBatches
                .Include(x => x.TimeTableEntries)
                .Include(x => x.BatchSemesters)
                .FirstOrDefaultAsync(x =>
                    x.BatchId == batchId &&
                    x.CreatedByUserId == userId);

            if (batch == null)
                return false;

            _context.TimeTableEntries.RemoveRange(batch.TimeTableEntries);
            _context.TimeTableBatchSemesters.RemoveRange(batch.BatchSemesters);
            _context.TimeTableBatches.Remove(batch);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}