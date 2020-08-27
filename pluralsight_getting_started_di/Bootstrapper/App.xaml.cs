using System.Windows;
using PeopleViewer.Presentation;
using PersonDataReader.CSV;
using PersonDataReader.Decorators;
using PersonDataReader.Service;
using PersonDataReader.SQL;

namespace PeopleViewer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ComposeObjects();
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Title = "With DI";
                Application.Current.MainWindow.Show();
            }
        }

        private static void ComposeObjects()
        {
            // var reader = new CSVReader();
            // var reader = new SQLReader();
            var wrappedReader = new ServiceReader();
            var reader = new CachingReader(wrappedReader);
            var viewModel = new PeopleViewModel(reader);
            Application.Current.MainWindow = new PeopleViewerWindow(viewModel);
        }
    }
}
