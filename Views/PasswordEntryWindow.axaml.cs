using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MonAppMultiplateforme.Views
{
    public partial class PasswordEntryWindow : Window
    {
        public string? Password { get; private set; }

        public PasswordEntryWindow()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            Password = this.FindControl<TextBox>("PasswordInput")?.Text;
            Close(true);
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Password = null;
            Close(false);
        }
    }
}
