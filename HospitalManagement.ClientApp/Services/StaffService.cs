using System.Text;
using HospitalManagement.ClientApp.Models;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Services
{
    public class StaffService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public StaffService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
        }

        public async Task<IEnumerable<Staff>> GetStaffsAsync()
        {
            var response = await _httpClient.GetAsync("Staff");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Staff>>(json);
        }

        public async Task<Staff> GetStaffById(int id)
        {
            var response = await _httpClient.GetAsync($"Staff/{id}");
            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Staff>(json);
            }
            return null;
        }

        public async Task<bool> AddStaffAsync(Staff staff)
        {
            var json = JsonConvert.SerializeObject(staff);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Staff", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStaffAsync(Staff staff)
        {
            var json = JsonConvert.SerializeObject(staff);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Staff/{staff.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Staff/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
