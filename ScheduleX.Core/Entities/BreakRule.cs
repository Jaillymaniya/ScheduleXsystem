using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleX.Core.Entities
{
    public class BreakRule
    {
        [Key]
        public int BreakRuleId { get; set; }

        [Required]
        public int ConfigId { get; set; }

        [ForeignKey(nameof(ConfigId))]
        public ScheduleConfig ScheduleConfig { get; set; } = null!;

        [Required]
        public byte BreakNo { get; set; }

        [Required]
        public byte AfterLectureNo { get; set; }

        [Required]
        public int BreakDurationMin { get; set; }

        [MaxLength(50)]
        public string? BreakName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Nav
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    }
}
