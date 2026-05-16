using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.Admin
{
    public interface IAcademicTermRepository
    {

        Task<List<AcademicTerm>> GetByCourseAsync(int courseId);

        Task AddAsync(AcademicTerm term);

        Task UpdateAsync(AcademicTerm term);

        //Task UpdateAsync(int id);
    }
}
