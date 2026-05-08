using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Web.Services.TT
{
    public class ScheduleConfigService
    {
        private readonly AppDbContext _context;

        public ScheduleConfigService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================
        // GET CONFIG
        // =========================================

        public async Task<ScheduleConfig?> GetConfigAsync(
            int academicYearId,
            int courseId)
        {
            return await _context.ScheduleConfigs
                .Include(x => x.BreakRules.OrderBy(x => x.BreakNo))
                .Include(x => x.TimeSlots.OrderBy(x => x.SlotNo))
                .FirstOrDefaultAsync(x =>
                    x.AcademicYearId == academicYearId &&
                    x.CourseId == courseId &&
                    x.IsActive);
        }

        // =========================================
        // SAVE CONFIG
        // =========================================

        public async Task<ScheduleConfig> SaveAsync(
            ScheduleConfig model)
        {
            var existing = await _context.ScheduleConfigs
                .FirstOrDefaultAsync(x =>
                    x.AcademicYearId == model.AcademicYearId &&
                    x.CourseId == model.CourseId &&
                    x.IsActive);

            if (existing == null)
            {
                _context.ScheduleConfigs.Add(model);
            }
            else
            {
                existing.StartTime = model.StartTime;
                existing.EndTime = model.EndTime;
                existing.LectureDurationMin =
                    model.LectureDurationMin;

                existing.WorkingDaysMask =
                    model.WorkingDaysMask;

                existing.LecturesPerDay =
                    model.LecturesPerDay;

                existing.DepartmentId =
                    model.DepartmentId;

                existing.IsActive =
                    model.IsActive;

                model = existing;
            }

            await _context.SaveChangesAsync();

            return model;
        }

        // =========================================
        // BREAK RULES
        // =========================================

        public async Task<List<BreakRule>> GetBreakRulesAsync(
            int configId)
        {
            return await _context.BreakRules
                .Where(x => x.ConfigId == configId)
                .OrderBy(x => x.BreakNo)
                .ToListAsync();
        }

        public async Task<BreakRule> AddBreakRuleAsync(
            BreakRule model)
        {
            _context.BreakRules.Add(model);

            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<BreakRule?> UpdateBreakRuleAsync(
            BreakRule model)
        {
            var existing = await _context.BreakRules
                .FirstOrDefaultAsync(x =>
                    x.BreakRuleId == model.BreakRuleId);

            if (existing == null)
                return null;

            existing.BreakNo = model.BreakNo;

            existing.BreakName = model.BreakName;

            existing.AfterLectureNo =
                model.AfterLectureNo;

            existing.BreakDurationMin =
                model.BreakDurationMin;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteBreakRuleAsync(
            int breakRuleId)
        {
            var existing = await _context.BreakRules
                .FirstOrDefaultAsync(x =>
                    x.BreakRuleId == breakRuleId);

            if (existing == null)
                return false;

            _context.BreakRules.Remove(existing);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================
        // GET SLOTS
        // =========================================

        public async Task<List<TimeSlot>> GetTimeSlotsAsync(
            int configId)
        {
            return await _context.TimeSlots
                .Include(x => x.BreakRule)
                .Where(x => x.ConfigId == configId)
                .OrderBy(x => x.SlotNo)
                .ToListAsync();
        }

        // =========================================
        // GENERATE SLOTS
        // =========================================

        public async Task<List<TimeSlot>> GenerateTimeSlotsAsync(
            int configId)
        {
            var config = await _context.ScheduleConfigs
                .Include(x => x.BreakRules)
                .FirstOrDefaultAsync(x =>
                    x.ConfigId == configId);

            if (config == null)
                throw new Exception("Configuration not found.");

            var breaks = config.BreakRules
                .OrderBy(x => x.AfterLectureNo)
                .ThenBy(x => x.BreakNo)
                .ToList();

            var totalLectureMinutes =
                config.LecturesPerDay *
                config.LectureDurationMin;

            var totalBreakMinutes =
                breaks.Sum(x => x.BreakDurationMin);

            var availableMinutes =
                (int)(config.EndTime.ToTimeSpan() -
                config.StartTime.ToTimeSpan()).TotalMinutes;

            if (totalLectureMinutes + totalBreakMinutes >
                availableMinutes)
            {
                throw new Exception(
                    "Time range is insufficient.");
            }

            // DELETE OLD SLOTS

            var oldSlots = _context.TimeSlots
                .Where(x => x.ConfigId == configId);

            _context.TimeSlots.RemoveRange(oldSlots);

            await _context.SaveChangesAsync();

            // GENERATE

            var slots = new List<TimeSlot>();

            var currentTime = config.StartTime;

            byte slotNo = 1;

            for (byte lectureNo = 1;
                 lectureNo <= config.LecturesPerDay;
                 lectureNo++)
            {
                var lectureEnd =
                    currentTime.AddMinutes(
                        config.LectureDurationMin);

                slots.Add(new TimeSlot
                {
                    ConfigId = config.ConfigId,
                    SlotNo = slotNo++,
                    StartTime = currentTime,
                    EndTime = lectureEnd,
                    SlotType = SlotTypeEnum.Lecture
                });

                currentTime = lectureEnd;

                var breaksAfterLecture = breaks
                    .Where(x =>
                        x.AfterLectureNo == lectureNo)
                    .OrderBy(x => x.BreakNo)
                    .ToList();

                foreach (var br in breaksAfterLecture)
                {
                    var breakEnd =
                        currentTime.AddMinutes(
                            br.BreakDurationMin);

                    slots.Add(new TimeSlot
                    {
                        ConfigId = config.ConfigId,
                        SlotNo = slotNo++,
                        StartTime = currentTime,
                        EndTime = breakEnd,
                        SlotType = SlotTypeEnum.Break,
                        BreakRuleId = br.BreakRuleId
                    });

                    currentTime = breakEnd;
                }
            }

            _context.TimeSlots.AddRange(slots);

            await _context.SaveChangesAsync();

            return await GetTimeSlotsAsync(configId);
        }

        public async Task<List<TimeSlot>> GenerateSlotsAsync(int configId)
{
    var config = await _context.ScheduleConfigs
        .Include(x => x.BreakRules)
        .FirstOrDefaultAsync(x => x.ConfigId == configId);

    if (config == null)
        return new List<TimeSlot>();

    var slots = new List<TimeSlot>();

    var start = config.StartTime;
    var lectureDuration = config.LectureDurationMin;
    int slotNo = 1;

    var breakRules = config.BreakRules
        .OrderBy(x => x.AfterLectureNo)
        .ToList();

    for (int i = 1; i <= config.LecturesPerDay; i++)
    {
        var end = start.Add(TimeSpan.FromMinutes(lectureDuration));

        slots.Add(new TimeSlot
        {
            ConfigId = config.ConfigId,
            SlotNo = slotNo++,
            StartTime = start,
            EndTime = end,
            SlotType = SlotTypeEnum.Lecture
        });

        start = end;

        var breakRule = breakRules.FirstOrDefault(b => b.AfterLectureNo == i);

        if (breakRule != null)
        {
            var breakEnd = start.Add(TimeSpan.FromMinutes(breakRule.BreakDurationMin));

            slots.Add(new TimeSlot
            {
                ConfigId = config.ConfigId,
                SlotNo = slotNo++,
                StartTime = start,
                EndTime = breakEnd,
                SlotType = SlotTypeEnum.Break,
                BreakRuleId = breakRule.BreakRuleId
            });

            start = breakEnd;
        }
    }

    return slots;
}
    }
}

