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
                

                await _repo.UpdateAsync(model);

                return (true, "Academic Year updated successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }

       
    }
}