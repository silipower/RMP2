using Microsoft.Maui.Controls;

namespace NotebookApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Регистрация маршрутов для навигации
            Routing.RegisterRoute(nameof(NotePage), typeof(NotePage));
        }
    }
}
