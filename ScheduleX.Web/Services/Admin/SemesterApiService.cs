using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.Admin;


namespace ScheduleX.Web.Services.Admin
{
    public class SemesterApiService
    {
        private readonly ISemesterRepository _repo;

        public SemesterApiService(ISemesterRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Semester>> GetAll()
        {
            try { return await _repo.GetAllAsync(); }
            catch { return new(); }
        }

        public async Task<List<Semester>> GetByCourse(int courseId)
        {
            try { return await _repo.GetByCourseAsync(courseId); }
            catch { return new(); }
        }

        public async Task<(bool, string)> Create(Semester model)
        {
            try
            {
                await _repo.AddAsync(model);
                return (true, "Semester added successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> Update(Semester model)
        {
            try
            {
                await _repo.UpdateAsync(model);
                return (true, "Semester updated successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        //public async Task<(bool, string)> Toggle(int id)
        //{
        //    try
        //    {
        //        await _repo.ToggleStatusAsync(id);
        //        return (true, "Status updated");
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}
        public async Task<(bool, string)> Toggle(int id)
        {
            try
            {
                var semester = await _repo.GetByIdAsync(id);

                if (semester == null)
                    return (false, "Semester not found");

                bool wasActive = semester.IsActive;

                await _repo.ToggleStatusAsync(id);

                return wasActive
                    ? (true, "Semester deactivated")
                    : (true, "Semester activated");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }


        public async Task<List<Course>> GetCourses()
        {
            try
            {
                return await _repo.GetAllCoursesAsync();
            }
            catch
            {
                return new();
            }
        }

        public async Task<(bool, string)> GenerateAll(int courseId, int maxSem)
        {
            try
            {
                var existing = await _repo.GetByCourseAsync(courseId);

                var existingNos = existing.Select(x => x.SemesterNo).ToList();

                int count = 0;

                for (int i = 1; i <= maxSem; i++)
                {
                    if (!existingNos.Contains(i))
                    {
                        await _repo.AddAsync(new Semester
                        {
                            CourseId = courseId,
                            SemesterNo = i,
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        });

                        count++;
                    }
                }

                if (count == 0)
                    return (true, "All semesters already exist");

                return (true, $"{count} semesters created");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}


