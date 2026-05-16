namespace ScheduleX.Web.DTOs
{
    public class DivisionPreviewGroupDto
    {
        public string Key { get; set; } = string.Empty;

        public string Semester { get; set; } = string.Empty;

        public string Division { get; set; } = string.Empty;

        public List<PreviewDto> Entries { get; set; } = new();
    }
}