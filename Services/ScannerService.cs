using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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

    public async Task<string?> RunAVScanAsync(string path, bool auto = false)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { path = path, auto = auto }),
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
            var response = await _httpClient.GetAsync($"{_baseUrl}/av/stats");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<string?> GetAVHistoryAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/av/history");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<string?> GetQuarantineAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/av/quarantine");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
        catch { return null; }
    }
}
