

using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TT
{
    public class DivisionRepository : IDivisionRepository
    {
        private readonly AppDbContext _context;

        public DivisionRepository(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL TT WISE

        //public async Task<List<Division>> GetAllAsync(int ttCoordinatorId)
        //{
        //    return await _context.Divisions
        //        .Include(x => x.AcademicYear)
        //        .Include(x => x.Semester)
        //        .Where(x => x.TTCoordinatorId == ttCoordinatorId)
        //        .OrderBy(x => x.DivisionName)
        //        .ToListAsync();
        //}

        public async Task<List<Division>> GetAllAsync(
      int ttCoordinatorId)
        {
            return await _context.Divisions

                .Include(x => x.AcademicYear)

                .Include(x => x.Semester)

                .Include(x => x.Course)

                .Where(x =>
                    x.TTCoordinatorId == ttCoordinatorId)

                .OrderBy(x => x.DivisionName)

                .ToListAsync();
        }

        // SEMESTERS

        public async Task<List<Semester>> GetSemestersAsync()
        {
            //return await _context.Semesters
            //    .Where(x => x.IsActive)
            //    .ToListAsync();
            return await _context.Semesters
    .ToListAsync();
        }

        // GET BY ID

        public async Task<Division?> GetByIdAsync(int id)
        {
            return await _context.Divisions
                .FirstOrDefaultAsync(x => x.DivisionId == id);
        }

        // DUPLICATE CHECK

        //public async Task<bool> ExistsAsync(
        //    int academicYearId,
        //    int semesterId,
        //    string divisionName,
        //    int ttCoordinatorId,
        //    int? excludeId = null)
        //{
        //    return await _context.Divisions.AnyAsync(x =>

        //        x.AcademicYearId == academicYearId &&

        //        x.SemesterId == semesterId &&

        //        x.TTCoordinatorId == ttCoordinatorId &&

        //        x.DivisionName.ToLower() ==
        //        divisionName.ToLower() &&

        //        (!excludeId.HasValue ||
        //         x.DivisionId != excludeId.Value)
        //    );
        //}

        public async Task<bool> ExistsAsync(
    int academicYearId,
    int courseId,
    int semesterId,
    string divisionName,
    int ttCoordinatorId,
    int? excludeId = null)
        {
            return await _context.Divisions.AnyAsync(x =>

                x.AcademicYearId == academicYearId &&

                x.CourseId == courseId &&

                x.SemesterId == semesterId &&

                x.TTCoordinatorId == ttCoordinatorId &&

                x.DivisionName.ToLower() ==
                divisionName.ToLower() &&

                (!excludeId.HasValue ||
                 x.DivisionId != excludeId.Value)
            );
        }

        // ADD

        public async Task AddAsync(Division division)
        {
            await _context.Divisions.AddAsync(division);
        }

        // UPDATE

        public async Task UpdateAsync(Division division)
        {
            var existingDivision = await _context.Divisions
                .FirstOrDefaultAsync(x =>
                    x.DivisionId == division.DivisionId);

            if (existingDivision != null)
            {
                existingDivision.SemesterId =
                    division.SemesterId;

                existingDivision.DivisionName =
                    division.DivisionName;

                existingDivision.StudentStrength =
                    division.StudentStrength;
            }
        }

        // DELETE

        public async Task DeleteAsync(int id)
        {
            var division = await _context.Divisions
                .FindAsync(id);

            if (division != null)
            {
                _context.Divisions.Remove(division);
            }
        }

        // SAVE

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}