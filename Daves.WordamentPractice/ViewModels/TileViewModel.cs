using Daves.WordamentSolver;
using GalaSoft.MvvmLight;
using System;

namespace Daves.WordamentPractice.ViewModels
{
    public class TileViewModel : ViewModelBase
    {
        private bool _raiseTileUpdated = true;
        public event Action TileUpdated;

        private string _string;
        public string String
        {
            get => _string;
            set
            {
                string previousString = _string;

                if (Set(ref _string, value))
                {
                    // If they were using the guess for the previous value, use the guess for the new value. Turn off the
                    // tile updated event so we're sure to only signal once for a simultaneous change (performance concern).
                    if (Board.GuessTilePoints(previousString) == Points)
                    {
                        _raiseTileUpdated = false;
                        Points = Board.GuessTilePoints(_string);
                        _raiseTileUpdated = true;
                    }

                    TileUpdated();
                }
            }
        }

        private int? _points;
        public int? Points
        {
            get => _points;
            set
            {
                if (Set(ref _points, value) && _raiseTileUpdated)
                {
                    TileUpdated();
                }
            }
        }

        public bool HasString => !string.IsNullOrWhiteSpace(String);
        public bool HasPoints => Points.HasValue;
        public bool IsEmpty => !HasString && !HasPoints;

        public void Clear()
        {
            String = null;
            Points = null;
        }
    }
}
