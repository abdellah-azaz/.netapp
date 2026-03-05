using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonAppMultiplateforme.Models;
using MonAppMultiplateforme.Services;

namespace MonAppMultiplateforme.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;

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

    private string _lastProcessedPassword = string.Empty;

    public MainWindowViewModel()
    {
        // Credentials provided by user: root / Azaz2003@abdellah
        _databaseService = new DatabaseService("localhost", 3306, "passworddb", "root", "Azaz2003@abdellah");
    }

    [RelayCommand]
    private async Task Navigate(string page)
    {
        CurrentPage = page;
        if (page == "Members")
        {
            await LoadMembers();
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = SearchMembers(value);
    }

    [RelayCommand]
    public async Task LoadMembers()
    {
        StatusMessage = "Chargement des membres...";
        var members = await _databaseService.GetMembersAsync();
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

        var results = await _databaseService.SearchMembersAsync(query);
        MembersList = new ObservableCollection<Member>(results);
    }

    [RelayCommand]
    private async Task DeleteMember(Member member)
    {
        if (member == null) return;

        StatusMessage = $"Suppression de {member.Fullname}...";
        bool success = await _databaseService.DeleteMemberAsync(member.Id);

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
        bool success = await _databaseService.UpdateMemberAsync(EditingMember.Id, MemberFullName, MemberEmail);

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
        var newMember = await _databaseService.AddMemberAsync(MemberFullName, MemberEmail);

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
        var result = await _databaseService.GenerateAndEncryptPasswordAsync();
        
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
        var result = await _databaseService.EncryptAndSavePasswordAsync(Password);
        
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
        
        var members = await _databaseService.GetMembersAsync();
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
}
