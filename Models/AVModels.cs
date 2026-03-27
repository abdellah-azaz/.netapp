using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonAppMultiplateforme.Models;

public class AVFileReport
{
    [JsonPropertyName("filename")] public string Filename { get; set; } = string.Empty;
    [JsonPropertyName("filepath")] public string Filepath { get; set; } = string.Empty;
    [JsonPropertyName("filesize")] public long Filesize { get; set; }
    [JsonPropertyName("sha256")] public string Sha256 { get; set; } = string.Empty;
    [JsonPropertyName("result")] public string Result { get; set; } = string.Empty;
    [JsonPropertyName("threat")] public string Threat { get; set; } = string.Empty;
    [JsonPropertyName("heuristic_score")] public int HeuristicScore { get; set; }
    [JsonPropertyName("entropy")] public double Entropy { get; set; }
    [JsonPropertyName("quarantined")] public bool Quarantined { get; set; }
}

public class AVScanStats
{
    [JsonPropertyName("total_files")] public int TotalFiles { get; set; }
    [JsonPropertyName("clean_files")] public int CleanFiles { get; set; }
    [JsonPropertyName("suspicious_files")] public int SuspiciousFiles { get; set; }
    [JsonPropertyName("malware_files")] public int MalwareFiles { get; set; }
}

public class AVScanReport
{
    [JsonPropertyName("report_id")] public string ReportId { get; set; } = string.Empty;
    [JsonPropertyName("scan_target")] public string TargetPath { get; set; } = string.Empty;
    [JsonPropertyName("statistics")] public AVScanStats Statistics { get; set; } = new();
    [JsonPropertyName("scan_duration")] public double ScanDuration { get; set; }
    [JsonPropertyName("generated_at")] public string ScanDate { get; set; } = string.Empty;
    [JsonPropertyName("files")] public List<AVFileReport> Files { get; set; } = new();
}

public class AVHistoryEntry
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("scan_id")] public string ScanId { get; set; } = string.Empty;
    [JsonPropertyName("target_path")] public string TargetPath { get; set; } = string.Empty;
    [JsonPropertyName("total_files")] public int TotalFiles { get; set; }
    [JsonPropertyName("clean_files")] public int CleanFiles { get; set; }
    [JsonPropertyName("suspicious_files")] public int SuspiciousFiles { get; set; }
    [JsonPropertyName("malware_files")] public int MalwareFiles { get; set; }
    [JsonPropertyName("scan_duration")] public double ScanDuration { get; set; }
    [JsonPropertyName("scan_date")] public string ScanDate { get; set; } = string.Empty;
}

public class AVQuarantineEntry
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("original_path")] public string OriginalPath { get; set; } = string.Empty;
    [JsonPropertyName("quarantine_path")] public string QuarantinePath { get; set; } = string.Empty;
    [JsonPropertyName("original_name")] public string Filename { get; set; } = string.Empty;
    [JsonPropertyName("quarantine_name")] public string QuarantineName { get; set; } = string.Empty;
    [JsonPropertyName("sha256")] public string FileHash { get; set; } = string.Empty;
    [JsonPropertyName("threat_name")] public string ThreatName { get; set; } = string.Empty;
    [JsonPropertyName("quarantine_date")] public string QuarantineDate { get; set; } = string.Empty;
}

public class GlobalAVStats
{
    [JsonPropertyName("total_scans")] public int TotalScans { get; set; }
    [JsonPropertyName("threats_detected")] public int ThreatsDetected { get; set; }
    [JsonPropertyName("in_quarantine")] public int InQuarantine { get; set; }
    [JsonPropertyName("raw_output")] public string RawOutput { get; set; } = string.Empty;
}

public class RealtimeEvent
{
    [JsonPropertyName("filepath")] public string FilePath { get; set; } = string.Empty;
    [JsonPropertyName("filename")] public string FileName { get; set; } = string.Empty;
    [JsonPropertyName("result")] public string Result { get; set; } = string.Empty;
    [JsonPropertyName("threat")] public string Threat { get; set; } = string.Empty;
    [JsonPropertyName("timestamp")] public string Timestamp { get; set; } = string.Empty;
}

public class RealtimeStatus
{
    [JsonPropertyName("status")] public string Status { get; set; } = "INACTIVE";
    [JsonPropertyName("watched_directories")] public List<string> WatchedDirectories { get; set; } = new();
    [JsonPropertyName("events")] public List<RealtimeEvent> Events { get; set; } = new();
    [JsonPropertyName("today_count")] public int TodayCount { get; set; }
}

public class AIExplainResponse
{
    [JsonPropertyName("explanation")] public string Explanation { get; set; } = string.Empty;
}
