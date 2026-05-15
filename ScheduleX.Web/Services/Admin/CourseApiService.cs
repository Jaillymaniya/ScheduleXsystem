using System.Net.Http.Json;
using ScheduleX.Core.Entities;
using ScheduleX.Web.DTOs;

namespace ScheduleX.Web.Services.Admin;

public class CourseApiService
{
    private readonly HttpClient _http;

    public CourseApiService(HttpClient http)
    {
        _http = http;
    }

    // ✅ GET returns DTO list
    public async Task<List<CourseDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<CourseDto>>("api/admin/course") ?? new();

    public async Task<List<CourseDto>> GetByDepartmentAsync(int departmentId)
        => await _http.GetFromJsonAsync<List<CourseDto>>($"api/admin/course/by-department/{departmentId}") ?? new();

    // ✅ POST/PUT still send Course entity (fine)
    //public async Task CreateAsync(Course course)
    //    => await _http.PostAsJsonAsync("api/admin/course", course);
    public async Task CreateAsync(CourseCreateDto dto)
    {
        var res = await _http.PostAsJsonAsync("api/admin/course", dto);
        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            //throw new Exception($"Create failed ({(int)res.StatusCode}): {body}");
            throw new Exception(body);
    }

    //public async Task UpdateAsync(Course course)
    //    => await _http.PutAsJsonAsync($"api/admin/course/{course.CourseId}", course);

    //public async Task UpdateAsync(Course course)
    //{
    //    var res = await _http.PutAsJsonAsync($"api/admin/course/{course.CourseId}", course);
    //    var body = await res.Content.ReadAsStringAsync();

    //    if (!res.IsSuccessStatusCode)
    //        throw new Exception($"Update failed ({(int)res.StatusCode}): {body}");
    //}

    public async Task UpdateAsync(int id, CourseUpdateDto dto)
    {
        var res = await _http.PutAsJsonAsync(
            $"api/admin/course/{id}",
            dto);

        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception(body);
    }

    public async Task ToggleAsync(int id)
        => await _http.PatchAsync($"api/admin/course/{id}", null);
}