using Daves.WordamentPractice.Helpers;
using Daves.WordamentSolver;
using System;
using System.Windows;

namespace Daves.WordamentPractice
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs startupEventArgs)
        {
            try
            {
                Solution.SetDictionary(FileHelper.ReadDictionaryFile());
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
