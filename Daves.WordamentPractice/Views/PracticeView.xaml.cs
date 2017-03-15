using Daves.WordamentPractice.ViewModels;
using System.Windows;

namespace Daves.WordamentPractice.Views
{
    public partial class PracticeView : Window
    {
        public PracticeView()
        {
            InitializeComponent();
            DataContext = new PracticeViewModel();
        }

        // So the game can be played, make the highlighted path disappear when the board is focused.
        private void BoardView_GotFocus(object sender, RoutedEventArgs e)
            => ((PracticeViewModel)DataContext).SelectedWord = null;
    }
}
