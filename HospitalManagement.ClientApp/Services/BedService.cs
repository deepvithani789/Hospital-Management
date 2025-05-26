using System.Text;
using HospitalManagement.ClientApp.Models;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Services
{
    public class BedService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public BedService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
        }

        public async Task<List<Bed>> GetBedsAsync()
        {
            var response = await _httpClient.GetAsync("Bed");
            var content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Bed>>(content);
        }

        public async Task<Bed> GetBedByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Bed/{id}");
            var content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Bed>(content);
        }

        public async Task<bool> AddAsync(Bed bed)
        {
            var json = JsonConvert.SerializeObject(bed);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Bed", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Bed bed)
        {
            var json = JsonConvert.SerializeObject(bed);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Bed/{bed.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Bed/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AssignToPatientAsync(int bedId, int patientId)
        {
            var assignDto = new { BedId = bedId, PatientId = patientId };
            var json = JsonConvert.SerializeObject(assignDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Bed/assign", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReleaseForPatient(int patientId)
        {
            var assignDto = new { PatientId = patientId };
            var json = JsonConvert.SerializeObject(assignDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"Bed/release/{patientId}", content);
            return response.IsSuccessStatusCode;
        }
    }
}
