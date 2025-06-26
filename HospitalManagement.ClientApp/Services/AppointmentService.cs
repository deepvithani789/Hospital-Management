using System.Text;
using HospitalManagement.ClientApp.Models;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Services
{
    public class AppointmentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public AppointmentService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"Appointment/Doctor/{doctorId}");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Appointment>>(json);
        }


        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync()
        {
            var response = await _httpClient.GetAsync("Appointment");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Appointment>>(json);
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Appointment/{id}");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Appointment>(json);
        }

        public async Task<bool> AddAppointmentAsync(Appointment appointment)
        {
            var json = JsonConvert.SerializeObject(appointment);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Appointment", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            var json = JsonConvert.SerializeObject(appointment);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Appointment/{appointment.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Appointment/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
