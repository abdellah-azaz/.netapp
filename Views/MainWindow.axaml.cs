using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MonAppMultiplateforme.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MonAppMultiplateforme.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += (s, e) => SetupViewModel();
        SetupViewModel();
    }

    private void SetupViewModel()
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.RequestPasswordAction = async () =>
            {
                var dialog = new PasswordEntryWindow();
                var result = await dialog.ShowDialog<bool>(this);
                return result ? dialog.Password : null;
            };
        }
    }

    private async void AddVaultFileButton_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choisir des fichiers à ajouter au coffre-fort",
            AllowMultiple = true
        });

        if (files != null && files.Count > 0)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                foreach (var file in files)
                {
                    var filePath = file.Path.LocalPath;
                    await viewModel.UploadFile(filePath);
                }
            }
        }
    }
}