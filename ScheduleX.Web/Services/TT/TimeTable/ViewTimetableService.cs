using System.Text.Json;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Web.DTOs;
using ScheduleX.Web.Models.Template;
using ScheduleX.Web.Services.Excel;

namespace ScheduleX.Web.Services.TT.TimeTable
{
    public class ViewTimetableService : IViewTimetableService
    {
        private readonly IViewTimetableRepository _repo;
        private readonly IExcelService _excelService;

        public ViewTimetableService(
            IViewTimetableRepository repo,
            IExcelService excelService)
        {
            _repo = repo;
            _excelService = excelService;
        }

        public async Task<ViewTimetableResultDto> GetBatchesAsync(
            LoadViewTimetableRequestDto dto)
        {
            var batches = await _repo.GetCoordinatorBatchesAsync(
                dto.UserId,
                dto.AcademicYearId,
                dto.CourseId,
                dto.AcademicTermId);

            return new ViewTimetableResultDto
            {
                Success = true,
                Batches = batches.Select(x => new ViewBatchCardDto
                {
                    BatchId = x.BatchId,
                    AcademicYear = x.AcademicYear.YearName,
                    Course = x.Course.CourseName,
                    Term = x.AcademicTerm.TermType.ToString(),
                    TemplateName = x.TimeTableTemplate.TemplateName,
                    CreatedAt = x.CreatedAt,
                    TotalEntries = x.TimeTableEntries.Count,
                    TotalDivisions = x.TimeTableEntries
                        .Select(e => $"{e.SemesterId}-{e.DivisionId}")
                        .Distinct()
                        .Count()
                }).ToList()
            };
        }

        public async Task<ViewTimetableResultDto> GetBatchPreviewAsync(
            int batchId,
            int userId)
        {
            var batch = await _repo.GetBatchWithTemplateAsync(
                batchId,
                userId);

            if (batch == null)
            {
                return new ViewTimetableResultDto
                {
                    Success = false,
                    Message = "Timetable not found."
                };
            }

            var entries = await _repo.GetEntriesByBatchAsync(
                batchId,
                userId);

            if (!entries.Any())
            {
                return new ViewTimetableResultDto
                {
                    Success = false,
                    Message = "No timetable entries found."
                };
            }

            var style = ParseTemplate(
                batch.TimeTableTemplate.TemplateJson);

            var preview = entries
                .Select(MapPreview)
                .ToList();

            var groups = preview
                .GroupBy(x => $"{x.Semester}-{x.Division}")
                .Select(g => new DivisionPreviewGroupDto
                {
                    Key = g.Key,
                    Semester = g.First().Semester,
                    Division = g.First().Division,
                    Entries = g.ToList()
                })
                .OrderBy(x => x.Semester)
                .ThenBy(x => x.Division)
                .ToList();

            return new ViewTimetableResultDto
            {
                Success = true,
                BatchId = batchId,
                TemplateStyle = style,
                Groups = groups
            };
        }

        public async Task<ViewTimetableResultDto> DownloadBatchAsync(
            int batchId,
            int userId)
        {
            var batch = await _repo.GetBatchWithTemplateAsync(
                batchId,
                userId);

            if (batch == null)
            {
                return new ViewTimetableResultDto
                {
                    Success = false,
                    Message = "Timetable not found."
                };
            }

            var entries = await _repo.GetEntriesByBatchAsync(
                batchId,
                userId);

            if (!entries.Any())
            {
                return new ViewTimetableResultDto
                {
                    Success = false,
                    Message = "No entries found."
                };
            }

            var style = ParseTemplate(
                batch.TimeTableTemplate.TemplateJson);

            var preview = entries
                .Select(MapPreview)
                .ToList();

            var excel = _excelService.GenerateExcel(
                preview,
                style);

            return new ViewTimetableResultDto
            {
                Success = true,
                Base64 = Convert.ToBase64String(excel)
            };
        }

        //    private PreviewDto MapPreview(TimeTableEntry x)
        //    {
        //        return new PreviewDto
        //        {
        //            EntryId = x.EntryId,
        //            DayOfWeek = x.DayOfWeek,
        //            SlotNo = x.TimeSlot?.SlotNo ?? 0,
        //            SlotTime = x.TimeSlot != null
        //                ? $"{x.TimeSlot.StartTime:hh\\:mm} - {x.TimeSlot.EndTime:hh\\:mm}"
        //                : "",
        //            Subject =
        //x.EntryType == EntryTypeEnum.Break
        //    ? (x.TimeSlot?.BreakRule?.BreakName ?? "Break")
        //    : x.EntryType == EntryTypeEnum.Free
        //        ? "Free"
        //        : x.SubjectSemester?.Subject?.SubjectName ?? "",
        //            Faculty = x.SubjectSemester?
        //                .SubjectFaculties?
        //                .FirstOrDefault(f => f.DivisionId == x.DivisionId)?
        //                .Faculty?
        //                .FacultyName ?? "",
        //            Room = x.Room?.RoomName ?? "",
        //            Division = x.Division?.DivisionName ?? "",
        //            Semester = $"Semester {x.Semester?.SemesterNo}",
        //            EntryType = x.EntryType.ToString(),
        //            IsBreak = x.EntryType == EntryTypeEnum.Break,
        //            IsProject = false,
        //            IsSelfStudy = x.EntryType == EntryTypeEnum.Free
        //        };
        //    }
        private PreviewDto MapPreview(TimeTableEntry x)
        {
            bool isBreak = x.EntryType == EntryTypeEnum.Break;

            // Safely check if the subject name represents a project
            string subjectName = x.SubjectSemester?.Subject?.SubjectName ?? "";

            Console.WriteLine(
    $"Subject: {subjectName} | Category: {x.SubjectSemester?.Subject?.SubjectCategory}"
);

            bool isProjectSubject = subjectName.Contains("project", StringComparison.OrdinalIgnoreCase)
                                 || subjectName.Contains("(pw)", StringComparison.OrdinalIgnoreCase);

            bool isProject = x.EntryType == EntryTypeEnum.Free && isProjectSubject;
            bool isSelfStudy = x.EntryType == EntryTypeEnum.Free && !isProjectSubject;

            return new PreviewDto
            {
                EntryId = x.EntryId,
                DayOfWeek = x.DayOfWeek,
                SlotNo = x.TimeSlot?.SlotNo ?? 0,
                SlotTime = x.TimeSlot != null
                    ? $"{x.TimeSlot.StartTime:hh\\:mm} - {x.TimeSlot.EndTime:hh\\:mm}"
                    : "",

                // Dynamic Subject text resolution
                Subject = isBreak
                    ? (x.TimeSlot?.BreakRule?.BreakName ?? "BREAK")
                    : (x.EntryType == EntryTypeEnum.Free
                        ? (isProject ? "PROJECT" : "SELF STUDY")
                        : subjectName),

                Faculty = x.SubjectSemester?
                    .SubjectFaculties?
                    .FirstOrDefault(f => f.DivisionId == x.DivisionId)?
                    .Faculty?
                    .FacultyName ?? "",
                Room = x.Room?.RoomName ?? "",
                Division = x.Division?.DivisionName ?? "",
                Semester = $"Semester {x.Semester?.SemesterNo}",
                EntryType = x.EntryType.ToString(),

                IsBreak = isBreak,
                IsProject = isProject,
                IsSelfStudy = isSelfStudy,
                IsLab = IsLab(x),
                IsTheory = !IsLab(x)
            };
        }

        private TemplateStyle ParseTemplate(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new TemplateStyle();

            try
            {
                return JsonSerializer.Deserialize<TemplateStyle>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new TemplateStyle();
            }
            catch
            {
                return new TemplateStyle();
            }
        }

        public async Task<bool> DeleteBatchAsync(
            int batchId,
            int userId)
        {
            return await _repo.DeleteBatchAsync(
                batchId,
                userId);
        }

        private bool IsLab(TimeTableEntry e)
        {
            // ✅ FIX: Determine slot type dynamically using RoomType instead of the overarching Subject Category
            if (e.Room != null)
            {
                return e.Room.RoomType == RoomTypeEnum.Lab;
            }

            // Fallback comparison if Room compilation hasn't evaluated yet
            var category = e.SubjectSemester?.Subject?.SubjectCategory;
            return category == SubjectCategoryEnum.Practical;
        }
    }
}