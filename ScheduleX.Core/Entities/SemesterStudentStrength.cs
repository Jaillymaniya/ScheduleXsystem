using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleX.Core.Entities
{
    public class SemesterStudentStrength
    {
        [Key]
        public int Id { get; set; }

        // =========================
        // Semester Reference
        // =========================
        [Required]
        public int SemesterId { get; set; }

        [ForeignKey(nameof(SemesterId))]
        public Semester Semester { get; set; } = null!;

        // =========================
        // Total Students
        // =========================
        [Required]
        [Range(1, 1000)] // optional validation
        public int TotalStudents { get; set; }

        // =========================
        // Metadata
        // =========================
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
