using ScheduleX.Web.Models.Template;

namespace ScheduleX.Web.DTOs
{
    public class ViewTimetableResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int BatchId { get; set; }

        public string? Base64 { get; set; }

        public TemplateStyle TemplateStyle { get; set; } = new();

        public List<ViewBatchCardDto> Batches { get; set; } = new();

        public List<DivisionPreviewGroupDto> Groups { get; set; } = new();
    }
}