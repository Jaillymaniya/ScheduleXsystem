using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces;

namespace ScheduleX.Web.Services.Admin
{
    public class AcademicYearApiService
    {
        private readonly IAcademicYearRepository _repo;

        public AcademicYearApiService(IAcademicYearRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AcademicYear>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<(bool success, string message)> CreateAsync(AcademicYear model)
        {
            try
            {
                // 🔥 AUTO GENERATE YEAR NAME
                model.YearName = $"{model.StartDate.Year}-{model.EndDate.Year % 100}";

                if (model.EndDate <= model.StartDate)
                    return (false, "End date must be after start date");

                await _repo.AddAsync(model);

                return (true, "Academic Year added successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<(bool success, string message)> UpdateAsync(AcademicYear model)
        {
            try
            {
                model.YearName = $"{model.StartDate.Year}-{model.EndDate.Year % 100}";

                await _repo.UpdateAsync(model);

                return (true, "Academic Year updated successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<(bool success, string message)> ToggleAsync(int id)
        {
            try
            {
                await _repo.ToggleStatusAsync(id);
                return (true, "Status updated");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}