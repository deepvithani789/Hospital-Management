using System.Text;
using HospitalManagement.ClientApp.Models;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Services
{
    public class PharmacyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public PharmacyService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
        }

        public async Task<List<MedicineInventoryViewModel>> GetMedicinesAsync()
        {
            var response = await _httpClient.GetAsync("Pharmacy/medicines");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<MedicineInventoryViewModel>>(json);
        }

        public async Task<MedicineInventoryViewModel> GetMedicineByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Pharmacy/medicines/{id}");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<MedicineInventoryViewModel>(json);
        }

        public async Task<bool> AddMedicineAsync(MedicineInventoryViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Pharmacy/medicines", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateMedicineAsync(MedicineInventoryViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Pharmacy/medicines/{model.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<MedicineInventoryViewModel>> LowStockAsync()
        {
            var response = await _httpClient.GetAsync("Pharmacy/alert/low-stocks");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<MedicineInventoryViewModel>>(json);
        }

        public async Task<List<MedicineInventoryViewModel>> Expired()
        {
            var response = await _httpClient.GetAsync("Pharmacy/alert/expired");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<MedicineInventoryViewModel>>(json);
        }

        public async Task<string> DispenseMedicineAsync(DispenseMedicineRequestDto model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Pharmacy/dispense", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode ? responseContent : $"Error: {responseContent}";
        }

        public async Task<List<DispensedMedicineViewModel>> GetDispensedMedicinesAsync()
        {
            var response = await _httpClient.GetAsync("Pharmacy/dispensed");
            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<List<DispensedMedicineViewModel>>(json);
        }

    }
}
