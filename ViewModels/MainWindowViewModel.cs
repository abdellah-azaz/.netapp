using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonAppMultiplateforme.Models;
using MonAppMultiplateforme.Services;

namespace MonAppMultiplateforme.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;
    private readonly ScannerService _scannerService;
    private readonly VaultService _vaultService;

    // --- Antivirus Properties ---
    [ObservableProperty] private AVScanReport? _lastAVScanResult;
    [ObservableProperty] private ObservableCollection<AVHistoryEntry> _avHistory = new();
    [ObservableProperty] private ObservableCollection<AVQuarantineEntry> _avQuarantine = new();
    [ObservableProperty] private string _avStatsText = "Chargement...";
    [ObservableProperty] private GlobalAVStats? _globalStats;
    [ObservableProperty] private string _scanTargetPath = string.Empty;
    [ObservableProperty] private bool _isAVScanning = false;
    [ObservableProperty] private int _quarantineBadgeCount = 0;
    [ObservableProperty] private bool _hasQuarantineItems = false;
    [ObservableProperty] private int _bootBadgeCount = 0;
    [ObservableProperty] private bool _hasBootVulnerabilities = false;
    
    // --- Real-time Monitoring Properties ---
    [ObservableProperty] private RealtimeStatus? _currentRealtimeStatus;
    [ObservableProperty] private string _realtimeStatusText = "● INACTIF";
    [ObservableProperty] private string _realtimeStatusColor = "Red";
    [ObservableProperty] private string _realtimeDirectoriesText = "Dossiers : /tmp | Downloads | Desktop";
    [ObservableProperty] private string _realtimeLastEventText = "Aucun événement récent";
    [ObservableProperty] private int _realtimeTodayCount = 0;
    
    // --- Vault Properties ---
    [ObservableProperty] private ObservableCollection<VaultFile> _vaultFiles = new();
    [ObservableProperty] private int _vaultBadgeCount = 0;
    [ObservableProperty] private bool _hasVaultFiles = false;
    [ObservableProperty] private bool _isVaultLoading = false;

    [ObservableProperty]
    private ScanResult? _structuredScanResult;

    [ObservableProperty]
    private bool _isScanning = false;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _currentPage = "Accueil";

    [ObservableProperty]
    private string _encryptedPassword = string.Empty;

    [ObservableProperty]
    private string _memberFullName = string.Empty;

    [ObservableProperty]
    private string _memberEmail = string.Empty;

    [ObservableProperty]
    private bool _isAddingMember = false;

    [ObservableProperty]
    private Member? _editingMember;

    [ObservableProperty]
    private ObservableCollection<Member> _membersList = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isSharePopupVisible = false;

    [ObservableProperty]
    private bool _isMemberSelectionVisible = false;

    [ObservableProperty]
    private ObservableCollection<MemberSelection> _memberSelectionList = new();

    [ObservableProperty]
    private int _criticalRiskCount = 0;

    [ObservableProperty]
    private int _highRiskCount = 0;
    
    [ObservableProperty]
    private int _selectedAVTabIndex = 0;
    
    [ObservableProperty]
    private bool _isRealtimeEventsPopupVisible = false;

    [ObservableProperty]
    private bool _isAIExplanationPopupVisible = false;

    [ObservableProperty]
    private string _aIExplanationText = string.Empty;

    [ObservableProperty]
    private bool _isAIAnalyzing = false;
    
    [ObservableProperty]
    private ObservableCollection<RealtimeEvent> _realtimeEventsList = new();

    [ObservableProperty]
    private int _mediumRiskCount = 0;

    [ObservableProperty]
    private int _lowRiskCount = 0;

    [ObservableProperty] private int _securityScore = 100;
    
    // --- Auth Properties ---
    [ObservableProperty] private bool _isLoggedIn = false;
    [ObservableProperty] private bool _isAuthRequired = true;
    [ObservableProperty] private string _authMode = "Login"; // "Login" or "Signup"
    [ObservableProperty] private string _authEmail = string.Empty;
    [ObservableProperty] private string _authPassword = string.Empty;
    [ObservableProperty] private string _authFullName = string.Empty;
    [ObservableProperty] private string _authTelephone = string.Empty;
    [ObservableProperty] private string _authVerificationCode = string.Empty;
    [ObservableProperty] private bool _isEmailVerificationPending = false;
    [ObservableProperty] private bool _isAuthLoading = false;
    
    // --- Current User Info ---
    [ObservableProperty] private string _currentUserFullName = "Utilisateur";
    [ObservableProperty] private string _currentUserEmail = string.Empty;
    [ObservableProperty] private string _currentUserPhone = string.Empty;
    [ObservableProperty] private bool _isSuperAdmin = false;
    [ObservableProperty] private bool _isLogoutConfirmationVisible = false;

    // --- Password Change Form ---
    [ObservableProperty] private string _oldPassword = string.Empty;
    [ObservableProperty] private string _newPassword = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private bool _isResetMode = false;
    [ObservableProperty] private string _resetEmail = string.Empty;
    [ObservableProperty] private string _resetCode = string.Empty;
    [ObservableProperty] private string _resetNewPassword = string.Empty;
    [ObservableProperty] private string _resetConfirmPassword = string.Empty;

    [ObservableProperty] private string _profileStatusMessage = string.Empty;

    // --- Profile Editing ---
    [ObservableProperty] private bool _isEditingProfile = false;
    [ObservableProperty] private string _editFullName = string.Empty;
    [ObservableProperty] private string _editTelephone = string.Empty;

    private bool _isInitialLoading = false;

    [ObservableProperty] private bool _isRandomPasswordEnabled = true;

    [ObservableProperty] private bool _isEncryptedResultVisible = true;

    [ObservableProperty] private string _scanHistoryCleanupMode = "Jamais";
    public ObservableCollection<string> ScanHistoryCleanupOptions { get; } = new() { "Jamais", "Chaque jour", "Chaque semaine", "Chaque mois" };

    [ObservableProperty] private bool _useCustomRestorePath = false;
    [ObservableProperty] private string _customRestorePath = string.Empty;
    [ObservableProperty] private bool _isAIAnalysisEnabled = true;

    [ObservableProperty] private bool _isRealtimeAnalysisEnabled = true;

    [ObservableProperty] private bool _requirePasswordForDelete = true;
    [ObservableProperty] private bool _requirePasswordForDownload = true;

    partial void OnRequirePasswordForDeleteChanged(bool value)
    {
        if (!_isInitialLoading) _ = SaveUserSettings();
    }

    partial void OnRequirePasswordForDownloadChanged(bool value)
    {
        if (!_isInitialLoading) _ = SaveUserSettings();
    }

    partial void OnIsRealtimeAnalysisEnabledChanged(bool value)
    {
        if (!_isInitialLoading) _ = SaveUserSettings();
    }

    partial void OnIsAIAnalysisEnabledChanged(bool value)
    {
        if (!_isInitialLoading) _ = SaveUserSettings();
    }

    partial void OnScanHistoryCleanupModeChanged(string value)
    {
        if (!_isInitialLoading) _ = SaveUserSettings();
        _ = ApplyHistoryCleanup(value);
    }

    partial void OnIsRandomPasswordEnabledChanged(bool value)
    {
        if (!_isInitialLoading) _ = SaveUserSettings();
    }

    partial void OnIsEncryptedResultVisibleChanged(bool value)
    {
        if (!_isInitialLoading) _ = SaveUserSettings();
    }

    partial void OnUseCustomRestorePathChanged(bool value)
    {
        if (!_isInitialLoading) _ = SaveUserSettings();
    }

    partial void OnCustomRestorePathChanged(string value)
    {
        if (!_isInitialLoading) _ = SaveUserSettings();
    }

    private async Task ApplyHistoryCleanup(string mode)
    {
        int days = mode switch
        {
            "Chaque jour" => 1,
            "Chaque semaine" => 7,
            "Chaque mois" => 30,
            _ => -1 // Jamais ou autre
        };

        if (days >= 0)
        {
            StatusMessage = "Nettoyage de l'historique en cours...";
            bool success = await _scannerService.CleanupAVHistoryAsync(days, CurrentUserEmail);
            if (success)
            {
                StatusMessage = "Historique nettoyé selon vos préférences.";
                if (CurrentPage == "Antivirus") await LoadAVHistory();
            }
        }
    }

    public Func<Task<string?>>? RequestPasswordAction { get; set; }

    private string _lastProcessedPassword = string.Empty;

    public MainWindowViewModel()
    {
        // Credentials provided by user: root / Azaz2003@abdellah
        _databaseService = new DatabaseService("localhost", 3306, "passworddb", "root", "Azaz2003@abdellah");
        _scannerService = new ScannerService("http://127.0.0.1:8000");
        _vaultService = new VaultService();
        
        // Initialiser l'état d'authentification
        _ = InitializeAuthStatus();
        
        // Démarrer le timer de rafraîchissement temps réel
        StartRealtimeTimer();
    }

    private void StartRealtimeTimer()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                try 
                {
                    if (IsLoggedIn && CurrentPage == "Antivirus" && IsRealtimeAnalysisEnabled)
                    {
                        await LoadRealtimeStatus();
                    }
                }
                catch { /* Ignore errors in background timer */ }
                await Task.Delay(10000); // 10 secondes
            }
        });
    }

    private async Task InitializeAuthStatus()
    {
        IsAuthLoading = true;
        bool isConfigured = await _scannerService.CheckAuthAsync();
        AuthMode = isConfigured ? "Login" : "Signup";
        IsAuthLoading = false;
    }

    [RelayCommand]
    private void ToggleLogoutConfirmation()
    {
        IsLogoutConfirmationVisible = !IsLogoutConfirmationVisible;
    }

    [RelayCommand]
    private void Logout()
    {
        IsLoggedIn = false;
        IsAuthRequired = true;
        IsLogoutConfirmationVisible = false;
        
        // Clear session sensitive data
        AuthPassword = string.Empty;
        Password = string.Empty;
        EncryptedPassword = string.Empty;
        StatusMessage = "Vous avez été déconnecté.";
        CurrentPage = "Accueil";
    }

    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(AuthEmail) || string.IsNullOrWhiteSpace(AuthPassword))
        {
            StatusMessage = "Veuillez remplir tous les champs.";
            return;
        }

        IsAuthLoading = true;
        var resultJson = await _scannerService.LoginAsync(AuthEmail, AuthPassword);
        
        try 
        {
            using var doc = JsonDocument.Parse(resultJson ?? "{}");
            bool success = doc.RootElement.GetProperty("success").GetBoolean();
            if (success)
            {
                IsLoggedIn = true;
                IsAuthRequired = false;
                
                // Store user info
                if (doc.RootElement.TryGetProperty("user", out var user))
                {
                    CurrentUserFullName = user.TryGetProperty("fullname", out var fn) ? fn.GetString() ?? "Utilisateur" : "Utilisateur";
                    CurrentUserEmail = user.TryGetProperty("email", out var em) ? em.GetString() ?? string.Empty : string.Empty;
                    CurrentUserPhone = user.TryGetProperty("telephone", out var te) ? te.GetString() ?? string.Empty : string.Empty;
                    IsSuperAdmin = user.TryGetProperty("is_superadmin", out var isa) && (isa.ValueKind == JsonValueKind.True || (isa.ValueKind == JsonValueKind.Number && isa.GetInt32() == 1));
                    
                    // Initialize edit fields
                    EditFullName = CurrentUserFullName;
                    EditTelephone = CurrentUserPhone;
                }

                StatusMessage = string.Empty;
                AuthPassword = string.Empty;
                
                // Load settings
                await LoadUserSettings();
            }
            else
            {
                StatusMessage = doc.RootElement.GetProperty("message").GetString() ?? "Erreur de connexion.";
            }
        }
        catch { StatusMessage = "Erreur lors de la connexion."; }
        
        IsAuthLoading = false;
    }

    private async Task LoadUserSettings()
    {
        if (string.IsNullOrEmpty(CurrentUserEmail)) return;

        _isInitialLoading = true;
        var settings = await _scannerService.GetUserSettingsAsync(CurrentUserEmail);
        if (settings != null)
        {
            IsRandomPasswordEnabled = settings.RandomPasswordEnabled;
            IsEncryptedResultVisible = settings.EncryptedResultVisible;
            ScanHistoryCleanupMode = settings.ScanHistoryCleanupMode;
            UseCustomRestorePath = settings.UseCustomRestorePath;
            CustomRestorePath = settings.CustomRestorePath;
            IsAIAnalysisEnabled = settings.IsAIAnalysisEnabled;
            IsRealtimeAnalysisEnabled = settings.IsRealtimeAnalysisEnabled;
            RequirePasswordForDelete = settings.RequirePasswordForDelete;
            RequirePasswordForDownload = settings.RequirePasswordForDownload;
        }
        _isInitialLoading = false;
    }

    private async Task SaveUserSettings()
    {
        if (string.IsNullOrEmpty(CurrentUserEmail) || _isInitialLoading) return;

        var settings = new UserSettings
        {
            Email = CurrentUserEmail,
            RandomPasswordEnabled = IsRandomPasswordEnabled,
            EncryptedResultVisible = IsEncryptedResultVisible,
            ScanHistoryCleanupMode = ScanHistoryCleanupMode,
            UseCustomRestorePath = UseCustomRestorePath,
            CustomRestorePath = CustomRestorePath,
            IsAIAnalysisEnabled = IsAIAnalysisEnabled,
            IsRealtimeAnalysisEnabled = IsRealtimeAnalysisEnabled,
            RequirePasswordForDelete = RequirePasswordForDelete,
            RequirePasswordForDownload = RequirePasswordForDownload
        };

        await _scannerService.SaveUserSettingsAsync(settings);
    }

    [RelayCommand]
    private async Task Signup()
    {
        if (string.IsNullOrWhiteSpace(AuthEmail) || string.IsNullOrWhiteSpace(AuthPassword) || string.IsNullOrWhiteSpace(AuthFullName))
        {
            StatusMessage = "Veuillez remplir tous les champs.";
            return;
        }

        IsAuthLoading = true;
        
        if (!IsEmailVerificationPending)
        {
            StatusMessage = "Demande de code de vérification en cours...";
            var resultJson = await _scannerService.RequestSignupCodeAsync(AuthFullName, AuthEmail);
            try 
            {
                using var doc = JsonDocument.Parse(resultJson ?? "{}");
                if (doc.RootElement.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    StatusMessage = "Code envoyé ! Veuillez vérifier vos emails.";
                    IsEmailVerificationPending = true;
                }
                else
                {
                    var detailMsg = doc.RootElement.TryGetProperty("message", out var detail) ? detail.GetString() : null;
                    StatusMessage = detailMsg ?? "Erreur lors de la demande de code.";
                }
            }
            catch { StatusMessage = "Erreur lors de la demande de code."; }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(AuthVerificationCode))
            {
                StatusMessage = "Veuillez entrer le code de vérification.";
                IsAuthLoading = false;
                return;
            }

            var resultJson = await _scannerService.SignupAsync(AuthFullName, AuthEmail, AuthPassword, AuthVerificationCode, AuthTelephone);
            try 
            {
                using var doc = JsonDocument.Parse(resultJson ?? "{}");
                if (doc.RootElement.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    StatusMessage = "Inscription réussie ! Veuillez vous connecter.";
                    AuthMode = "Login";
                    AuthPassword = string.Empty;
                    AuthVerificationCode = string.Empty;
                    IsEmailVerificationPending = false;
                }
                else
                {
                    var detailMsg = doc.RootElement.TryGetProperty("message", out var detail) ? detail.GetString() : null;
                    StatusMessage = detailMsg ?? "Erreur d'inscription.";
                }
            }
            catch { StatusMessage = "Erreur lors de l'inscription."; }
        }
        
        IsAuthLoading = false;
    }

    [RelayCommand]
    private void SwitchAuthMode()
    {
        AuthMode = AuthMode == "Login" ? "Signup" : "Login";
        StatusMessage = string.Empty;
        IsEmailVerificationPending = false;
        AuthVerificationCode = string.Empty;
    }

    [RelayCommand]
    private async Task UpdatePassword()
    {
        if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ProfileStatusMessage = "Veuillez remplir tous les champs.";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ProfileStatusMessage = "Les nouveaux mots de passe ne correspondent pas.";
            return;
        }

        IsAuthLoading = true;
        var resultJson = await _scannerService.UpdatePasswordAsync(CurrentUserEmail, OldPassword, NewPassword);
        
        try 
        {
            using var doc = JsonDocument.Parse(resultJson ?? "{}");
            bool success = doc.RootElement.TryGetProperty("success", out var s) && s.GetBoolean();
            ProfileStatusMessage = doc.RootElement.TryGetProperty("message", out var m) ? m.GetString() ?? string.Empty : (success ? "Succès" : "Erreur");
            
            if (success)
            {
                OldPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
            }
        }
        catch { ProfileStatusMessage = "Erreur lors de la mise à jour."; }
        
        IsAuthLoading = false;
    }

    [RelayCommand]
    private void StartEditProfile()
    {
        EditFullName = CurrentUserFullName;
        EditTelephone = CurrentUserPhone;
        IsEditingProfile = true;
    }

    [RelayCommand]
    private void CancelEditProfile()
    {
        IsEditingProfile = false;
        ProfileStatusMessage = string.Empty;
    }

    [RelayCommand]
    private void SwitchToResetMode()
    {
        IsResetMode = true;
        StatusMessage = string.Empty;
        ResetEmail = AuthEmail; // Pré-remplir avec l'email du login
    }

    [RelayCommand]
    private void CancelResetMode()
    {
        IsResetMode = false;
        StatusMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SendForgotCode()
    {
        if (string.IsNullOrWhiteSpace(ResetEmail))
        {
            StatusMessage = "Veuillez entrer votre email.";
            return;
        }

        IsAuthLoading = true;
        StatusMessage = "Envoi du code...";
        var resultJson = await _scannerService.ForgotPasswordAsync(ResetEmail);
        
        if (resultJson != null && resultJson.Contains("\"success\":true"))
        {
            StatusMessage = "Code envoyé ! Vérifiez vos emails.";
        }
        else
        {
            StatusMessage = "Échec de l'envoi du code.";
        }
        IsAuthLoading = false;
    }

    [RelayCommand]
    private async Task ResetWithCode()
    {
        if (string.IsNullOrWhiteSpace(ResetCode) || string.IsNullOrWhiteSpace(ResetNewPassword))
        {
            StatusMessage = "Veuillez remplir tous les champs.";
            return;
        }

        if (ResetNewPassword != ResetConfirmPassword)
        {
            StatusMessage = "Les mots de passe ne correspondent pas.";
            return;
        }

        IsAuthLoading = true;
        StatusMessage = "Réinitialisation...";
        var resultJson = await _scannerService.ResetPasswordAsync(ResetEmail, ResetCode, ResetNewPassword);

        if (resultJson != null && resultJson.Contains("\"success\":true"))
        {
            StatusMessage = "Mot de passe réinitialisé ! Connectez-vous.";
            IsResetMode = false; // Retour au login
        }
        else
        {
            StatusMessage = "Code invalide ou expiré.";
        }
        IsAuthLoading = false;
    }

    [RelayCommand]
    private async Task UpdateProfile()
    {
        if (string.IsNullOrWhiteSpace(EditFullName))
        {
            ProfileStatusMessage = "Le nom ne peut pas être vide.";
            return;
        }

        IsAuthLoading = true;
        var resultJson = await _scannerService.UpdateProfileAsync(CurrentUserEmail, EditFullName, EditTelephone);
        
        try 
        {
            using var doc = JsonDocument.Parse(resultJson ?? "{}");
            bool success = doc.RootElement.TryGetProperty("success", out var s) && s.GetBoolean();
            ProfileStatusMessage = doc.RootElement.TryGetProperty("message", out var m) ? m.GetString() ?? string.Empty : (success ? "Succès" : "Erreur");
            
            if (success)
            {
                if (doc.RootElement.TryGetProperty("user", out var user))
                {
                    CurrentUserFullName = user.TryGetProperty("fullname", out var fn) ? fn.GetString() ?? CurrentUserFullName : CurrentUserFullName;
                    CurrentUserPhone = user.TryGetProperty("telephone", out var te) ? te.GetString() ?? CurrentUserPhone : CurrentUserPhone;
                }
                IsEditingProfile = false;
            }
        }
        catch { ProfileStatusMessage = "Erreur lors de la mise à jour."; }
        
        IsAuthLoading = false;
    }

    [RelayCommand]
    private async Task RunScan()
    {
        IsScanning = true;
        StatusMessage = "Audit de sécurité en cours...";
        // ScanResult property was removed and replaced by StructuredScanResult
        
        var resultJson = await _scannerService.RunScanAsync();
        
        if (resultJson != null && !resultJson.StartsWith("Erreur"))
        {
            try 
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                StructuredScanResult = JsonSerializer.Deserialize<ScanResult>(resultJson, options);
                CalculateRiskMetrics();
                StatusMessage = "Scan terminé avec succès.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Deserialization failed: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                StatusMessage = $"Erreur de lecture : {ex.Message}";
            }
        }
        else
        {
            StatusMessage = resultJson ?? "Le scan a échoué.";
        }
        IsScanning = false;
    }

    private void CalculateRiskMetrics()
    {
        if (StructuredScanResult == null) return;

        CriticalRiskCount = StructuredScanResult.Risques.Count(r => r.Niveau == "CRITIQUE");
        HighRiskCount = StructuredScanResult.Risques.Count(r => r.Niveau == "ÉLEVÉ" || r.Niveau == "ELEVE");
        MediumRiskCount = StructuredScanResult.Risques.Count(r => r.Niveau == "MOYEN");
        // Total count for badge
        BootBadgeCount = CriticalRiskCount + HighRiskCount + MediumRiskCount;
        HasBootVulnerabilities = BootBadgeCount > 0;

        // Basic scoring: Start at 100, -25 per Critical, -15 per High, -5 per Medium
        int score = 100 - (CriticalRiskCount * 25) - (HighRiskCount * 15) - (MediumRiskCount * 5);
        SecurityScore = Math.Max(0, score);
    }

    [RelayCommand]
    private async Task Navigate(string page)
    {
        CurrentPage = page;
        if (page == "Members")
        {
            await LoadMembers();
        }
        else if (page == "Antivirus")
        {
            await RefreshAVData();
        }
        else if (page == "Vault")
        {
            await RefreshVault();
        }
    }

    [RelayCommand]
    private async Task RefreshVault()
    {
        if (string.IsNullOrEmpty(CurrentUserEmail)) return;
        
        IsVaultLoading = true;
        var files = await _vaultService.GetFilesAsync(CurrentUserEmail);
        VaultFiles = new ObservableCollection<VaultFile>(files);
        VaultBadgeCount = files.Count;
        HasVaultFiles = files.Count > 0;
        IsVaultLoading = false;
    }

    [RelayCommand]
    private async Task UploadToVault()
    {
        // Cette commande est maintenant gérée via AddVaultFileButton_Click dans MainWindow.axaml.cs
        // pour permettre l'utilisation du StorageProvider (Sélecteur de fichiers natif).
        await Task.CompletedTask;
    }
    
    public async Task UploadFile(string filePath)
    {
        if (string.IsNullOrEmpty(CurrentUserEmail)) return;
        
        StatusMessage = "Chiffrement et upload en cours...";
        bool success = await _vaultService.UploadFileAsync(filePath, CurrentUserEmail);
        
        if (success)
        {
            StatusMessage = "Fichier ajouté au coffre-fort !";
            await RefreshVault();
        }
        else
        {
            StatusMessage = "Erreur lors de l'upload.";
        }
    }

    [RelayCommand]
    private async Task DownloadVaultFile(VaultFile file)
    {
        if (file == null || string.IsNullOrEmpty(CurrentUserEmail)) return;
        
        string? password = null;
        if (RequirePasswordForDownload)
        {
            if (RequestPasswordAction == null) return;
            password = await RequestPasswordAction();
            if (string.IsNullOrEmpty(password)) return;

            StatusMessage = "Vérification de l'identité...";
            bool isVerified = await _vaultService.VerifyPasswordAsync(CurrentUserEmail, password);
            if (!isVerified)
            {
                StatusMessage = "Mot de passe incorrect. Action annulée.";
                return;
            }
        }

        StatusMessage = $"Téléchargement de {file.filename}...";
        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        if (!Directory.Exists(downloadsPath)) downloadsPath = Path.GetTempPath();
        
        string destPath = Path.Combine(downloadsPath, "decrypted_" + file.filename);
        
        bool success = await _vaultService.DownloadFileAsync(file.file_id, destPath, CurrentUserEmail);
        if (success)
            StatusMessage = $"Fichier enregistré dans : {destPath}";
        else
            StatusMessage = "Erreur lors du téléchargement.";
    }

    [RelayCommand]
    private async Task DeleteVaultFile(VaultFile file)
    {
        if (file == null || string.IsNullOrEmpty(CurrentUserEmail)) return;
        
        if (RequirePasswordForDelete)
        {
            if (RequestPasswordAction == null) return;
            var password = await RequestPasswordAction();
            if (string.IsNullOrEmpty(password)) return;

            StatusMessage = "Vérification de l'identité...";
            bool isVerified = await _vaultService.VerifyPasswordAsync(CurrentUserEmail, password);
            if (!isVerified)
            {
                StatusMessage = "Mot de passe incorrect. Action annulée.";
                return;
            }
        }

        StatusMessage = $"Suppression de {file.filename}...";
        bool success = await _vaultService.DeleteFileAsync(file.file_id, CurrentUserEmail);
        if (success)
        {
            StatusMessage = "Fichier supprimé.";
            await RefreshVault();
        }
        else
        {
            StatusMessage = "Erreur de suppression.";
        }
    }

    [RelayCommand]
    private async Task RefreshAVData()
    {
        await Task.WhenAll(LoadAVStats(), LoadAVHistory(), LoadAVQuarantine(), LoadRealtimeStatus());
    }

    [RelayCommand]
    private async Task OpenRealtimeEvents()
    {
        await LoadRealtimeStatus();
        IsRealtimeEventsPopupVisible = true;
    }

    [RelayCommand]
    private void CloseRealtimeEvents()
    {
        IsRealtimeEventsPopupVisible = false;
    }

    private async Task LoadRealtimeStatus()
    {
        var status = await _scannerService.GetRealtimeStatusAsync();
        if (status != null)
        {
            CurrentRealtimeStatus = status;
            RealtimeStatusText = status.Status == "ACTIVE" ? "● ACTIVE" : "● INACTIVE";
            RealtimeStatusColor = status.Status == "ACTIVE" ? "LimeGreen" : "Red";
            RealtimeDirectoriesText = string.Join("  |  ", status.WatchedDirectories);
            RealtimeTodayCount = status.TodayCount;
            
            if (status.Events != null)
            {
                RealtimeEventsList = new ObservableCollection<RealtimeEvent>(status.Events);
                if (status.Events.Count > 0)
                {
                    var latest = status.Events[0];
                    string icon = latest.Result == "MALWARE" ? "⚠️" : "✅";
                    string time = latest.Timestamp.Split(' ').Last().Substring(0, 5); // HH:mm
                    RealtimeLastEventText = $"{icon} {latest.FileName} — {time}";
                }
            }
        }
    }

    private async Task LoadAVStats()
    {
        var json = await _scannerService.GetAVStatsAsync();
        if (json != null)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var stats = JsonSerializer.Deserialize<GlobalAVStats>(json, options);
                if (stats != null)
                {
                    GlobalStats = stats;
                    AvStatsText = GlobalStats.RawOutput;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to parse global stats: {ex.Message}");
                AvStatsText = json; // Fallback
            }
        }
    }

    private async Task LoadAVHistory()
    {
        var json = await _scannerService.GetAVHistoryAsync(CurrentUserEmail);
        if (json != null)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var history = JsonSerializer.Deserialize<List<AVHistoryEntry>>(json, options);
            if (history != null) AvHistory = new ObservableCollection<AVHistoryEntry>(history);
        }
    }

    private async Task LoadAVQuarantine()
    {
        var json = await _scannerService.GetQuarantineAsync(CurrentUserEmail);
        if (json != null)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<List<AVQuarantineEntry>>(json, options);
            if (items != null)
            {
                AvQuarantine = new ObservableCollection<AVQuarantineEntry>(items);
                QuarantineBadgeCount = items.Count;
                HasQuarantineItems = items.Count > 0;
            }
        }
    }

    [RelayCommand]
    private async Task RunAVScan()
    {
        if (string.IsNullOrWhiteSpace(ScanTargetPath))
        {
            StatusMessage = "Veuillez entrer un chemin à scanner.";
            return;
        }

        IsAVScanning = true;
        StatusMessage = "Scan AV-Shield en cours...";
        
        var resultJson = await _scannerService.RunAVScanAsync(ScanTargetPath, CurrentUserEmail);
        
        if (resultJson != null && !resultJson.StartsWith("Erreur"))
        {
            try 
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var root = JsonDocument.Parse(resultJson);
                if (root.RootElement.TryGetProperty("report", out var reportElem) && reportElem.ValueKind != JsonValueKind.Null)
                {
                    var reportJson = reportElem.GetRawText();
                    Console.WriteLine($"DEBUG: Received Report JSON: {reportJson}");
                    
                    LastAVScanResult = JsonSerializer.Deserialize<AVScanReport>(reportJson, options);
                    
                    if (LastAVScanResult != null)
                    {
                        Console.WriteLine($"DEBUG: Deserialized Report - Malwares: {LastAVScanResult.Statistics.MalwareFiles}, Files Count: {LastAVScanResult.Files?.Count}");
                        StatusMessage = $"Scan terminé. Menaces : {LastAVScanResult.Statistics.MalwareFiles}";
                    }
                    else
                    {
                        StatusMessage = "Erreur : Échec de la désérialisation du rapport.";
                    }
                    await RefreshAVDataCommand.ExecuteAsync(null);
                }
                else
                {
                    StatusMessage = "Attention : Aucun rapport généré (le fichier n'existe peut-être plus).";
                    Console.WriteLine("DEBUG: No report found in response or report is null.");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur de parsing : {ex.Message}";
            }
        }
        else
        {
            StatusMessage = resultJson ?? "Le scan a échoué.";
        }
        IsAVScanning = false;
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = SearchMembers(value);
    }

    [RelayCommand]
    public async Task LoadMembers()
    {
        StatusMessage = "Chargement des membres...";
        var members = await _databaseService.GetMembersAsync(CurrentUserEmail);
        MembersList = new ObservableCollection<Member>(members);
        StatusMessage = $"Pret - {members.Count} membres.";
    }

    private async Task SearchMembers(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            await LoadMembers();
            return;
        }

        var results = await _databaseService.SearchMembersAsync(query, CurrentUserEmail);
        MembersList = new ObservableCollection<Member>(results);
    }

    [RelayCommand]
    private async Task DeleteMember(Member member)
    {
        if (member == null) return;

        StatusMessage = $"Suppression de {member.Fullname}...";
        bool success = await _databaseService.DeleteMemberAsync(member.Id, CurrentUserEmail);

        if (success)
        {
            MembersList.Remove(member);
            StatusMessage = "Membre supprimé.";
        }
        else
        {
            StatusMessage = "Erreur lors de la suppression.";
        }
    }

    [RelayCommand]
    private async Task UpdateMember(Member member)
    {
        if (member == null) return;

        EditingMember = member;
        MemberFullName = member.Fullname;
        MemberEmail = member.Mail;
        IsAddingMember = true;
        StatusMessage = $"Modification de {member.Fullname}";
    }

    private async Task ConfirmUpdateMember()
    {
        if (EditingMember == null) return;

        if (string.IsNullOrWhiteSpace(MemberFullName) || string.IsNullOrWhiteSpace(MemberEmail))
        {
            StatusMessage = "Veuillez remplir tous les champs.";
            return;
        }

        StatusMessage = "Mise à jour...";
        bool success = await _databaseService.UpdateMemberAsync(EditingMember.Id, MemberFullName, MemberEmail, CurrentUserEmail);

        if (success)
        {
            EditingMember.Fullname = MemberFullName;
            EditingMember.Mail = MemberEmail;
            
            // Forcer le rafraîchissement du DataGrid (ObservableCollection ne voit pas les changements de propriétés internes)
            int index = MembersList.IndexOf(EditingMember);
            if (index != -1)
            {
                MembersList[index] = null!; // Reset temporaire
                MembersList[index] = EditingMember;
            }

            StatusMessage = "Membre mis à jour !";
            ToggleAddMemberForm();
        }
        else
        {
            StatusMessage = "Erreur lors de la mise à jour.";
        }
    }

    [RelayCommand]
    private void ToggleAddMemberForm()
    {
        IsAddingMember = !IsAddingMember;
        if (!IsAddingMember)
        {
            MemberFullName = string.Empty;
            MemberEmail = string.Empty;
            EditingMember = null;
        }
    }

    [RelayCommand]
    private async Task SaveMember()
    {
        if (EditingMember != null)
        {
            await ConfirmUpdateMember();
        }
        else
        {
            await AddMember();
        }
    }

    [RelayCommand]
    private async Task AddMember()
    {
        if (string.IsNullOrWhiteSpace(MemberFullName) || string.IsNullOrWhiteSpace(MemberEmail))
        {
            StatusMessage = "Veuillez remplir tous les champs.";
            return;
        }

        StatusMessage = "Ajout du membre...";
        var newMember = await _databaseService.AddMemberAsync(MemberFullName, MemberEmail, CurrentUserEmail);

        if (newMember != null)
        {
            StatusMessage = $"Membre {newMember.Fullname} ajouté avec succès !";
            MembersList.Add(newMember); // Ajout immédiat au tableau
            MemberFullName = string.Empty;
            MemberEmail = string.Empty;
            IsAddingMember = false;
        }
        else
        {
            StatusMessage = "Erreur lors de l'ajout du membre.";
        }
    }

    [RelayCommand]
    private async Task GeneratePassword()
    {
        StatusMessage = "Génération en cours...";
        var result = await _databaseService.GenerateAndEncryptPasswordAsync(CurrentUserEmail);
        
        if (result != null)
        {
            EncryptedPassword = result;
            // Note: L'API /generate actuelle retourne le chiffré.
            // On le stocke pour le partage (même si c'est le chiffré, l'API send-password rechiffrera... 
            // ou on peut adapter l'API si besoin. Ici on suit la demande UI).
            _lastProcessedPassword = result; 
            StatusMessage = "Mot de passe généré et chiffré avec succès !";
            IsSharePopupVisible = true; 
        }
        else
        {
            StatusMessage = "Erreur lors de la communication avec l'API.";
        }
    }

    [RelayCommand]
    private async Task ValidatePassword()
    {
        if (string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "Veuillez saisir un mot de passe.";
            return;
        }

        StatusMessage = "Traitement en cours...";
        var result = await _databaseService.EncryptAndSavePasswordAsync(Password, CurrentUserEmail);
        
        if (result != null)
        {
            EncryptedPassword = result;
            _lastProcessedPassword = result; // Garder le chiffré pour partage via send-plain-password
            StatusMessage = "Mot de passe chiffré et enregistré avec succès !";
            Password = string.Empty;
            IsSharePopupVisible = true;
        }
        else
        {
            StatusMessage = "Erreur lors de la communication avec l'API.";
        }
    }

    [RelayCommand]
    private void NoShare()
    {
        IsSharePopupVisible = false;
        _lastProcessedPassword = string.Empty;
    }

    [RelayCommand]
    private async Task YesShare()
    {
        IsSharePopupVisible = false;
        StatusMessage = "Chargement des membres pour le partage...";
        
        var members = await _databaseService.GetMembersAsync(CurrentUserEmail);
        MemberSelectionList = new ObservableCollection<MemberSelection>(
            members.Select(m => new MemberSelection(m))
        );
        
        IsMemberSelectionVisible = true;
    }

    [RelayCommand]
    private void CancelSharing()
    {
        IsMemberSelectionVisible = false;
        _lastProcessedPassword = string.Empty;
    }

    [RelayCommand]
    private async Task SendSharing()
    {
        var selectedIds = MemberSelectionList
            .Where(ms => ms.IsSelected)
            .Select(ms => ms.Member.Id)
            .ToList();

        if (selectedIds.Count == 0)
        {
            StatusMessage = "Veuillez sélectionner au moins un membre.";
            return;
        }

        StatusMessage = "Partage en cours...";
        bool success = await _databaseService.SendPasswordToMembersAsync(_lastProcessedPassword, selectedIds);

        if (success)
        {
            StatusMessage = "Partage réussi avec succès !";
            IsMemberSelectionVisible = false;
            _lastProcessedPassword = string.Empty;
        }
        else
        {
            StatusMessage = "Erreur lors du partage.";
        }
    }

    [RelayCommand]
    private async Task RestoreQuarantineItem(AVQuarantineEntry entry)
    {
        if (entry == null) return;
        
        if (string.IsNullOrWhiteSpace(entry.QuarantineName))
        {
            StatusMessage = "Impossible de restaurer ce fichier (données de quarantaine incomplètes).";
            return;
        }

        string? destination = UseCustomRestorePath ? CustomRestorePath : null;
        bool success = await _scannerService.RestoreQuarantineAsync(entry.QuarantineName, CurrentUserEmail, destination);
        
        if (success)
        {
            StatusMessage = $"Fichier {entry.Filename} restauré avec succès.";
            await RefreshAVDataCommand.ExecuteAsync(null);
        }
        else
        {
            StatusMessage = $"Échec de la restauration de {entry.Filename}.";
        }
    }

    [RelayCommand]
    private async Task DeleteQuarantineItem(AVQuarantineEntry entry)
    {
        if (entry == null) return;
        
        if (string.IsNullOrWhiteSpace(entry.QuarantineName))
        {
            StatusMessage = "Impossible de supprimer ce fichier (données de quarantaine incomplètes).";
            return;
        }

        bool success = await _scannerService.DeleteQuarantineAsync(entry.QuarantineName, CurrentUserEmail);
        if (success)
        {
            StatusMessage = $"Fichier {entry.Filename} supprimé définitivement.";
            await RefreshAVDataCommand.ExecuteAsync(null);
        }
        else
        {
            StatusMessage = $"Échec de la suppression de {entry.Filename}.";
        }
    }

    [RelayCommand]
    private async Task ExplainDetection(AVFileReport file)
    {
        if (file == null) return;
        
        IsAIAnalyzing = true;
        AIExplanationText = "Analyse en cours par l'IA (Llama 3.3)...";
        IsAIExplanationPopupVisible = true;
        
        try
        {
            var explanation = await _scannerService.ExplainDetectionAsync(
                file.Filename, 
                file.Result, 
                file.Threat, 
                file.HeuristicScore, 
                file.Entropy
            );
            
            if (!string.IsNullOrEmpty(explanation))
            {
                AIExplanationText = explanation;
            }
            else
            {
                AIExplanationText = "Désolé, l'IA n'a pas pu générer d'explication pour le moment.";
            }
        }
        catch (Exception ex)
        {
            AIExplanationText = $"Erreur lors de l'analyse : {ex.Message}";
        }
        finally
        {
            IsAIAnalyzing = false;
        }
    }

    [RelayCommand]
    private void CloseAIExplanation()
    {
        IsAIExplanationPopupVisible = false;
        AIExplanationText = string.Empty;
    }
}
