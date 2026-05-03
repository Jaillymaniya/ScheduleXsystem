//using ScheduleX.Core.Entities;
//using System.Text.RegularExpressions;

//namespace ScheduleX.Web.Helpers
//{
//    public class SubjectCsvValidator
//    {
//    }
//}
using ScheduleX.Core.Entities;
using System.Text.RegularExpressions;

public static class SubjectCsvValidator
{
    public static (bool isValid, List<string> errors, List<Subject> validData)
        Validate(List<string[]> rows)
    {
        var errors = new List<string>();
        var valid = new List<Subject>();

        int rowNo = 1;

        foreach (var row in rows)
        {
            rowNo++;

            bool hasError = false;

            try
            {
                // ✅ COLUMN CHECK
                if (row.Length < 6)
                {
                    errors.Add($"Row {rowNo}: Missing columns");
                    continue;
                }

                string name = row[0]?.Trim() ?? "";
                string code = row[1]?.Trim() ?? "";
                string categoryText = row[2]?.Trim() ?? "";
                string theoryText = row[3]?.Trim() ?? "";
                string practicalText = row[4]?.Trim() ?? "";
                string electiveText = row[5]?.Trim() ?? "";

                // =========================
                // NAME VALIDATION
                // =========================
                if (string.IsNullOrWhiteSpace(name) ||
                    !Regex.IsMatch(name, @"^[a-zA-Z0-9 &().-]+$"))
                {
                    errors.Add($"Row {rowNo}: Invalid Subject Name");
                    hasError = true;
                }

                // =========================
                // CODE VALIDATION
                // =========================
                if (!string.IsNullOrWhiteSpace(code) &&
                    !Regex.IsMatch(code, @"^[a-zA-Z0-9]+$"))
                {
                    errors.Add($"Row {rowNo}: Invalid Subject Code");
                    hasError = true;
                }

                // =========================
                // CATEGORY VALIDATION
                // =========================
                if (!Enum.TryParse<SubjectCategoryEnum>(categoryText, true, out var category))
                {
                    errors.Add($"Row {rowNo}: Invalid Category (Use Theory/Practical/Both)");
                    hasError = true;
                }

                // =========================
                // CREDIT PARSE (SAFE)
                // =========================
                int theory = 0;
                int practical = 0;

                if (!int.TryParse(theoryText, out theory))
                    theory = 0;

                if (!int.TryParse(practicalText, out practical))
                    practical = 0;

                // =========================
                // CATEGORY LOGIC
                // =========================
                if (category == SubjectCategoryEnum.Theory)
                {
                    practical = 0;

                    if (theory < 0 || theory > 9)
                    {
                        errors.Add($"Row {rowNo}: Theory credit must be 0-9");
                        hasError = true;
                    }
                }
                else if (category == SubjectCategoryEnum.Practical)
                {
                    theory = 0;

                    if (practical < 0 || practical > 9)
                    {
                        errors.Add($"Row {rowNo}: Practical credit must be 0-9");
                        hasError = true;
                    }
                }
                else if (category == SubjectCategoryEnum.Both)
                {
                    if (theory < 0 || theory > 9 || practical < 0 || practical > 9)
                    {
                        errors.Add($"Row {rowNo}: Both credits must be 0-9");
                        hasError = true;
                    }
                }

                // =========================
                // ELECTIVE VALIDATION
                // =========================
                bool isElective = false;

                if (!string.IsNullOrWhiteSpace(electiveText))
                {
                    if (!bool.TryParse(electiveText, out isElective))
                    {
                        errors.Add($"Row {rowNo}: IsElective must be true/false");
                        hasError = true;
                    }
                }

                // =========================
                // ADD ONLY VALID ROW
                // =========================
                if (!hasError)
                {
                    valid.Add(new Subject
                    {
                        SubjectName = name,
                        SubjectCode = code,
                        SubjectCategory = category,
                        TheoryCredits = theory,
                        PracticalCredits = practical,
                        IsElective = isElective
                    });
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Row {rowNo}: Unexpected error ({ex.Message})");
            }
        }

        return (errors.Count == 0, errors, valid);
    }
}