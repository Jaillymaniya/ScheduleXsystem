using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.Admin;
using ScheduleX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ScheduleX.Infrastructure.Repositories.Admin
{

    public class AcademicTermRepository : IAcademicTermRepository
    {
        private readonly AppDbContext _db;

        public AcademicTermRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<AcademicTerm>> GetByCourseAsync(int courseId)
        {
            return await _db.AcademicTerms
                .Include(x => x.AcademicYear)
                .Include(x => x.Course) // ✅ ADD THIS
                .Where(x => x.CourseId == courseId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(AcademicTerm term)
        {
            var activeYear = await _db.AcademicYears
                .FirstOrDefaultAsync(x => x.IsActive);

            if (activeYear == null)
                throw new Exception("No active academic year found.");

            term.AcademicYearId = activeYear.AcademicYearId;

            term.CreatedAt = DateTime.Now;

            _db.AcademicTerms.Add(term);

            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(AcademicTerm term)
        {
            var existing = await _db.AcademicTerms
                .FirstOrDefaultAsync(x => x.AcademicTermId == term.AcademicTermId);

            if (existing == null)
                throw new Exception("Term not found.");

            existing.TermType = term.TermType;
            existing.SemesterPattern = term.SemesterPattern;
            existing.Status = term.Status;
            existing.IsCurrent = term.IsCurrent;
            existing.StartDate = term.StartDate;
            existing.EndDate = term.EndDate;

            await _db.SaveChangesAsync();
        }
    }
}