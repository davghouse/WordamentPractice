using Daves.WordamentPractice.ViewModels;
using Microsoft.Win32;
using System.Windows;

namespace Daves.WordamentPractice.Views
{
    public partial class PracticeView : Window
    {
        private readonly PracticeViewModel _practiceViewModel;

        public PracticeView()
        {
            InitializeComponent();
            DataContext = _practiceViewModel = new PracticeViewModel();
        }

        // So the game can be played, make the highlighted path disappear when the board is focused.
        private void BoardView_GotFocus(object sender, RoutedEventArgs e)
            => _practiceViewModel.SelectedWord = null;

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                _practiceViewModel.SaveToFile(dialog.FileName);
            }
        }

        private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                _practiceViewModel.LoadFromFile(dialog.FileName);
            }
        }

        private void BoardGenerationQualityFactorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new BoardGenerationQualityFactorDialog(_practiceViewModel.BoardViewModel.BoardGenerationQualityFactor);

            if (dialog.ShowDialog() == true)
            {
                _practiceViewModel.BoardViewModel.BoardGenerationQualityFactor = dialog.BoardGenerationQualityFactor;
            }
        }
    }
}
