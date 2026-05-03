//namespace ScheduleX.Web.Helpers
//{
//    public class CsvHelper
//    {
//    }
//}
using System.Text;

public static class CsvHelper
{
    public static byte[] GenerateSubjectTemplate()
    {
        try
        {
            var sb = new StringBuilder();

            sb.AppendLine("SubjectName,SubjectCode,SubjectCategory,TheoryCredits,PracticalCredits,IsElective");
            sb.AppendLine("Maths,MTH101,Theory,4,0,false");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
        catch
        {
            throw new Exception("CSV generation failed");
        }
    }
}