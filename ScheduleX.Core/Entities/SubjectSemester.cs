using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleX.Core.Entities
{
    public class SubjectSemester
    {
        [Key]
        public int SubjectSemesterId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public Subject Subject { get; set; } = null!;

        [Required]
        public int SemesterId { get; set; }

        [ForeignKey(nameof(SemesterId))]
        public Semester Semester { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<SubjectFaculty> SubjectFaculties { get; set; } = new List<SubjectFaculty>();
        public ICollection<SubjectLectureConfig> LectureConfigs { get; set; } = new List<SubjectLectureConfig>();
        public ICollection<SubjectRoomConfig> RoomConfigs { get; set; } = new List<SubjectRoomConfig>();
    }
}
