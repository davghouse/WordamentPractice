using Daves.WordamentPractice.Utilities;
using Daves.WordamentSolver;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Daves.WordamentPractice.ViewModels
{
    public class PracticeViewModel : ViewModelBase
    {
        private bool IsStarted { get; set; }
        private bool IsBeingPopulated { get; set; }
        private bool IsBeingCleared { get; set; }

        public PracticeViewModel()
        {
            _timer = new UITimer(TimeSpan.FromSeconds(1), _timer_Tick);

            StartCommand = new RelayCommand(ExecuteStartCommand, CanExecuteStartCommand);
            PauseCommand = new RelayCommand(ExecutePauseCommand, CanExecutePauseCommand);
            StopCommand = new RelayCommand(ExecuteStopCommand, CanExecuteStopCommand);
            ClearCommand = new RelayCommand(ExecuteClearCommand, CanExecuteClearCommand);

            foreach (var tileViewModel in BoardViewModel.TileViewModels)
            {
                tileViewModel.PropertyChanged += TileViewModel_PropertyChanged;
            }
        }

        public BoardViewModel BoardViewModel { get; set; } = new BoardViewModel();

        private UITimer _timer;
        private void _timer_Tick(object sender, EventArgs e) => TimerLabel = _timer.ToString();
        private string _timerLabel = "0:00";
        public string TimerLabel
        {
            get => _timerLabel;
            set => Set(ref _timerLabel, value);
        }

        private string _pauseButtonContent = "Pause";
        public string PauseButtonContent
        {
            get => _pauseButtonContent;
            set => Set(ref _pauseButtonContent, value);
        }

        private bool _isPaused;
        private bool IsPaused
        {
            get => _isPaused;
            set
            {
                _isPaused = value;
                PauseButtonContent = _isPaused ? "Unpause" : "Pause";
            }
        }

        private Solution _solution = new Solution();
        private Solution Solution
        {
            get => _solution;
            set
            {
                if (Set(ref _solution, value))
                {
                    PointsLabel = $"0 of {_solution.TotalPoints} points";
                    WordsLabel = $"0 of {_solution.WordsFound} words";
                    SolutionWords = _solution.Words;
                }
            }
        }

        private string _pointsLabel = "0 of 0 points";
        public string PointsLabel
        {
            get => _pointsLabel;
            set => Set(ref _pointsLabel, value);
        }

        private string _wordsLabel = "0 of 0 words";
        public string WordsLabel
        {
            get => _wordsLabel;
            set => Set(ref _wordsLabel, value);
        }

        private IReadOnlyList<Word> _solutionWords;
        public IReadOnlyList<Word> SolutionWords
        {
            get => _solutionWords;
            set => Set(ref _solutionWords, value);
        }

        public IReadOnlyList<WordSorter> WordSorters { get; } = WordSorter.All;

        private WordSorter _selectedWordSorter;
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

        public ICommand StartCommand { get; }

        private bool CanExecuteStartCommand()
            => !IsStarted;

        private void ExecuteStartCommand()
        {
            IsBeingPopulated = true;
            BoardViewModel.Populate();
            Solution = BoardViewModel.GetSolution(SelectedWordSorter);
            IsBeingPopulated = false;

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

            IsBeingCleared = true;
            BoardViewModel.Clear();
            Solution = BoardViewModel.GetSolution(SelectedWordSorter);
            IsBeingCleared = false;
        }

        private void TileViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsBeingPopulated || IsBeingCleared) return;

            if (e.PropertyName.Equals(nameof(TileViewModel.String))
                || e.PropertyName.Equals(nameof(TileViewModel.Points)))
            {
                Solution = BoardViewModel.GetSolution(SelectedWordSorter);
            }
        }
    }
}
