using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleX.Core.Entities
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [MaxLength(100)]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Only alphabets and spaces allowed")]
        public string DepartmentName { get; set; } = null!;

        [MaxLength(20)]
        [RegularExpression(@"^[A-Za-z0-9]*$",
           ErrorMessage = "Only alphanumeric characters allowed")]
        public string? DepartmentCode { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Nav
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();

        public ICollection<ScheduleConfig> ScheduleConfigs { get; set; } = new List<ScheduleConfig>();
        public ICollection<TimeTableBatch> TimeTableBatches { get; set; } = new List<TimeTableBatch>();
    }
}
