using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.Admin
{
    public interface IFacultyRepository
    {
        Task<List<Faculty>> GetAllAsync();
        Task AddAsync(Faculty faculty);
        Task UpdateAsync(Faculty faculty);
        Task DeleteAsync(int id);

        Task ToggleStatusAsync(int id);
    }
}
