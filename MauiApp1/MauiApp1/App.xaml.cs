using Microsoft.Maui.Controls;

namespace NotebookApp
{
    public partial class App : Application
    {
        public static DBService Database { get; private set; }
        public App()
        {
            InitializeComponent();
            Database = new DBService();
            MainPage = new NavigationPage(new MainPage());
        }
    }
}
