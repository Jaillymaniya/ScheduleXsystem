namespace ScheduleX.Web.DTOs
{
    public class LoadViewTimetableRequestDto
    {
        public int UserId { get; set; }

        public int AcademicYearId { get; set; }

        public int CourseId { get; set; }

        public int AcademicTermId { get; set; }
    }
}