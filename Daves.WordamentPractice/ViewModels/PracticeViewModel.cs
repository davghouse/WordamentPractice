using Daves.WordamentPractice.Utilities;
using Daves.WordamentSolver;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
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
            _timer = new UITimer(TimeSpan.FromSeconds(1), _timer_Tick);

            StartCommand = new RelayCommand(ExecuteStartCommand, CanExecuteStartCommand);
            PauseCommand = new RelayCommand(ExecutePauseCommand, CanExecutePauseCommand);
            StopCommand = new RelayCommand(ExecuteStopCommand, CanExecuteStopCommand);
            ClearCommand = new RelayCommand(ExecuteClearCommand, CanExecuteClearCommand);

            foreach (var tileViewModel in BoardViewModel.TileViewModels)
            {
                tileViewModel.TileUpdated += TileViewModel_TileUpdated;
            }
        }

        public BoardViewModel BoardViewModel { get; set; } = new BoardViewModel();

        private bool _isStarted;
        public bool IsStarted
        {
            get => _isStarted;
            set => Set(ref _isStarted, value);
        }

        private UITimer _timer;
        private void _timer_Tick(object sender, EventArgs e) => TimerLabel = _timer.ToString();
        private string _timerLabel = "0:00";
        public string TimerLabel
        {
            get => _timerLabel;
            set => Set(ref _timerLabel, value);
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
            set
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
            set => Set(ref _solutionWords, value);
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
                    BoardViewModel.SelectedPath = _selectedWord?.BestPath;
                }
            }
        }

        public ICommand StartCommand { get; }

        private bool CanExecuteStartCommand()
            => !IsStarted;

        private void ExecuteStartCommand()
        {
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

        private void ExecuteStopCommand()
        {
            IsPaused = false;
            IsStarted = false;
            _timer.Stop();
        }

        public ICommand ClearCommand { get; }

        private bool CanExecuteClearCommand()
            => !BoardViewModel.IsEmpty;

        private void ExecuteClearCommand()
        {
            ExecuteStopCommand();

            _isBeingCleared = true;
            BoardViewModel.Clear();
            Solution = BoardViewModel.GetSolution(SelectedWordSorter);
            _isBeingCleared = false;
        }

        private void TileViewModel_TileUpdated()
        {
            if (_isBeingPopulated || _isBeingCleared) return;

            Solution = BoardViewModel.GetSolution(SelectedWordSorter);
        }
    }
}
