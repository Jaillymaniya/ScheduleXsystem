using System.ComponentModel.DataAnnotations;
using ScheduleX.Core.Entities;

public class SubjectVM
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9 &().-]+$", ErrorMessage = "Invalid subject name")]
    public string SubjectName { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Invalid code")]
    public string SubjectCode { get; set; }

    [Range(0, 9)]
    public int TheoryCredits { get; set; }

    [Range(0, 9)]
    public int PracticalCredits { get; set; }

    [Required]
    public SubjectCategoryEnum SubjectCategory { get; set; }

    public bool IsElective { get; set; }

    public int CourseId { get; set; }
}