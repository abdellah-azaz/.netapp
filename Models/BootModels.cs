using System;
using System.Text.Json.Serialization;

namespace MonAppMultiplateforme.Models;

public class BootHistoryEntry
{
    [JsonPropertyName("scan_id")] public string ScanId { get; set; } = string.Empty;
    [JsonPropertyName("filename")] public string Filename { get; set; } = string.Empty;
    [JsonPropertyName("created_at")] public string CreatedAt { get; set; } = string.Empty;
}
