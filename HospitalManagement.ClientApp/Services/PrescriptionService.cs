using System.Text;
using HospitalManagement.ClientApp.Models;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Services
{
    public class PrescriptionService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public PrescriptionService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsAsync()
        {
            var response = await _httpClient.GetAsync("Prescription");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Prescription>>(json);
        }

        public async Task<Prescription> GetPrescriptionById(int id)
        {
            var response = await _httpClient.GetAsync($"Prescription/{id}");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Prescription>(json);
        }

        public async Task<bool> AddPrescriptionAsync(Prescription prescription)
        {
            var json = JsonConvert.SerializeObject(prescription);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Prescription", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdatePrescriptionAsync(Prescription prescription)
        {
            var json = JsonConvert.SerializeObject(prescription);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Prescription/{prescription.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePrescriptionAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Prescription/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
