using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator
{
    public interface ILectureConfigRepository
    {
        Task<List<SubjectSemester>> GetSubjectsAsync(int semesterId, int academicYearId);

        Task<List<SubjectLectureConfig>> GetBySemesterAsync(int semesterId, int academicYearId);

        Task<SubjectLectureConfig?> GetBySubjectSemesterAsync(int subjectSemesterId, int academicYearId);

        Task<List<Semester>> GetSemestersAsync(
    int userId,
    int courseId,
    int academicYearId);

        Task AddAsync(SubjectLectureConfig entity);

        Task SaveChangesAsync();

        Task<List<SubjectLectureConfig>> GetBySubjectSemesterListAsync(
    List<int> subjectSemesterIds,
    int academicYearId);
    }
}
