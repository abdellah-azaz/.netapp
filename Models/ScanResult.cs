using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonAppMultiplateforme.Models;

// --- RISKS ---
public class ScanRisk
{
    [JsonPropertyName("niveau")] public string Niveau { get; set; } = string.Empty;
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    public string Icon => Niveau switch
    {
        "CRITIQUE" => "M13,14H11V10H13M13,18H11V16H13M1,21H23L12,2L1,21Z",
        "ÉLEVÉ" or "ELEVE" => "M11,15H13V17H11V15M11,7H13V13H11V7M12,2C6.47,2 2,6.5 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20Z",
        _ => "M11,9H13V11H11V9M11,13H13V17H11V13M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,20C7.59,20 4,16.41 4,12C4,7.59 7.59,4 12,4C16.41,4 20,7.59 20,12C20,16.41 16.41,20 12,20Z"
    };

    public string Color => Niveau switch
    {
        "CRITIQUE" => "#e74c3c",
        "ÉLEVÉ" or "ELEVE" => "#e67e22",
        "MOYEN" => "#f1c40f",
        _ => "#3498db"
    };
}

// --- SYSTEM ---
public class ScanSystemInfo
{
    [JsonPropertyName("hostname")] public string Hostname { get; set; } = string.Empty;
    [JsonPropertyName("os")] public string OS { get; set; } = string.Empty;
    [JsonPropertyName("kernel")] public string Kernel { get; set; } = string.Empty;
    [JsonPropertyName("distro")] public string Distro { get; set; } = string.Empty;
    [JsonPropertyName("windows_version")] public string WindowsVersion { get; set; } = string.Empty;
    [JsonPropertyName("ram_gb")] public float RamGb { get; set; }
    [JsonPropertyName("cpu_cores")] public int CpuCores { get; set; }
    [JsonPropertyName("architecture")] public string Architecture { get; set; } = string.Empty;
    [JsonPropertyName("est_root")] public bool? EstRoot { get; set; }
    [JsonPropertyName("est_admin")] public bool? EstAdmin { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; } = string.Empty;

    public string DisplayOS => !string.IsNullOrEmpty(Distro) ? Distro : $"Windows {WindowsVersion}";
}

// --- ACCOUNTS ---
public class UserAccount
{
    [JsonPropertyName("nom")] public string Nom { get; set; } = string.Empty;
    [JsonPropertyName("actif")] public bool? Actif { get; set; }
    [JsonPropertyName("mdp_expire_jamais")] public bool? MdpExpireJamais { get; set; }
}

public class PasswordPolicy
{
    [JsonPropertyName("longueur_minimale")] public int? LongueurMinimale { get; set; }
    [JsonPropertyName("complexite")] public bool? Complexite { get; set; }
    [JsonPropertyName("seuil_verrouillage")] public string? SeuilVerrouillage { get; set; }
}

public class ScanAccounts
{
    [JsonPropertyName("nombre_admins")] public int? NombreAdmins { get; set; }
    [JsonPropertyName("utilisateurs")] public List<UserAccount> Utilisateurs { get; set; } = new();
    [JsonPropertyName("uid0")] public List<string>? Uid0 { get; set; }
    [JsonPropertyName("shadow_permissions")] public string? ShadowPermissions { get; set; }
    [JsonPropertyName("sudoers")] public List<string>? Sudoers { get; set; }
    [JsonPropertyName("administrateurs")] public List<string>? Administrateurs { get; set; }
    [JsonPropertyName("politique_mdp")] public PasswordPolicy? PolitiqueMdp { get; set; }
}

// --- NETWORK ---
public class NetworkShare
{
    [JsonPropertyName("nom")] public string Nom { get; set; } = string.Empty;
    [JsonPropertyName("chemin")] public string Chemin { get; set; } = string.Empty;
}

public class ExposedPort
{
    [JsonPropertyName("port")] public int Port { get; set; }
    [JsonPropertyName("service")] public string Service { get; set; } = string.Empty;
    [JsonPropertyName("risque")] public string Risque { get; set; } = string.Empty;
}

public class ScanNetwork
{
    [JsonPropertyName("smb1_active")] public bool? Smb1 { get; set; }
    [JsonPropertyName("samba_active")] public bool? SambaActive { get; set; }
    [JsonPropertyName("rdp_active")] public bool? Rdp { get; set; }
    [JsonPropertyName("parefeu_desactive")] public bool? ParefeuDesactive { get; set; }
    [JsonPropertyName("ufw_status")] public string? UfwStatus { get; set; }
    [JsonPropertyName("ports_ouverts")] public List<ExposedPort> PortsOuverts { get; set; } = new();
    [JsonPropertyName("ports_ecoute")] public List<int>? PortsEcoute { get; set; }
    [JsonPropertyName("partages_reseau")] public List<NetworkShare> Partages { get; set; } = new();
}

// --- SECURITY & SOFTWARE ---
public class DefenderInfo
{
    [JsonPropertyName("protection_temps_reel")] public bool? ProtectionTempsReel { get; set; }
    [JsonPropertyName("age_signatures_jours")] public int? AgeSignatures { get; set; }
    [JsonPropertyName("antivirus_actif")] public bool? AntivirusActif { get; set; }
}

public class ScanSecurity
{
    [JsonPropertyName("uac_active")] public bool? Uac { get; set; }
    [JsonPropertyName("bitlocker_actif")] public bool? Bitlocker { get; set; }
    [JsonPropertyName("windows_update_actif")] public bool? WinUpdate { get; set; }
    [JsonPropertyName("ssh_permit_root")] public string? SshRoot { get; set; }
    [JsonPropertyName("ssh_password_auth")] public string? SshPasswordAuth { get; set; }
    [JsonPropertyName("apparmor_status")] public string? AppArmor { get; set; }
    [JsonPropertyName("selinux_status")] public string? SelinuxStatus { get; set; }
    [JsonPropertyName("lsass_protege")] public bool? LsassProtege { get; set; }
    [JsonPropertyName("ps_execution_policy")] public string? PsExecutionPolicy { get; set; }
    [JsonPropertyName("defender")] public DefenderInfo? Defender { get; set; }
}

public class ScanSoftware
{
    [JsonPropertyName("total_paquets")] public int? TotalPaquets { get; set; }
    [JsonPropertyName("maj_securite_pendantes")] public int? MajSecurity { get; set; }
}

// --- PERSISTENCE & LOGS ---
public class UnquotedService
{
    [JsonPropertyName("service")] public string Service { get; set; } = string.Empty;
    [JsonPropertyName("chemin")] public string Chemin { get; set; } = string.Empty;
}

public class AutorunEntry
{
    [JsonPropertyName("nom")] public string Nom { get; set; } = string.Empty;
    [JsonPropertyName("valeur")] public string Valeur { get; set; } = string.Empty;
    [JsonPropertyName("suspect")] public bool? Suspect { get; set; }
}

public class ScanPersistence
{
    [JsonPropertyName("services_actifs")] public int? ServicesActifs { get; set; }
    [JsonPropertyName("services_running_top")] public List<string>? ServicesRunningTop { get; set; }
    [JsonPropertyName("cron_jobs")] public List<string>? CronJobs { get; set; }
    [JsonPropertyName("cron_files")] public List<string>? CronFiles { get; set; }
    [JsonPropertyName("autorun_suspects")] public List<AutorunEntry>? Autoruns { get; set; }
    [JsonPropertyName("unquoted_services")] public List<UnquotedService>? UnquotedServices { get; set; }
    [JsonPropertyName("taches_suspectes")] public List<string>? TachesSuspectes { get; set; }
}

public class ScanLogs
{
    [JsonPropertyName("echecs_connexion")] public int EchecsConnexion { get; set; }
    [JsonPropertyName("nouveaux_comptes")] public int NewAccounts { get; set; }
    [JsonPropertyName("utilisation_privileges")] public int? UtilisationPrivileges { get; set; }
    [JsonPropertyName("changements_politique")] public int? ChangementsPolitique { get; set; }
}

// --- EXTERNAL ---
public class ScanExternal
{
    [JsonPropertyName("ip_publique")] public string IpPublique { get; set; } = string.Empty;
    [JsonPropertyName("ports_exposes")] public List<ExposedPort> PortsExposes { get; set; } = new();
    [JsonPropertyName("nombre_exposes")] public int NombreExposes { get; set; }
}

// --- MAIN RESULT ---
public class ScanResult
{
    [JsonPropertyName("date_scan")] public string DateScan { get; set; } = string.Empty;
    [JsonPropertyName("systeme")] public ScanSystemInfo Systeme { get; set; } = new();
    [JsonPropertyName("comptes")] public ScanAccounts Comptes { get; set; } = new();
    [JsonPropertyName("reseau")] public ScanNetwork Reseau { get; set; } = new();
    [JsonPropertyName("ports_externes")] public ScanExternal? PortsExternes { get; set; }
    [JsonPropertyName("securite")] public ScanSecurity Securite { get; set; } = new();
    [JsonPropertyName("logiciels")] public ScanSoftware Logiciels { get; set; } = new();
    [JsonPropertyName("persistence")] public ScanPersistence Persistence { get; set; } = new();
    [JsonPropertyName("logs")] public ScanLogs Logs { get; set; } = new();
    [JsonPropertyName("risques")] public List<ScanRisk> Risques { get; set; } = new();
}
