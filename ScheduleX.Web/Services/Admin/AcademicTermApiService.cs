using System.Net.Http.Json;
using ScheduleX.Web.DTOs;

namespace ScheduleX.Web.Services.Admin
{
    public class AcademicTermApiService
    {
        private readonly HttpClient _http;

        public AcademicTermApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<AcademicTermDto>> GetByCourseAsync(int courseId)
        {
            return await _http.GetFromJsonAsync<List<AcademicTermDto>>
                ($"api/admin/academicterm/by-course/{courseId}")
                ?? new();
        }

        public async Task CreateAsync(AcademicTermCreateDto dto)
        {
            var res = await _http.PostAsJsonAsync(
                "api/admin/academicterm",
                dto);

            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception(body);
        }

        public async Task UpdateAsync(int id, AcademicTermUpdateDto dto)
        {
            var res = await _http.PutAsJsonAsync(
                $"api/admin/academicterm/{id}",
                dto);

            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception(body);
        }
    }
}
