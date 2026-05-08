using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator;

public interface IScheduleConfigRepository
{
    // =========================================
    // CONFIGURATION
    // =========================================

    Task<ScheduleConfig?> GetConfigAsync(
        int academicYearId,
        int courseId);

    Task<ScheduleConfig> SaveAsync(
        ScheduleConfig model);

    // =========================================
    // BREAK RULES
    // =========================================

    Task<List<BreakRule>> GetBreakRulesAsync(
        int configId);

    Task<BreakRule> AddBreakRuleAsync(
        BreakRule model);

    Task<BreakRule?> UpdateBreakRuleAsync(
        BreakRule model);

    Task<bool> DeleteBreakRuleAsync(
        int breakRuleId);

    // =========================================
    // TIME SLOTS
    // =========================================

    Task<List<TimeSlot>> GetTimeSlotsAsync(
        int configId);

    Task<List<TimeSlot>> GenerateTimeSlotsAsync(
        int configId);

    // =========================================
    // TEMPLATES
    // =========================================

    Task<List<TimeTableTemplate>> GetTemplatesAsync();
}
