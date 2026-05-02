using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleX.Core.Entities
{
    public class AcademicYear
    {
        [Key]
        public int AcademicYearId { get; set; }

        [Required]
        [MaxLength(20)]
        public string YearName { get; set; } = null!; // "2025-26"

      
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // =========================
        // NAVIGATION PROPERTIES
        // =========================

        // Timetable batches for this academic year
        public ICollection<TimeTableBatch> TimeTableBatches { get; set; } = new List<TimeTableBatch>();

        // Year-wise divisions
        public ICollection<Division> Divisions { get; set; } = new List<Division>();

        // Faculty assignments per year
        public ICollection<SubjectFaculty> SubjectFaculties { get; set; } = new List<SubjectFaculty>();

        // Lecture configs per year
        public ICollection<SubjectLectureConfig> SubjectLectureConfigs { get; set; } = new List<SubjectLectureConfig>();

        // Schedule configurations per year
        public ICollection<ScheduleConfig> ScheduleConfigs { get; set; } = new List<ScheduleConfig>();
    }

}
