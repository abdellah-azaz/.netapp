using System.Text.Json.Serialization;

namespace MonAppMultiplateforme.Models;

public class UserSettings
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("random_password_enabled")]
    public bool RandomPasswordEnabled { get; set; } = true;

    [JsonPropertyName("encrypted_result_visible")]
    public bool EncryptedResultVisible { get; set; } = true;

    [JsonPropertyName("scan_history_cleanup_mode")]
    public string ScanHistoryCleanupMode { get; set; } = "Jamais";

    [JsonPropertyName("use_custom_restore_path")]
    public bool UseCustomRestorePath { get; set; } = false;

    [JsonPropertyName("custom_restore_path")]
    public string CustomRestorePath { get; set; } = string.Empty;

    [JsonPropertyName("is_ai_analysis_enabled")]
    public bool IsAIAnalysisEnabled { get; set; } = true;

    [JsonPropertyName("is_realtime_analysis_enabled")]
    public bool IsRealtimeAnalysisEnabled { get; set; } = true;

    [JsonPropertyName("require_password_for_delete")]
    public bool RequirePasswordForDelete { get; set; } = true;

    [JsonPropertyName("require_password_for_download")]
    public bool RequirePasswordForDownload { get; set; } = true;
}
