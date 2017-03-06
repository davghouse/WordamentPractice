using Daves.WordamentSolver;
using MoreLinq;
using System;
using System.Linq;

namespace Daves.WordamentPractice.ViewModels
{
    public class BoardViewModel
    {
        private static readonly double[] _letterFrequencies = new[]
        {
            8.167, 1.492, 2.782, 4.253, 12.702, 2.228, 2.015, 6.094, 6.966, 0.153, 0.772, 4.025, 2.406,
            6.749, 7.507, 1.929, 0.095, 5.987, 6.327, 9.056, 2.758, 0.978, 2.360, 0.150, 1.974, 0.074
        };

        private static readonly double[] _cumulativeLetterFrequencies = new double[26];

        static BoardViewModel()
        {
            _cumulativeLetterFrequencies[0] = _letterFrequencies[0];
            for (int i = 1; i < 25; ++i)
            {
                _cumulativeLetterFrequencies[i] = _letterFrequencies[i] + _cumulativeLetterFrequencies[i - 1];
            }
            _cumulativeLetterFrequencies[25] = 100;
        }

        public BoardViewModel()
        {
            for (int i = 0; i < 16; ++i)
            {
                TileViewModels[i] = new TileViewModel();
            }
        }

        public TileViewModel[] TileViewModels { get; } = new TileViewModel[16];

        public void Populate()
        {
            var rand = new Random();

            Board bestBoard = GetBoard();
            int missingTileCount = TileViewModels.Count(tvm => string.IsNullOrWhiteSpace(tvm.String));
            for (int i = 0; i < missingTileCount * 2; ++i)
            {


            }

            foreach (var tileViewModel in TileViewModels)
            {
                if (string.IsNullOrWhiteSpace(tileViewModel.String))
                {
                    int letterIndex = Array.IndexOf(_cumulativeLetterFrequencies,
                        _cumulativeLetterFrequencies.First(f => f > rand.NextDouble() * 100));
                    tileViewModel.String = ((char)('A' + letterIndex)).ToString();
                }

                if (!tileViewModel.Points.HasValue)
                {
                    tileViewModel.Points = Board.GuessTilePoints(tileViewModel.String);
                }
            }
        }

        public bool IsEmpty
            => TileViewModels.All(tvm => tvm.IsEmpty);

        public void Clear()
            => TileViewModels.ForEach(tvm => tvm.Clear());

        public Board GetBoard()
            => new Board(4, 4,
                TileViewModels.Select(tvm => tvm.String),
                TileViewModels.Select(tvm => tvm.Points));

        public Solution GetSolution()
            => new Solution(GetBoard());
    }
}
