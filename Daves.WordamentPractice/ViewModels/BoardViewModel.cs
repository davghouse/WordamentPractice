﻿using Daves.WordamentSolver;
using GalaSoft.MvvmLight;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Daves.WordamentPractice.ViewModels
{
    public class BoardViewModel : ViewModelBase
    {
        public BoardViewModel()
        {
            for (int i = 0; i < 16; ++i)
            {
                TileViewModels[i] = new TileViewModel();
            }
        }

        public TileViewModel[] TileViewModels { get; } = new TileViewModel[16];

        private IReadOnlyList<Tile> _selectedPath;
        public IReadOnlyList<Tile> SelectedPath
        {
            get => _selectedPath;
            set => Set(ref _selectedPath, value);
        }

        public void Populate()
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
            for (int i = 0; i < emptyTileStringCount * _boardGenerationQualityFactor; ++i)
            {
                for (int t = 0; t < 16; ++t)
                {
                    if (originalTileStrings[t] != null) continue;

                    trialTileStrings[t] = _viableLetters[rand.Next(0, _viableLetters.Length)];
                }

                int trialWordsFound = new SimpleSolution(new Board(4, 4, t => trialTileStrings[t], p => null)).TotalWords;
                if (trialWordsFound > mostWordsFound)
                {
                    mostWordsFound = trialWordsFound;
                    Array.Copy(trialTileStrings, bestTileStrings, 16);
                }
            }

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

        public Board GetBoard()
            => new Board(4, 4,
                TileViewModels.Select(tvm => tvm.String),
                TileViewModels.Select(tvm => tvm.Points));

        public Solution GetSolution(WordSorter selectedWordSorter = null)
            => new Solution(GetBoard(), selectedWordSorter);

        public int GetTotalWords()
            => new SimpleSolution(GetBoard()).TotalWords;

        private static int _boardGenerationQualityFactor = int.Parse(ConfigurationManager.AppSettings["BoardGenerationQualityFactor"] ?? "6");

        private static readonly string[] _viableLetters = new string[]
        {
            "E", "T", "A", "O", "I", "N", "S", "H", "R", "D", "L", "C", "U", "M", "W", "F", "G", "Y", "P", "B", "V"
        };
    }
}
