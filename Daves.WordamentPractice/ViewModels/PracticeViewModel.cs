using Daves.WordamentPractice.Utilities;
using Daves.WordamentSolver;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Daves.WordamentPractice.ViewModels
{
    public class PracticeViewModel : ViewModelBase
    {
        private bool _isBeingPopulated;
        private bool _isBeingCleared;

        public PracticeViewModel()
        {
            _timer = new UITimer(TimeSpan.FromSeconds(1), _timer_Tick_UpdateTimerLabel);

            StartCommand = new RelayCommand(ExecuteStartCommand, CanExecuteStartCommand);
            PauseCommand = new RelayCommand(ExecutePauseCommand, CanExecutePauseCommand);
            StopCommand = new RelayCommand(ExecuteStopCommand, CanExecuteStopCommand);
            ClearCommand = new RelayCommand(ExecuteClearCommand, CanExecuteClearCommand);

            foreach (var tileViewModel in BoardViewModel.TileViewModels)
            {
                tileViewModel.TileUpdated += TileViewModel_TileUpdated_ConsiderRecalculatingSolution;
            }
        }

        public BoardViewModel BoardViewModel { get; set; } = new BoardViewModel();

        private bool _isStarted;
        public bool IsStarted
        {
            get => _isStarted;
            private set => Set(ref _isStarted, value);
        }

        private UITimer _timer;
        private void _timer_Tick_UpdateTimerLabel(object sender, EventArgs e) => TimerLabel = _timer.ToString();
        private string _timerLabel = "0:00";
        public string TimerLabel
        {
            get => _timerLabel;
            private set => Set(ref _timerLabel, value);
        }

        private bool _isPaused;
        public bool IsPaused
        {
            get => _isPaused;
            private set => Set(ref _isPaused, value);
        }

        private Solution _solution = new Solution();
        public Solution Solution
        {
            get => _solution;
            private set
            {
                if (Set(ref _solution, value))
                {
                    SolutionWords = _solution.Words;
                }
            }
        }

        private IReadOnlyList<Word> _solutionWords;
        public IReadOnlyList<Word> SolutionWords
        {
            get => _solutionWords;
            private set => Set(ref _solutionWords, value);
        }

        public IReadOnlyList<WordSorter> WordSorters { get; } = WordSorter.All;

        private WordSorter _selectedWordSorter = WordSorter.Points;
        public WordSorter SelectedWordSorter
        {
            get => _selectedWordSorter;
            set
            {
                if (Set(ref _selectedWordSorter, value) && _selectedWordSorter != null)
                {
                    Solution.SortWords(_selectedWordSorter);
                    SolutionWords = Solution.Words.ToArray();
                }
            }
        }

        private Word _selectedWord;
        public Word SelectedWord
        {
            get => _selectedWord;
            set
            {
                if (Set(ref _selectedWord, value))
                {
                    WordLabel = _selectedWord?.String;
                    BoardViewModel.HighlightedPath = _selectedWord?.BestPath;
                }
            }
        }

        private HashSet<Word> _foundWords = new HashSet<Word>();
        public ObservableCollection<WordPath> FoundWordPaths { get; } = new ObservableCollection<WordPath>();

        private WordPath _selectedFoundWordPath;
        public WordPath SelectedFoundWordPath
        {
            get => _selectedFoundWordPath;
            set
            {
                if (Set(ref _selectedFoundWordPath, value))
                {
                    WordLabel = _selectedFoundWordPath?.Word.String;
                    BoardViewModel.HighlightedPath = _selectedFoundWordPath?.Path;
                }
            }
        }

        private string _wordLabel;
        public string WordLabel
        {
            get => _wordLabel;
            set => Set(ref _wordLabel, value);
        }

        private int _totalPointsFound;
        public int TotalPointsFound
        {
            get => _totalPointsFound;
            set => Set(ref _totalPointsFound, value);
        }

        private int _totalWordsFound;
        public int TotalWordsFound
        {
            get => _totalWordsFound;
            set => Set(ref _totalWordsFound, value);
        }

        private void Reset()
        {
            IsPaused = IsStarted = false;
            _timer.Stop();
            _foundWords.Clear();
            FoundWordPaths.Clear();
            TotalPointsFound = 0;
            TotalWordsFound = 0;
        }

        public ICommand StartCommand { get; }

        private bool CanExecuteStartCommand()
            => !IsStarted;

        private void ExecuteStartCommand()
        {
            Reset();

            _isBeingPopulated = true;
            BoardViewModel.Populate();
            Solution = BoardViewModel.GetSolution(SelectedWordSorter);
            _isBeingPopulated = false;

            IsStarted = true;
            _timer.Start();
        }

        public ICommand PauseCommand { get; }

        private bool CanExecutePauseCommand()
            => IsStarted;

        private void ExecutePauseCommand()
        {
            if (IsPaused)
            {
                IsPaused = false;
                _timer.Unpause();
            }
            else
            {
                _timer.Pause();
                IsPaused = true;
            }
        }

        public ICommand StopCommand { get; }

        private bool CanExecuteStopCommand()
            => IsStarted;

        // Stop the clock but leave the found words intact so the user can look over their results.
        private void ExecuteStopCommand()
        {
            IsPaused = IsStarted = false;
            _timer.Pause();
        }

        public ICommand ClearCommand { get; }

        private bool CanExecuteClearCommand()
            => !BoardViewModel.IsEmpty;

        private void ExecuteClearCommand()
        {
            Reset();

            _isBeingCleared = true;
            BoardViewModel.Clear();
            Solution = BoardViewModel.GetSolution(SelectedWordSorter);
            _isBeingCleared = false;
        }

        private void TileViewModel_TileUpdated_ConsiderRecalculatingSolution()
        {
            if (_isBeingPopulated || _isBeingCleared) return;

            Reset();

            Solution = BoardViewModel.GetSolution(SelectedWordSorter);
        }

        public PathSubmissionStatus SubmitPath(IReadOnlyList<Tile> tilePath)
        {
            var status = PathSubmissionStatus.NoWordsFound;
            if (Solution.TryGetPath(tilePath, out Path path))
            {
                status = PathSubmissionStatus.OldWordsFound;
                foreach (var word in path.Words)
                {
                    if (_foundWords.Add(word))
                    {
                        status = PathSubmissionStatus.NewWordsFound;
                        var wordPath = new WordPath(word, path);
                        TotalPointsFound += wordPath.Points;
                        // This sucks, but there's no free & easy way to display in reversed order if we Add instead.
                        FoundWordPaths.Insert(0, wordPath);
                        ++TotalWordsFound;
                    }
                }
            }

            return status;
        }

        public void SaveToFile(string filePath)
            => System.IO.File.WriteAllLines(filePath,
                BoardViewModel.TileViewModels.Select(t => t.String).Concat(
                BoardViewModel.TileViewModels.Select(t => t.Points?.ToString())));

        public void LoadFromFile(string filePath)
        {
            Reset();

            _isBeingPopulated = true;

            string[] lines = System.IO.File.ReadAllLines(filePath);
            if (lines.Length < 16 * 2)
                throw new FormatException($"{filePath} doesn't correctly define a board.");

            for (int i = 0; i < 16; ++i)
            {
                BoardViewModel.TileViewModels[i].String = lines[i];
            }

            for (int i = 0; i < 16; ++i)
            {
                if (int.TryParse(lines[i + 16], out int points))
                {
                    BoardViewModel.TileViewModels[i].Points = points;
                }
                else
                {
                    BoardViewModel.TileViewModels[i].Points = null;
                }
            }

            Solution = BoardViewModel.GetSolution(SelectedWordSorter);

            _isBeingPopulated = false;
        }
    }
}
