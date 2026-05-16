namespace ScheduleX.Web.DTOs
{
    public class ViewBatchCardDto
    {
        public int BatchId { get; set; }

        public string AcademicYear { get; set; } = string.Empty;

        public string Course { get; set; } = string.Empty;

        public string Term { get; set; } = string.Empty;

        public string TemplateName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int TotalEntries { get; set; }

        public int TotalDivisions { get; set; }
    }
}