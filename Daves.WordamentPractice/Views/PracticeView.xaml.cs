using Daves.WordamentPractice.ViewModels;
using Daves.WordamentSolver;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
        private void BoardView_MouseEnter_ClearSelectedWords(object sender, MouseEventArgs e)
        {
            _practiceViewModel.SelectedWord = null;
            _practiceViewModel.SelectedFoundWordPath = null;
        }

        private void SaveAsMenuItem_Click_SaveBoardToFile(object sender, RoutedEventArgs e)
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

        private void LoadMenuItem_Click_LoadBoardFromFile(object sender, RoutedEventArgs e)
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

        private void BoardGenerationQualityFactorMenuItem_Click_ShowDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new BoardGenerationQualityFactorDialog(_practiceViewModel.BoardViewModel.BoardGenerationQualityFactor);

            if (dialog.ShowDialog() == true)
            {
                _practiceViewModel.BoardViewModel.BoardGenerationQualityFactor = dialog.BoardGenerationQualityFactor;
            }
        }

        private List<TileView> _tileViewPathBeingBuilt = new List<TileView>();
        private List<Tile> _tilePathBeingBuilt = new List<Tile>();
        private bool IsPlayable => _practiceViewModel.IsStarted;
        private bool IsPathBeingBuilt => _tilePathBeingBuilt.Count > 0;
        private Board Board => _practiceViewModel.BoardViewModel.Board;

        private void TileView_MouseLeftButtonDown_TryBeginningPath(object sender, MouseButtonEventArgs e)
        {
            if (!IsPlayable || IsPathBeingBuilt) return;

            AddToPathBeingBuilt((TileView)sender);
        }

        private void TileView_MouseEnteredInterior_TryContinuingPath(object sender, RoutedEventArgs e)
        {
            if (!IsPlayable || !IsPathBeingBuilt) return;

            var tileView = (TileView)sender;

            // If we've re-entered the next to last tile, remove the last tile from the path.
            if (_tileViewPathBeingBuilt.Count > 1
                && _tileViewPathBeingBuilt[_tileViewPathBeingBuilt.Count - 2] == tileView)
            {
                RemoveFromPathBeingBuilt();
            }
            else if (Board.Tiles[tileView.Position].CanExtend(_tilePathBeingBuilt))
            {
                AddToPathBeingBuilt((TileView)sender);
            }
        }

        private void AddToPathBeingBuilt(TileView tileView)
        {
            tileView.SetColors(Colors.White, Colors.Black);
            _tileViewPathBeingBuilt.Add(tileView);
            _tilePathBeingBuilt.Add(Board.Tiles[tileView.Position]);
            _tilePathBeingBuilt.Last().IsTaken = true;
        }

        private void RemoveFromPathBeingBuilt()
        {
            _tileViewPathBeingBuilt.Last().ResetColors();
            _tileViewPathBeingBuilt.RemoveAt(_tileViewPathBeingBuilt.Count - 1);
            _tilePathBeingBuilt.Last().IsTaken = false;
            _tilePathBeingBuilt.RemoveAt(_tilePathBeingBuilt.Count - 1);
        }

        private void Window_MouseLeftButtonUp_TrySubmittingPath(object sender, MouseButtonEventArgs e)
        {
            if (IsPlayable && IsPathBeingBuilt)
            {
                UpdateAfterPathSubmission(_practiceViewModel.SubmitPath(_tilePathBeingBuilt));
            }
        }

        // If the user leaves the window, releases the LMB, and then re-enters, try submitting.
        private void Window_MouseEnter_TrySubmittingPath(object sender, MouseEventArgs e)
        {
            if (IsPlayable && IsPathBeingBuilt && e.LeftButton == MouseButtonState.Released)
            {
                UpdateAfterPathSubmission(_practiceViewModel.SubmitPath(_tilePathBeingBuilt));
            }
        }

        public void UpdateAfterPathSubmission(PathSubmissionStatus status)
        {
            if (status == PathSubmissionStatus.NoWordsFound
                && _tilePathBeingBuilt.Count <= 2)
            {
                _tileViewPathBeingBuilt.ForEach(v => v.ResetColors());
            }
            else
            {
                var backgroundBrush = status == PathSubmissionStatus.NewWordsFound ? new SolidColorBrush(Colors.Green)
                    : status == PathSubmissionStatus.NoWordsFound ? new SolidColorBrush(Colors.Red)
                    : new SolidColorBrush(Color.FromArgb(255, 205, 189, 31)); // Old words found, gold-ish color.

                foreach (var tileView in _tileViewPathBeingBuilt)
                {
                    tileView.SetBackgroundColors(backgroundBrush);
                    tileView.ResetForegroundColors();
                }

                var backgroundColorAnimation = new ColorAnimation()
                {
                    To = ((SolidColorBrush)Application.Current.FindResource("OrangeishTileBrush")).Color,
                    AccelerationRatio = 1,
                    Duration = new Duration(TimeSpan.FromSeconds(.55))
                };
                backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, backgroundColorAnimation);
            }

            _tileViewPathBeingBuilt.Clear();
            _tilePathBeingBuilt.ForEach(t => t.IsTaken = false);
            _tilePathBeingBuilt.Clear();
        }
    }
}
