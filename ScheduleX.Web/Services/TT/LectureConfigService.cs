using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;

namespace ScheduleX.Web.Services.TT
{
    public class LectureConfigService
    {
        private readonly ILectureConfigRepository _repo;

        public LectureConfigService(ILectureConfigRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Semester>> GetSemestersAsync(
    int userId,
    int courseId,
    int academicYearId,
    int academicTermId)
        {
            return _repo.GetSemestersAsync(userId, courseId, academicYearId, academicTermId);
        }

        public Task<List<SubjectSemester>> GetSubjectsAsync(int semesterId, int academicYearId)
        {
            return _repo.GetSubjectsAsync(semesterId, academicYearId);
        }

        public Task<List<SubjectLectureConfig>> GetConfigsAsync(int semesterId, int academicYearId)
        {
            return _repo.GetBySemesterAsync(semesterId, academicYearId);
        }

        public async Task SaveAsync(List<SubjectLectureConfig> configs)
        {
            var academicYearId = configs.First().AcademicYearId;
            var subjectSemesterIds = configs.Select(x => x.SubjectSemesterId).ToList();

            var existingList = await _repo.GetBySubjectSemesterListAsync(
                subjectSemesterIds,
                academicYearId
            );

            var existingMap = existingList.ToDictionary(x => x.SubjectSemesterId);

            foreach (var item in configs)
            {
                if (existingMap.TryGetValue(item.SubjectSemesterId, out var existing))
                {
                    existing.TheoryLecturesPerWeek = item.TheoryLecturesPerWeek;
                    existing.PracticalLecturesPerWeek = item.PracticalLecturesPerWeek;
                    existing.PracticalBlockSize = item.PracticalBlockSize;
                }
                else
                {
                    await _repo.AddAsync(item);
                }
            }

            await _repo.SaveChangesAsync();
        }
    }
}