using Daves.WordamentPractice.Helpers;
using Daves.WordamentPractice.Properties;
using Daves.WordamentSolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Daves.WordamentPractice.ViewModels
{
    public class BoardViewModel : ViewModelBase
    {
        public BoardViewModel()
        {
            var tileViewModels = new TileViewModel[16];
            for (int i = 0; i < 16; ++i)
            {
                tileViewModels[i] = new TileViewModel(i);
                tileViewModels[i].TileUpdated += TileViewModel_TileUpdated_ClearBoardCache;
            }
            TileViewModels = tileViewModels;
        }

        public IReadOnlyList<TileViewModel> TileViewModels { get; }

        private void TileViewModel_TileUpdated_ClearBoardCache() => _board = null;
        private Board _board;
        public Board Board
        {
            get
            {
                if (_board == null)
                {
                    _board = new Board(4, 4,
                        TileViewModels.Select(tvm => tvm.String),
                        TileViewModels.Select(tvm => tvm.Points));
                }

                return _board;
            }
        }

        private IReadOnlyList<Tile> _highlightedPath;
        public IReadOnlyList<Tile> HighlightedPath
        {
            get => _highlightedPath;
            set => Set(ref _highlightedPath, value);
        }

        public int BoardGenerationQualityFactor
        {
            get => Settings.Default.BoardGenerationQualityFactor;
            set
            {
                Settings.Default.BoardGenerationQualityFactor = value;
                Settings.Default.Save();
            }
        }

        private static readonly IReadOnlyList<string> _viableLetters = new string[]
        {
            "E", "T", "A", "O", "I", "N", "S", "H", "R", "D", "L", "C", "U", "M", "W", "F", "G", "Y", "P", "B", "V"
        };

        public async Task PopulateAsync(Action<string> progressUpdater)
        {
            // Generate some random boards (6 times the number of tiles needing strings, by default) and choose the best one.
            var rand = new Random();
            int mostWordsFound = GetTotalWords();
            string[] originalTileStrings = TileViewModels
                .Select(tvm => tvm.HasString ? tvm.String : null)
                .ToArray();
            string[] bestTileStrings = originalTileStrings.ToArray();
            string[] trialTileStrings = originalTileStrings.ToArray();
            int emptyTileStringCount = originalTileStrings.Count(s => s == null);
            int boardGenerationCount = emptyTileStringCount * BoardGenerationQualityFactor;

            await Task.Run(() =>
            {
                var stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < boardGenerationCount; ++i)
                {
                    if (i % 10 == 0 && stopwatch.ElapsedMilliseconds > 250)
                    {
                        progressUpdater($"{100 * i / (double)boardGenerationCount:N0}% complete...");
                    }

                    for (int t = 0; t < 16; ++t)
                    {
                        if (originalTileStrings[t] != null) continue;

                        trialTileStrings[t] = _viableLetters[rand.Next(0, _viableLetters.Count)];
                    }

                    int trialWordsFound = new SimpleSolution(new Board(4, 4, t => trialTileStrings[t], p => null)).TotalWords;
                    if (trialWordsFound > mostWordsFound)
                    {
                        mostWordsFound = trialWordsFound;
                        Array.Copy(trialTileStrings, bestTileStrings, 16);
                    }
                }

                progressUpdater(string.Empty);
            });

            for (int t = 0; t < 16; ++t)
            {
                TileViewModels[t].String = bestTileStrings[t];
                TileViewModels[t].Points = TileViewModels[t].Points ?? Board.GuessTilePoints(bestTileStrings[t]);
            }
        }

        public bool IsEmpty
            => TileViewModels.All(tvm => tvm.IsEmpty);

        public void Clear()
            => TileViewModels.ForEach(tvm => tvm.Clear());

        public Solution GetSolution(WordSorter selectedWordSorter = null)
            => new Solution(Board, selectedWordSorter);

        public int GetTotalWords()
            => new SimpleSolution(Board).TotalWords;
    }
}
