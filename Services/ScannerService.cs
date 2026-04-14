using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MonAppMultiplateforme.Models;

namespace MonAppMultiplateforme.Services;

public class ScannerService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ScannerService(string baseUrl = "http://127.0.0.1:8000")
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task<string?> RunScanAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/scanner");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return $"Erreur API: {response.StatusCode}";
        }
        catch (Exception ex)
        {
            return $"Erreur de connexion: {ex.Message}";
        }
    }

    public async Task<string?> RunAVScanAsync(string path, string email, bool auto = false)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { path = path, owner_email = email, auto = auto }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/scannerav", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return $"Erreur API: {response.StatusCode}";
        }
        catch (Exception ex)
        {
            return $"Erreur de connexion: {ex.Message}";
        }
    }

    public async Task<string?> GetAVStatsAsync()
    {
        try
        {
            return await _httpClient.GetStringAsync($"{_baseUrl}/av/stats");
        }
        catch { return null; }
    }

    public async Task<bool> RestoreQuarantineAsync(string filename, string email, string? destination = null)
    {
        try
        {
            var url = $"{_baseUrl}/av/quarantine/restore/{Uri.EscapeDataString(filename)}?email={Uri.EscapeDataString(email)}";
            if (!string.IsNullOrEmpty(destination))
            {
                url += $"&destination={Uri.EscapeDataString(destination)}";
            }
            var response = await _httpClient.PostAsync(url, null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteQuarantineAsync(string filename, string email)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/av/quarantine/delete/{Uri.EscapeDataString(filename)}?email={Uri.EscapeDataString(email)}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<string?> GetAVHistoryAsync(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/av/history?email={Uri.EscapeDataString(email)}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<string?> GetQuarantineAsync(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/av/quarantine?email={Uri.EscapeDataString(email)}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<bool> CheckAuthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/auth/check");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var root = JsonDocument.Parse(json);
                return root.RootElement.GetProperty("configured").GetBoolean();
            }
            return false;
        }
        catch { return false; }
    }

    public async Task<string?> RequestSignupCodeAsync(string fullname, string email)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { fullname, email }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/request-signup-code", content);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex) { return $"Error: {ex.Message}"; }
    }

    public async Task<string?> SignupAsync(string fullname, string email, string password, string code, string? telephone = null)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { fullname, email, password, telephone, code }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/signup", content);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex) { return $"Error: {ex.Message}"; }
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { email, password }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/login", content);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex) { return $"Error: {ex.Message}"; }
    }

    public async Task<string?> UpdatePasswordAsync(string email, string oldPassword, string newPassword)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { email = email, old_password = oldPassword, new_password = newPassword }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/update-password", content);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex) { return $"Error: {ex.Message}"; }
    }

    public async Task<string?> UpdateProfileAsync(string email, string fullname, string telephone)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { email = email, fullname = fullname, telephone = telephone }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/update-profile", content);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex) { return $"Error: {ex.Message}"; }
    }

    public async Task<string?> ForgotPasswordAsync(string email)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { email = email }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/forgot-password", content);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex) { return $"Error: {ex.Message}"; }
    }

    public async Task<string?> ResetPasswordAsync(string email, string code, string newPassword)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { email = email, code = code, new_password = newPassword }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/reset-password", content);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex) { return $"Error: {ex.Message}"; }
    }

    public async Task<bool> CleanupAVHistoryAsync(int days, string email)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/av/history/cleanup/{days}?email={Uri.EscapeDataString(email)}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<UserSettings?> GetUserSettingsAsync(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/settings/{Uri.EscapeDataString(email)}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserSettings>(json);
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<bool> SaveUserSettingsAsync(UserSettings settings)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(settings),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/settings", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<RealtimeStatus?> GetRealtimeStatusAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/realtime-events");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<RealtimeStatus>(json, options);
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<string?> ExplainDetectionAsync(string filename, string result, string? threatName, int heuristicScore, double entropy)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { 
                    filename = filename, 
                    result = result, 
                    threat_name = threatName, 
                    heuristic_score = heuristicScore, 
                    entropy = entropy 
                }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/av/explain", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var root = JsonDocument.Parse(json);
                return root.RootElement.GetProperty("explanation").GetString();
            }
            return null;
        }
        catch { return null; }
    }

    // --- SSH Remote Scan Methods ---

    public async Task<string?> TestSSHConnectionAsync(string host, int port, string username, string password)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { host, port, username, password }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/ssh/test", content);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex) { return JsonSerializer.Serialize(new { success = false, message = $"Erreur de connexion: {ex.Message}" }); }
    }

    public async Task<string?> RunSSHScanAsync(string host, int port, string username, string password)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { host, port, username, password }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/ssh/scan", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return $"Erreur API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
        }
        catch (Exception ex) { return $"Erreur de connexion: {ex.Message}"; }
    }

    public async Task<string?> RunSSHAVScanAsync(string host, int port, string username, string password, string scanPath)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { host, port, username, password, scan_path = scanPath }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_baseUrl}/ssh/scanav", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return $"Erreur API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
        }
        catch (Exception ex) { return $"Erreur de connexion: {ex.Message}"; }
    }
}
