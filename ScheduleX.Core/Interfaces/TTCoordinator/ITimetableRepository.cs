using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator
{
    public interface ITimetableRepository
    {
        Task<(bool Success, string Message, int BatchId, List<TimeTableEntry> Entries)>
            GenerateAsync(
                int userId,
                int academicYearId,
                int academicTermId,
                int courseId,
                int templateId
            );

        Task<List<Course>> GetCoursesForCoordinatorAsync(int userId);

        Task<List<AcademicTerm>> GetTermsByCourseAsync(
            int academicYearId,
            int courseId
        );
        Task<List<AcademicYear>> GetAcademicYearsAsync();
        Task<List<TimeTableTemplate>> GetTemplatesAsync();

       

        Task<(bool Success, string Message)> SwapEntriesAsync(
            int entryId1,
            int entryId2,
            int userId
        );
    }
}