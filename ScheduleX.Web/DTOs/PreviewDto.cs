namespace ScheduleX.Web.DTOs
{
    public class PreviewDto
    {
        public int EntryId { get; set; }

        public int DayOfWeek { get; set; }

        public int SlotNo { get; set; }

        public string SlotTime { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Faculty { get; set; } = string.Empty;

        public string Room { get; set; } = string.Empty;

        public string Division { get; set; } = string.Empty;

        public string Semester { get; set; } = string.Empty;

        public string EntryType { get; set; } = string.Empty;

        public int? SubjectSemesterId { get; set; }

        public int? RoomId { get; set; }

        public int? FacultyId { get; set; }

        public Guid? BlockId { get; set; }

        public byte? BlockPart { get; set; }

        public bool IsBreak { get; set; }

        public bool IsProject { get; set; }

        public bool IsSelfStudy { get; set; }
    }
}