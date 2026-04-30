using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleX.Core.Entities
{
    public class SubjectRoomConfig
    {
        [Key]
        public int SubjectRoomConfigId { get; set; }

        [Required]
        public int SubjectSemesterId { get; set; }

        [ForeignKey(nameof(SubjectSemesterId))]
        public SubjectSemester SubjectSemester { get; set; } = null!;

        public int? RoomId { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }

        public RoomTypeEnum? PreferredRoomType { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
