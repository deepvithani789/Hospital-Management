using System.Text;
using HospitalManagement.ClientApp.Models;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Services
{
    public class BillingService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public BillingService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
        }

        public async Task<IEnumerable<Billing>> GetBillingsAsync()
        {
            var response = await _httpClient.GetAsync("Billing");
            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Billing>>(json);
        }

        public async Task<Billing> GetBillingByIdAsync(int id)
        {
            var reponse = await _httpClient.GetAsync($"Billing/{id}");
            var json = await reponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Billing>(json);
        }

        public async Task<bool> AddBillingAsync(Billing billing)
        {
            var json = JsonConvert.SerializeObject(billing);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Billing", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBillingAsync(Billing billing)
        {
            var json = JsonConvert.SerializeObject(billing);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Billing/{billing.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBillingAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Billing/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
