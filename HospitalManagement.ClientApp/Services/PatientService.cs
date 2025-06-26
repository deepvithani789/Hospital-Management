using System.Text;
using HospitalManagement.ClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Services
{
    public class PatientService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public PatientService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
        }

        public async Task<IEnumerable<Patient>> GetPatientsAsync()
        {
            var response = await _httpClient.GetAsync("Patient");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Patient>>(json);
        }

        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Patient/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Patient>(json);
            }
            return null;
        }

        public async Task<bool> AddPatientAsync(Patient patient)
        {
            var json = JsonConvert.SerializeObject(patient);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Patient", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            var json = JsonConvert.SerializeObject(patient);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Patient/{patient.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Patient/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
