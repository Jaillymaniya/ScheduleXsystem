using ScheduleX.Core.Entities;
using System.Text;
using System.Text.RegularExpressions;

namespace ScheduleX.Web.Helpers
{
    public static class FacultyCsvValidator
    {
        public static (List<Faculty> valid, List<string> errors)
            Validate(List<string[]> rows, List<Department> departments)
        {
            var valid = new List<Faculty>();
            var errors = new List<string>();

            int rowNo = 1;

            foreach (var row in rows)
            {
                rowNo++;

                try
                {
                    if (row.Length < 8)
                    {
                        errors.Add($"Row {rowNo}: Missing columns");
                        continue;
                    }

                    string name = row[0]?.Trim();
                    string code = row[1]?.Trim();
                    string email = row[2]?.Trim();
                    string phone = row[3]?.Trim();
                    string deptName = row[4]?.Trim();
                    string isExternalText = row[5]?.Trim();
                    string maxLecturesText = row[6]?.Trim();
                    string allowedDepartments = row[7]?.Trim();

                    var dept = departments
                        .FirstOrDefault(x =>
                            x.DepartmentName.Equals(deptName, StringComparison.OrdinalIgnoreCase));

                    if (dept == null)
                    {
                        errors.Add($"Row {rowNo}: Department '{deptName}' not found");
                        continue;
                    }

                    bool isExternal = bool.TryParse(isExternalText, out var ex) && ex;
                    int maxLectures = int.TryParse(maxLecturesText, out var ml) ? ml : 0;

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        errors.Add($"Row {rowNo}: Faculty name required");
                        continue;
                    }

                    valid.Add(new Faculty
                    {
                        FacultyName = name,
                        FacultyCode = code,
                        Email = email,
                        Phone = phone,
                        DepartmentId = dept.DepartmentId,
                        IsExternal = isExternal,
                        MaxLecturesPerDay = maxLectures,
                        AllowedDepartmentsCsv = allowedDepartments,
                        IsActive = true
                    });
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {rowNo}: {ex.Message}");
                }
            }

            return (valid, errors);
        }

        public static byte[] GenerateFacultyTemplate()
        {
            var sb = new StringBuilder();

            sb.AppendLine("FacultyName,FacultyCode,Email,Phone,DepartmentName,IsExternal,MaxLecturesPerDay,AllowedDepartments(Use | separator)");
            //sb.AppendLine("John Doe,FD101,john@gmail.com,9876543210,Computer Science,false,4");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}