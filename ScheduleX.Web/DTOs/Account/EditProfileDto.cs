using System.ComponentModel.DataAnnotations;

namespace ScheduleX.Web.DTOs.Account
{
    public class EditProfileDto
    {
        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters allowed")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50)]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter valid 10 digit number")]
        public string PhoneNumber { get; set; } = "";

        public string Email { get; set; } = "";
    }
}