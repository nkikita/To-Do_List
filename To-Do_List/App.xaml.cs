using System.Configuration;
using System.Data;
using System.Windows;
using To_Do_List.Services;

namespace To_Do_List
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var storage = new JsonTaskStorage();
            var viewModel = new MainViewModel(storage);
            var window = new MainWindow
            {
                DataContext = viewModel
            };
            window.Show();
        }
    }

}
