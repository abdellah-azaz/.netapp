using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MonAppMultiplateforme.Models;

namespace MonAppMultiplateforme.Services
{
    public class AuthResult
    {
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
    }

    public class VaultService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://127.0.0.1:8000";

        public VaultService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> UploadFileAsync(string filePath, string email)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(File.OpenRead(filePath));
                content.Add(fileContent, "file", Path.GetFileName(filePath));

                var response = await _httpClient.PostAsync($"{_baseUrl}/vault/encrypt?email={Uri.EscapeDataString(email)}", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<VaultFile>> GetFilesAsync(string email)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<VaultFile>>($"{_baseUrl}/vault/list/{Uri.EscapeDataString(email)}");
                return response ?? new List<VaultFile>();
            }
            catch
            {
                return new List<VaultFile>();
            }
        }

        public async Task<bool> DownloadFileAsync(string fileId, string destinationPath, string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/vault/decrypt/{fileId}?email={Uri.EscapeDataString(email)}");
                if (!response.IsSuccessStatusCode) return false;

                using var fs = new FileStream(destinationPath, FileMode.Create);
                await response.Content.CopyToAsync(fs);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> VerifyPasswordAsync(string email, string password)
        {
            try
            {
                var cleanEmail = email?.Trim();
                var cleanPassword = password?.Trim();
                
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/auth/login", new { email = cleanEmail, password = cleanPassword });
                if (!response.IsSuccessStatusCode) return false;
                
                var result = await response.Content.ReadFromJsonAsync<AuthResult>();
                return result != null && result.success;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileId, string email)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/vault/delete/{fileId}?email={Uri.EscapeDataString(email)}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
