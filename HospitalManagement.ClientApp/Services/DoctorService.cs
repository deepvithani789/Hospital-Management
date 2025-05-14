using System.Text;
using HospitalManagement.ClientApp.Models;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Services
{
    public class DoctorService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public DoctorService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsAsync()
        {
            var response = await _httpClient.GetAsync("Doctor");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Doctor>>(json);
        }

        public async Task<Doctor> GetDoctorByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Doctor/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Doctor>(json);
            }
            return null;
        }

        public async Task<bool> AddDoctorAsync(Doctor doctor)
        {
            var json = JsonConvert.SerializeObject(doctor);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Doctor", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            var json = JsonConvert.SerializeObject(doctor);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Doctor/{doctor.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDoctor(int id)
        {
            var response = await _httpClient.DeleteAsync($"Doctor/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleAvailabilityAsync(int id)
        {
            var response = await _httpClient.PatchAsync($"Doctor/{id}/toggle-availability", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Doctor>> GetAvailableDoctorsAsync()
        {
            var response = await _httpClient.GetAsync("Doctor/AvailableDoctors");
            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Doctor>>();
            }
            return new List<Doctor>();
        }
    }
}
