using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MySqlConnector;
using MonAppMultiplateforme.Models;

namespace MonAppMultiplateforme.Services;

public class DatabaseService
{
    private readonly string _connectionString;
    private static readonly HttpClient _httpClient = new HttpClient();

    public DatabaseService(string host, int port, string database, string user, string password)
    {
        _connectionString = $"Server={host};Port={port};Database={database};User ID={user};Password={password};";
    }

    public async Task<bool> SavePasswordAsync(string password)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO passwords (psswrd) VALUES (@psswrd)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@psswrd", password);

            await command.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving password: {ex.Message}");
            return false;
        }
    }

    public async Task<string?> GenerateAndEncryptPasswordAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("http://127.0.0.1:8000/generate", null);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                return json.RootElement.GetProperty("result").GetString();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating password via API: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> EncryptAndSavePasswordAsync(string password)
    {
        try
        {
            var payload = new { text = password };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var contentString = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://127.0.0.1:8000/encrypt", contentString);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                return json.RootElement.GetProperty("result").GetString();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error encrypting password via API: {ex.Message}");
            return null;
        }
    }

    public async Task<Member?> AddMemberAsync(string fullname, string mail)
    {
        try
        {
            var payload = new { fullname = fullname, mail = mail };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var contentString = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://127.0.0.1:8000/members", contentString);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Member>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding member via API: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Member>> GetMembersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://127.0.0.1:8000/members");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<MemberListResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result?.Members ?? new List<Member>();
            }
            return new List<Member>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching members via API: {ex.Message}");
            return new List<Member>();
        }
    }

    public async Task<List<Member>> SearchMembersAsync(string query)
    {
        try
        {
            var response = await _httpClient.GetAsync($"http://127.0.0.1:8000/members/search?fullname={Uri.EscapeDataString(query)}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<MemberListResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result?.Members ?? new List<Member>();
            }
            return new List<Member>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching members via API: {ex.Message}");
            return new List<Member>();
        }
    }

    public async Task<bool> UpdateMemberAsync(int id, string fullname, string mail)
    {
        try
        {
            var payload = new { fullname = fullname, mail = mail };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var contentString = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"http://127.0.0.1:8000/members/{id}", contentString);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating member via API: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteMemberAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"http://127.0.0.1:8000/members/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting member via API: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendPasswordToMembersAsync(string password, List<int> memberIds)
    {
        try
        {
            var payload = new { password = password, member_ids = memberIds };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var contentString = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://127.0.0.1:8000/members/send-plain-password", contentString);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending password via API: {ex.Message}");
            return false;
        }
    }

    private class MemberListResponse
    {
        public List<Member> Members { get; set; } = new();
        public int Count { get; set; }
    }
}
