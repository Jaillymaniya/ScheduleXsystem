//using Microsoft.EntityFrameworkCore;
//using ScheduleX.Core.Entities;
//using ScheduleX.Core.Interfaces.TTCoordinator;
//using ScheduleX.Web.DTOs;
//using ScheduleX.Web.Models.Template;
//using ScheduleX.Web.Services.Excel;

//namespace ScheduleX.Web.Services.TimeTable
//{
//    public class TimeTableService : ITimeTableService
//    {
//        private readonly ITimetableRepository _repo;
//        private readonly IExcelService _excel;

//        public TimeTableService(ITimetableRepository repo, IExcelService excel)
//        {
//            _repo = repo;
//            _excel = excel;
//        }

//        //public async Task<GenerateResultDto> GetPreviewByBatch(int batchId)
//        //{
//        //    // Here, your data is stored in 'entries'
//        //    var entries = await _repo.GetEntriesByBatch(batchId);

//        //    // Change 'result.Entries' to 'entries' to fix the error
//        //    var preview = entries.Select(e => new PreviewDto
//        //    {
//        //        Day = e.DayOfWeek,
//        //        Slot = e.TimeSlot?.SlotNo ?? 0,

//        //        Subject = e.EntryType switch
//        //        {
//        //            EntryTypeEnum.Break => e.TimeSlot?.BreakRule?.BreakName ?? "Break",
//        //            EntryTypeEnum.Free => "Free",
//        //            _ => e.SubjectSemester?.Subject?.SubjectName ?? "N/A"
//        //        },

//        //        Faculty = e.EntryType == EntryTypeEnum.Lecture
//        //            ? (e.SubjectSemester?.SubjectFaculties?
//        //                .FirstOrDefault(f => f.DivisionId == e.DivisionId)?
//        //                .Faculty?.FacultyName ?? "N/A")
//        //            : "",

//        //        Room = e.Room?.RoomName ?? "N/A",
//        //        Division = e.Division?.DivisionName ?? "N/A"
//        //    }).ToList();

//        //    preview = preview
//        //        .OrderBy(x => x.Day)
//        //        .ThenBy(x => x.Slot)
//        //        .ToList();

//        //    var excel = _excel.GenerateExcel(preview);

//        //    return new GenerateResultDto
//        //    {
//        //        Success = true,
//        //        Preview = preview,
//        //        Base64 = Convert.ToBase64String(excel)
//        //    };
//        //}
//        public async Task<GenerateResultDto> GetPreviewByBatch(int batchId)
//        {
//            var entries = await _repo.GetEntriesByBatch(batchId);
//            var batch = await _repo.GetBatchWithTemplate(batchId);

//            TemplateStyle style;

//            try
//            {
//                if (!string.IsNullOrEmpty(batch?.TimeTableTemplate?.TemplateJson))
//                {
//                    style = System.Text.Json.JsonSerializer.Deserialize<TemplateStyle>(
//                        batch.TimeTableTemplate.TemplateJson
//                    );
//                }
//                else
//                {
//                    style = GetDefaultStyle();
//                }
//            }
//            catch
//            {
//                style = GetDefaultStyle();
//            }

//            var preview = entries.Select(e => new PreviewDto
//            {
//                Day = e.DayOfWeek,
//                Slot = e.TimeSlot?.SlotNo ?? 0,

//                Subject = e.EntryType switch
//                {
//                    EntryTypeEnum.Break => e.TimeSlot?.BreakRule?.BreakName ?? "Break",
//                    EntryTypeEnum.Free => "Free",
//                    _ => e.SubjectSemester?.Subject?.SubjectName ?? "N/A"
//                },

//                Faculty = e.EntryType == EntryTypeEnum.Lecture
//                    ? e.SubjectSemester?.SubjectFaculties?
//                        .FirstOrDefault(f => f.DivisionId == e.DivisionId)?
//                        .Faculty?.FacultyName ?? ""
//                    : "",

//                Room = e.Room?.RoomName ?? "",
//                Division = e.Division?.DivisionName ?? ""
//            })
//            .OrderBy(x => x.Day)
//            .ThenBy(x => x.Slot)
//            .ToList();

//            var excel = _excel.GenerateExcel(preview, style);

//            return new GenerateResultDto
//            {
//                Success = true,
//                Preview = preview,
//                Base64 = Convert.ToBase64String(excel),
//                TemplateStyle = style
//            };
//        }

//        private TemplateStyle GetDefaultStyle()
//        {
//            return new TemplateStyle
//            {
//                headerBg = "#1e293b",
//                headerText = "#ffffff",
//                bodyBg = "#ffffff",
//                bodyText = "#111827",
//                borderColor = "#cbd5e1",
//                cellPadding = "8px",
//                fontSize = "14px",
//                showRoom = true,
//                showFaculty = true,
//                titleAlign = "center"
//            };
//        }

//        public async Task<GenerateResultDto> GenerateAsync(GenerateTTDto dto)
//        {
//            try
//            {
//                // Here, the repository returns a tuple where the entries are inside 'result.Entries'
//                var result = await _repo.GenerateAsync(
//                    dto.UserId,
//                    dto.CourseId,
//                    dto.SemesterIds,
//                    dto.TemplateId
//                );

//                if (!result.Success || result.Entries == null)
//                {
//                    return new GenerateResultDto
//                    {
//                        Success = false,
//                        Message = result.Message
//                    };
//                }
//                var batch = await _repo.GetBatchWithTemplate(result.Entries.First().BatchId);
//                TemplateStyle style;

//                if (!string.IsNullOrEmpty(batch?.TimeTableTemplate?.TemplateJson))
//                {
//                    style = System.Text.Json.JsonSerializer.Deserialize<TemplateStyle>(
//                        batch.TimeTableTemplate.TemplateJson
//                    );
//                }
//                else
//                {
//                    style = new TemplateStyle();
//                }

//                var preview = result.Entries.Select(e => new PreviewDto
//                {
//                    Day = e.DayOfWeek,
//                    Slot = e.TimeSlot?.SlotNo ?? 0,

//                    Subject = e.EntryType switch
//                    {
//                        EntryTypeEnum.Break => e.TimeSlot?.BreakRule?.BreakName ?? "Break",
//                        EntryTypeEnum.Free => "Free",
//                        _ => e.SubjectSemester?.Subject?.SubjectName ?? "N/A"
//                    },

//                    Faculty = e.EntryType == EntryTypeEnum.Lecture
//                        ? (e.SubjectSemester?.SubjectFaculties?
//                            .FirstOrDefault(f => f.DivisionId == e.DivisionId)?
//                            .Faculty?.FacultyName ?? "N/A")
//                        : "",

//                    Room = e.Room?.RoomName ?? "N/A",
//                    Division = e.Division?.DivisionName ?? "N/A"
//                }).ToList();

//                var excel = _excel.GenerateExcel(preview, style);

//                return new GenerateResultDto
//                {
//                    Success = true,
//                    Message = "Generated Successfully",
//                    Base64 = Convert.ToBase64String(excel),
//                    Preview = preview,
//                    TemplateStyle = style
//                };
//            }
//            catch (Exception ex)
//            {
//                return new GenerateResultDto
//                {
//                    Success = false,
//                    Message = $"System Error: {ex.Message}"
//                };
//            }
//        }

//    }
//}
//for edit
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Web.DTOs;
using ScheduleX.Web.Models.Template;
using ScheduleX.Web.Services.Excel;

namespace ScheduleX.Web.Services.TimeTable
{
    public class TimeTableService : ITimeTableService
    {
        private readonly AppDbContext _context;
        private readonly ITimetableRepository _repo;
        private readonly IExcelService _excel;

      
        private readonly IViewTimetableRepository _viewRepo;
       

        public TimeTableService(
            ITimetableRepository repo,
            IViewTimetableRepository viewRepo,
            IExcelService excel)
        {
            _repo = repo;
            _viewRepo = viewRepo;
            _excel = excel;
        }
        public async Task<GenerateResultDto> GenerateAsync(GenerateTTDto dto)
        {
            try
            {
                var result = await _repo.GenerateAsync(
                    dto.UserId,
                    dto.AcademicYearId,
                    dto.AcademicTermId,
                    dto.CourseId,
                    dto.TemplateId
                );

                if (!result.Success)
                {
                    return new GenerateResultDto
                    {
                        Success = false,
                        Message = result.Message
                    };
                }

                // IMPORTANT:
                // always reload from DB with Includes
                return await GetPreviewByBatch(result.BatchId);
            }
            catch (Exception ex)
            {
                return new GenerateResultDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        //public async Task<GenerateResultDto> GenerateAsync(GenerateTTDto dto)
        //{
        //    try
        //    {
        //        var result = await _repo.GenerateAsync(
        //            dto.UserId,
        //            dto.AcademicYearId,
        //            dto.AcademicTermId,
        //            dto.CourseId,
        //            dto.TemplateId
        //        );

        //        if (!result.Success || !result.Entries.Any())
        //        {
        //            return new GenerateResultDto
        //            {
        //                Success = false,
        //                Message = result.Message
        //            };
        //        }

        //        return await GetPreviewByBatch(result.BatchId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GenerateResultDto
        //        {
        //            Success = false,
        //            Message = ex.Message
        //        };
        //    }
        //}

        public async Task<GenerateResultDto> GetPreviewByBatch(int batchId)
        {
            try
            {
                var entries = await _viewRepo.GetEntriesByBatch(batchId);
                var batch = await _viewRepo.GetBatchWithTemplate(batchId);

                if (!entries.Any())
                {
                    return new GenerateResultDto
                    {
                        Success = false,
                        Message = "No timetable entries found."
                    };
                }

                var style = GetTemplateStyle(batch);

                var preview = entries
                    .Select(e => new PreviewDto
                    {
                        EntryId = e.EntryId,
                        DayOfWeek = e.DayOfWeek,
                        SlotNo = e.TimeSlot?.SlotNo ?? 0,
                        SlotTime = e.TimeSlot != null
                            ? $"{e.TimeSlot.StartTime:hh\\:mm} - {e.TimeSlot.EndTime:hh\\:mm}"
                            : "",

                        Subject = GetSubjectName(e),
                        Faculty = GetFacultyName(e),
                        Room = e.Room?.RoomName ?? "",
                        Division = e.Division?.DivisionName ?? "",
                        Semester = e.Semester != null
                            ? $"Sem {e.Semester.SemesterNo}"
                            : "",
                        EntryType = e.EntryType.ToString(),

                        SubjectSemesterId = e.SubjectSemesterId,
                        RoomId = e.RoomId,
                        FacultyId = e.FacultyId,
                        BlockId = e.BlockId,
                        BlockPart = e.BlockPart,

                        IsBreak = e.EntryType == EntryTypeEnum.Break,
                        IsProject = IsProject(e),
                        IsSelfStudy = IsSelfStudy(e)
                    })
                    .OrderBy(x => x.Division)
                    .ThenBy(x => x.DayOfWeek)
                    .ThenBy(x => x.SlotNo)
                    .ToList();

                var excelBytes = _excel.GenerateExcel(preview, style);

                return new GenerateResultDto
                {
                    Success = true,
                    Message = "Timetable generated successfully.",
                    BatchId = batchId,
                    Preview = preview,
                    Base64 = Convert.ToBase64String(excelBytes),
                    TemplateStyle = style
                };
            }
            catch (Exception ex)
            {
                return new GenerateResultDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<(bool Success, string Message)> SwapEntriesAsync(SwapEntryDto dto)
        {
            return await _repo.SwapEntriesAsync(
                dto.EntryId1,
                dto.EntryId2,
                dto.UserId
            );
        }

        private string GetSubjectName(TimeTableEntry e)
        {
            if (e.EntryType == EntryTypeEnum.Break)
                return "Break";

            if (e.EntryType == EntryTypeEnum.Free)
            {
                if (IsProject(e))
                    return "Project";

                if (IsSelfStudy(e))
                    return "Self Study";

                return "Free";
            }

            return e.SubjectSemester?.Subject?.SubjectName ?? "Lecture";
        }

        private string GetFacultyName(TimeTableEntry e)
        {
            if (e.EntryType != EntryTypeEnum.Lecture)
                return "";

            return e.SubjectSemester?
                .SubjectFaculties?
                .FirstOrDefault(x => x.DivisionId == e.DivisionId)?
                .Faculty?
                .FacultyName ?? "";
        }

        private bool IsProject(TimeTableEntry e)
        {
            var name = e.SubjectSemester?.Subject?.SubjectName ?? "";

            return name.Contains("project", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("(pw)", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsSelfStudy(TimeTableEntry e)
        {
            return e.EntryType == EntryTypeEnum.Free && !IsProject(e);
        }

        private TemplateStyle GetTemplateStyle(TimeTableBatch? batch)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(batch?.TimeTableTemplate?.TemplateJson))
                {
                    var style = System.Text.Json.JsonSerializer
                        .Deserialize<TemplateStyle>(
    batch.TimeTableTemplate.TemplateJson,
    new System.Text.Json.JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

                    if (style != null)
                        return style;
                }
            }
            catch { }

            return GetDefaultStyle();
        }

        private TemplateStyle GetDefaultStyle()
        {
            return new TemplateStyle
            {
                HeaderBg = "#1e293b",
                HeaderText = "#ffffff",
                BodyBg = "#ffffff",
                BodyText = "#111827",
                BorderColor = "#cbd5e1",
                CellPadding = "8px",
                FontSize = "14px",
                ShowRoom = true,
                ShowFaculty = true,
                TitleAlign = "center"
            };
        }
    }
}