using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities
{
    public enum TermTypeEnum : byte
    {
        Winter = 1,
        Summer = 2,
        Annual = 3
    }

    public enum TermStatusEnum : byte
    {
        Upcoming = 1,
        Running = 2,
        Completed = 3
    }

    public enum SemesterPatternEnum : byte
    {
        Odd = 1,
        Even = 2,
        Annual = 3,
        Custom = 4
    }

    public class AcademicTerm
    {
        [Key]
        public int AcademicTermId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [ForeignKey(nameof(AcademicYearId))]
        public AcademicYear AcademicYear { get; set; } = null!;

        [Required]
        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;

        [Required]
        public TermTypeEnum TermType { get; set; }

        [Required]
        public SemesterPatternEnum SemesterPattern { get; set; }

        [Required]
        public TermStatusEnum Status { get; set; } = TermStatusEnum.Upcoming;

        public bool IsCurrent { get; set; } = false;

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // NAVIGATION
        public ICollection<DivisionRoomAllocation> DivisionRoomAllocations { get; set; }
            = new List<DivisionRoomAllocation>();

        public ICollection<TimeTableBatch> TimeTableBatches { get; set; }
            = new List<TimeTableBatch>();
    }
}