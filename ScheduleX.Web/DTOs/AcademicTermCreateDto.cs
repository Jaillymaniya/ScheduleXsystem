using ScheduleX.Core.Entities;
namespace ScheduleX.Web.DTOs
{
    public class AcademicTermCreateDto
    {
        public int CourseId { get; set; }

        public TermTypeEnum TermType { get; set; }

        public SemesterPatternEnum SemesterPattern { get; set; }

        public TermStatusEnum Status { get; set; }

        public bool IsCurrent { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }
    }
}
