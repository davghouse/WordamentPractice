using WordamentPractice.Utilities;
using Daves.WordamentSolver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace WordamentPractice.ViewModels
{
    public class PracticeViewModel : ViewModelBase
    {
        public PracticeViewModel()
        {
            _timer = new UITimer(TimeSpan.FromSeconds(1), _timer_Tick_UpdateTimerLabel);

            StartCommand = new RelayCommand(CanExecuteStartCommand, ExecuteStartCommand);
            PauseCommand = new RelayCommand(CanExecutePauseCommand, ExecutePauseCommand);
            StopCommand = new RelayCommand(CanExecuteStopCommand, ExecuteStopCommand);
            ClearCommand = new RelayCommand(CanExecuteClearCommand, ExecuteClearCommand);

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

        private bool _isBeingPopulated;
        public bool IsBeingPopulated
        {
            get => _isBeingPopulated;
            private set => Set(ref _isBeingPopulated, value);
        }

        private bool _isBeingCleared;
        private bool IsBeingCleared
        {
            get => _isBeingCleared;
            set => Set(ref _isBeingCleared, value);
        }

        private readonly UITimer _timer;
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
                    SolutionWords = Solution.Words;
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
                if (Set(ref _selectedWordSorter, value) && SelectedWordSorter != null)
                {
                    Solution.SortWords(SelectedWordSorter);
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
                    WordLabel = SelectedWord?.String;
                    BoardViewModel.HighlightedPath = SelectedWord?.BestPath;
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
                    WordLabel = SelectedFoundWordPath?.Word.String;
                    BoardViewModel.HighlightedPath = SelectedFoundWordPath?.Path;
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
            => !(IsStarted || IsBeingPopulated);

        private async void ExecuteStartCommand()
        {
            Reset();

            IsBeingPopulated = true;
            await BoardViewModel.PopulateAsync(p => WordLabel = p);
            Solution = BoardViewModel.GetSolution(SelectedWordSorter);
            IsBeingPopulated = false;

            IsStarted = true;
            _timer.Start();

            CommandManager.InvalidateRequerySuggested(); // http://stackoverflow.com/a/14152091
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
            => !BoardViewModel.IsEmpty && !IsBeingPopulated;

        private void ExecuteClearCommand()
        {
            Reset();

            IsBeingCleared = true;
            BoardViewModel.Clear();
            Solution = BoardViewModel.GetSolution(SelectedWordSorter);
            IsBeingCleared = false;
        }

        private void TileViewModel_TileUpdated_ConsiderRecalculatingSolution()
        {
            if (IsBeingPopulated || IsBeingCleared) return;

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

            IsBeingPopulated = true;

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

            IsBeingPopulated = false;
        }
    }
}
