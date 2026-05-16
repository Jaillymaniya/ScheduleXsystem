using System.ComponentModel.DataAnnotations;

namespace ScheduleX.Web.DTOs
{
    public class GenerateTTDto
    {
        [Required(ErrorMessage = "Academic year is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select academic year")]
        public int AcademicYearId { get; set; }

        [Required(ErrorMessage = "Academic term is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select academic term")]
        public int AcademicTermId { get; set; }

        [Required(ErrorMessage = "Course is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select course")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Template is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select timetable template")]
        public int TemplateId { get; set; }

        public int UserId { get; set; }
    }
}