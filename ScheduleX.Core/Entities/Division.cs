
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ScheduleX.Core.Entities
{
    [Index(
        nameof(AcademicYearId),
        nameof(CourseId),
        nameof(SemesterId),
        nameof(DivisionName),
        nameof(TTCoordinatorId),
        IsUnique = true)]

    public class Division
    {
        [Key]
        public int DivisionId { get; set; }

        // ACADEMIC YEAR

        [Required]
        public int AcademicYearId { get; set; }

        [ForeignKey(nameof(AcademicYearId))]
        public AcademicYear AcademicYear { get; set; } = null!;

        // COURSE

        [Required]
        //public int CourseId { get; set; }
        public int? CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;

        // SEMESTER

        [Required]
        public int SemesterId { get; set; }

        [ForeignKey(nameof(SemesterId))]
        public Semester Semester { get; set; } = null!;

        // DIVISION NAME

        [Required]
        [MaxLength(20)]
        public string DivisionName { get; set; } = null!;

        // STUDENT STRENGTH

        [Required]
        public int StudentStrength { get; set; }

        // ACTIVE

        public bool IsActive { get; set; } = true;

        // TT COORDINATOR

        public int? TTCoordinatorId { get; set; }

        [ForeignKey(nameof(TTCoordinatorId))]
        public User TTCoordinator { get; set; } = null!;

        // CREATED DATE

        public DateTime CreatedAt { get; set; }
            = DateTime.Now;

        // NAVIGATION

        public ICollection<DivisionRoomAllocation>
            DivisionRoomAllocations
        { get; set; }
            = new List<DivisionRoomAllocation>();

        public ICollection<TimeTableEntry>
            TimeTableEntries
        { get; set; }
            = new List<TimeTableEntry>();

        public ICollection<SubjectFaculty>
            SubjectFaculties
        { get; set; }
            = new List<SubjectFaculty>();
    }
}