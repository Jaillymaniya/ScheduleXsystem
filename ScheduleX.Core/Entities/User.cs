//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ScheduleX.Core.Entities
//{
//    public enum UserRole : byte
//    {
//        Admin = 1,
//        TTCoordinator = 2
//    }

//    public class User
//    {
//        [Key]
//        public int UserId { get; set; }

//        [Required(ErrorMessage = "Full Name is required")]
//        [MaxLength(100)]
//        [RegularExpression(@"^[a-zA-Z\s]+$",
//            ErrorMessage = "Full Name must contain only letters")]
//        public string FullName { get; set; } = null!;

//        [Required(ErrorMessage = "Username is required")]
//        [MaxLength(50)]
//        public string Username { get; set; } = null!;

//        [Required]
//        [MaxLength(255)]
//        public string PasswordHash { get; set; } = null!;

//        [Required]
//        public UserRole Role { get; set; }

//        public int? DepartmentId { get; set; }

//        [ForeignKey(nameof(DepartmentId))]
//        public Department? Department { get; set; }

//        [Required(ErrorMessage = "Email is required")]
//        [EmailAddress(ErrorMessage = "Invalid Email format")]
//        [MaxLength(120)]
//        public string? Email { get; set; }

//        [Required(ErrorMessage = "Phone is required")]
//        [RegularExpression(@"^[0-9]{10}$",
//            ErrorMessage = "Phone must be exactly 10 digits")]
//        [MaxLength(10)]
//        public string? Phone { get; set; }

//        public bool IsActive { get; set; } = true;

//        public DateTime CreatedAt { get; set; } = DateTime.Now;

//        public ICollection<TTCoordinatorCourse> TTCoordinatorCourses { get; set; }
//            = new List<TTCoordinatorCourse>();

//        public ICollection<TimeTableBatch> CreatedBatches { get; set; }
//            = new List<TimeTableBatch>();

//        //public ICollection<TimeTableEntryHistory> TimeTableEntryHistories { get; set; }
//        //    = new List<TimeTableEntryHistory>();

//        //public ICollection<ExportHistory> ExportHistories { get; set; }
//        //    = new List<ExportHistory>();
//    }
//}
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities
{
    public enum UserRole : byte
    {
        Admin = 1,
        TTCoordinator = 2
    }

    public class User : IdentityUser<int>
    {
        // 🔥 FULL NAME (YOUR FIELD)
        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessage = "Full Name must contain only letters")]
        public string FullName { get; set; } = null!;

        // 🔥 ROLE (YOUR BUSINESS LOGIC)
        [Required]
        public UserRole Role { get; set; }

        // 🔥 DEPARTMENT
        public int? DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }

        // 🔥 PHONE (USE IDENTITY FIELD WITH VALIDATION)
        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Phone must be exactly 10 digits")]
        [MaxLength(10)]
        public override string? PhoneNumber { get; set; }

        // 🔥 EMAIL VALIDATION (EXTEND IDENTITY FIELD)
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public override string? Email { get; set; }

        // 🔥 USERNAME VALIDATION (EXTEND IDENTITY FIELD)
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50)]
        public override string? UserName { get; set; }

        // 🔥 EXTRA FIELDS
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 🔥 RELATIONS
        public ICollection<TTCoordinatorCourse> TTCoordinatorCourses { get; set; }
            = new List<TTCoordinatorCourse>();

        public ICollection<TimeTableBatch> CreatedBatches { get; set; }
            = new List<TimeTableBatch>();
    }
}