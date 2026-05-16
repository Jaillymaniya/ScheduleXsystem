using ClosedXML.Excel;
using ScheduleX.Web.DTOs;
using ScheduleX.Web.Models.Template;

namespace ScheduleX.Web.Services.Excel
{
    public class ExcelService : IExcelService
    {
        public byte[] GenerateExcel(List<PreviewDto> data, TemplateStyle style)
        {
            using var wb = new XLWorkbook();

            var grouped = data
                .GroupBy(x => $"{x.Semester}-{x.Division}")
                .ToList();

            foreach (var group in grouped)
            {
                var groupData = group.ToList();

                var first = groupData.First();

                var sheetName =
                    $"{first.Semester}-{first.Division}";

                if (sheetName.Length > 31)
                    sheetName = sheetName.Substring(0, 31);

                var ws = wb.Worksheets.Add(sheetName);

                var days = groupData
                    .Select(x => x.DayOfWeek)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var slots = groupData
                    .Select(x => x.SlotNo)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                // TITLE
                ws.Cell(1, 1).Value =
                    $"TIME TABLE - {first.Semester} - {first.Division}";

                ws.Range(1, 1, 1, days.Count + 1).Merge();

                ws.Cell(1, 1).Style.Fill.BackgroundColor =
                    XLColor.FromHtml(style.HeaderBg);

                ws.Cell(1, 1).Style.Font.FontColor =
                    XLColor.FromHtml(style.HeaderText);

                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 16;

                ws.Cell(1, 1).Style.Alignment.Horizontal =
                    GetAlignment(style.TitleAlign);

                ws.Cell(1, 1).Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

                // HEADER
                ws.Cell(2, 1).Value = "Time";

                for (int i = 0; i < days.Count; i++)
                {
                    ws.Cell(2, i + 2).Value =
                        GetDayName(days[i]);
                }

                var headerRange =
                    ws.Range(2, 1, 2, days.Count + 1);

                headerRange.Style.Fill.BackgroundColor =
                    XLColor.FromHtml(style.HeaderBg);

                headerRange.Style.Font.FontColor =
                    XLColor.FromHtml(style.HeaderText);

                headerRange.Style.Font.Bold = true;

                headerRange.Style.Alignment.Horizontal =
                    GetAlignment(style.TitleAlign);

                headerRange.Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

                headerRange.Style.Border.OutsideBorder =
                    XLBorderStyleValues.Thin;

                headerRange.Style.Border.InsideBorder =
                    XLBorderStyleValues.Thin;

                headerRange.Style.Border.OutsideBorderColor =
                    XLColor.FromHtml(style.BorderColor);

                headerRange.Style.Border.InsideBorderColor =
                    XLColor.FromHtml(style.BorderColor);

                int row = 3;

                foreach (var slot in slots)
                {
                    var slotItem = groupData
                        .FirstOrDefault(x => x.SlotNo == slot);

                    ws.Cell(row, 1).Value =
                        slotItem?.SlotTime ?? $"Slot {slot}";

                    ws.Cell(row, 1).Style.Fill.BackgroundColor =
                        XLColor.FromHtml(style.BodyBg);

                    ws.Cell(row, 1).Style.Font.FontColor =
                        XLColor.FromHtml(style.BodyText);

                    ws.Cell(row, 1).Style.Font.Bold = true;

                    ws.Cell(row, 1).Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 1).Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                    for (int i = 0; i < days.Count; i++)
                    {
                        var day = days[i];

                        var item = groupData.FirstOrDefault(x =>
                            x.DayOfWeek == day &&
                            x.SlotNo == slot);

                        var cell = ws.Cell(row, i + 2);

                        ApplyBaseCellStyle(cell, style);

                        if (item == null)
                        {
                            cell.Value = "Free";
                            cell.Style.Fill.BackgroundColor =
                                XLColor.FromHtml("#f8fafc");

                            cell.Style.Font.FontColor =
                                XLColor.FromHtml("#64748b");

                            continue;
                        }

                        if (item.IsBreak)
                        {
                            cell.Value = "Break";

                            cell.Style.Fill.BackgroundColor =
                                XLColor.FromHtml("#fde68a");

                            cell.Style.Font.FontColor =
                                XLColor.Black;

                            cell.Style.Font.Bold = true;

                            continue;
                        }

                        var content = item.Subject;

                        if (style.ShowFaculty &&
                            !string.IsNullOrWhiteSpace(item.Faculty))
                        {
                            content +=
                                Environment.NewLine +
                                item.Faculty;
                        }

                        if (style.ShowRoom &&
                            !string.IsNullOrWhiteSpace(item.Room))
                        {
                            content +=
                                Environment.NewLine +
                                item.Room;
                        }

                        cell.Value = content;
                    }

                    row++;
                }

                ws.Columns().AdjustToContents();

                foreach (var column in ws.Columns())
                {
                    if (column.Width < 20)
                        column.Width = 20;
                }

                ws.Rows().AdjustToContents();
            }

            using var ms = new MemoryStream();

            wb.SaveAs(ms);

            return ms.ToArray();
        }

        private void ApplyBaseCellStyle(
            IXLCell cell,
            TemplateStyle style)
        {
            cell.Style.Fill.BackgroundColor =
                XLColor.FromHtml(style.BodyBg);

            cell.Style.Font.FontColor =
                XLColor.FromHtml(style.BodyText);

            cell.Style.Alignment.WrapText = true;

            cell.Style.Alignment.Horizontal =
                GetAlignment(style.TitleAlign);

            cell.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            cell.Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            cell.Style.Border.OutsideBorderColor =
                XLColor.FromHtml(style.BorderColor);

            if (!string.IsNullOrWhiteSpace(style.FontSize))
            {
                var px = style.FontSize
                    .Replace("px", "")
                    .Trim();

                if (int.TryParse(px, out int size))
                {
                    cell.Style.Font.FontSize = size;
                }
            }
        }

        private XLAlignmentHorizontalValues GetAlignment(string? align)
        {
            return align?.ToLower() switch
            {
                "left" => XLAlignmentHorizontalValues.Left,
                "right" => XLAlignmentHorizontalValues.Right,
                _ => XLAlignmentHorizontalValues.Center
            };
        }

        private string GetDayName(int day)
        {
            return day switch
            {
                1 => "Monday",
                2 => "Tuesday",
                3 => "Wednesday",
                4 => "Thursday",
                5 => "Friday",
                6 => "Saturday",
                7 => "Sunday",
                _ => ""
            };
        }
    }
}