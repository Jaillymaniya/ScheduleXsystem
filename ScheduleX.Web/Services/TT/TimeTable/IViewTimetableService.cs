using ScheduleX.Web.DTOs;

namespace ScheduleX.Web.Services.TT.TimeTable
{
    public interface IViewTimetableService
    {
        Task<ViewTimetableResultDto> GetBatchesAsync(
            LoadViewTimetableRequestDto dto
        );

        Task<ViewTimetableResultDto> GetBatchPreviewAsync(
            int batchId,
            int userId
        );

        Task<ViewTimetableResultDto> DownloadBatchAsync(
            int batchId,
            int userId
        );

        Task<bool> DeleteBatchAsync(
            int batchId,
            int userId
        );
    }
}
