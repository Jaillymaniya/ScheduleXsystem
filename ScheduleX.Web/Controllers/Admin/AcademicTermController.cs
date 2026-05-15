using Microsoft.AspNetCore.Mvc;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.Admin;
using ScheduleX.Web.DTOs;


namespace ScheduleX.Web.Controllers.Admin
{

    [ApiController]
    [Route("api/admin/academicterm")]
    public class AcademicTermController : ControllerBase
    {
        private readonly IAcademicTermRepository _repo;

        public AcademicTermController(IAcademicTermRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var data = await _repo.GetByCourseAsync(courseId);

            var result = data.Select(x => new AcademicTermDto
            {
                AcademicTermId = x.AcademicTermId,
                CourseId = x.CourseId,
                CourseName = x.Course.CourseName ?? "",
                AcademicYearName = x.AcademicYear.YearName ?? "",
                TermType = x.TermType,
                SemesterPattern = x.SemesterPattern,
                Status = x.Status,
                IsCurrent = x.IsCurrent,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AcademicTermCreateDto dto)
        {
            try
            {
                var term = new AcademicTerm
                {
                    CourseId = dto.CourseId,
                    TermType = dto.TermType,
                    SemesterPattern = dto.SemesterPattern,
                    Status = dto.Status,
                    IsCurrent = dto.IsCurrent,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate
                };

                await _repo.AddAsync(term);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AcademicTermUpdateDto dto)
        {
            try
            {
                var term = new AcademicTerm
                {
                    AcademicTermId = id,
                    CourseId = dto.CourseId,
                    TermType = dto.TermType,
                    SemesterPattern = dto.SemesterPattern,
                    Status = dto.Status,
                    IsCurrent = dto.IsCurrent,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate
                };

                await _repo.UpdateAsync(term);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}




