using ScheduleX.Core.Entities;
namespace ScheduleX.Web.DTOs
{
    public class AcademicTermDto
    {

        public int AcademicTermId { get; set; }

        public int CourseId { get; set; }

        public string CourseName { get; set; } = "";

        public string AcademicYearName { get; set; } = "";

        public TermTypeEnum TermType { get; set; }

        public SemesterPatternEnum SemesterPattern { get; set; }

        public TermStatusEnum Status { get; set; }

        public bool IsCurrent { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }
    }
}
