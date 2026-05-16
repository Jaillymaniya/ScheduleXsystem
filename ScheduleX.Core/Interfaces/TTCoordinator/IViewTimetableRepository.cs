using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator
{
    public interface IViewTimetableRepository
    {
        Task<List<TimeTableBatch>> GetCoordinatorBatchesAsync(
            int userId,
            int academicYearId,
            int courseId,
            int academicTermId
        );

        Task<TimeTableBatch?> GetBatchWithTemplateAsync(
            int batchId,
            int userId
        );

        Task<List<TimeTableEntry>> GetEntriesByBatchAsync(
            int batchId,
            int userId
        );

        // used by TimeTableService
        Task<TimeTableBatch?> GetBatchWithTemplate(int batchId);

        Task<List<TimeTableEntry>> GetEntriesByBatch(int batchId);

        Task<bool> DeleteBatchAsync(
            int batchId,
            int userId
        );
    }
}