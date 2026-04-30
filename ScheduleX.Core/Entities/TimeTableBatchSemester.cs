using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleX.Core.Entities
{
    public class TimeTableBatchSemester
    {
        [Key]
        public int BatchSemesterId { get; set; }

        [Required]
        public int BatchId { get; set; }

        [ForeignKey(nameof(BatchId))]
        public TimeTableBatch TimeTableBatch { get; set; } = null!;

        [Required]
        public int SemesterId { get; set; }

        [ForeignKey(nameof(SemesterId))]
        public Semester Semester { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
