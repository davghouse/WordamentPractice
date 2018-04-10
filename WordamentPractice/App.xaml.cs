using WordamentPractice.Views;
using Daves.WordamentSolver;
using System;
using System.Windows;

namespace WordamentPractice
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs startupEventArgs)
        {
            try
            {
                Solution.SetDictionary();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            var practiceView = new PracticeView();
            practiceView.Show();
        }
    }
}
