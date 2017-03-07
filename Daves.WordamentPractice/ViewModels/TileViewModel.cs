using Daves.WordamentSolver;
using GalaSoft.MvvmLight;

namespace Daves.WordamentPractice.ViewModels
{
    public class TileViewModel : ViewModelBase
    {
        private string _string;
        public string String
        {
            get => _string;
            set
            {
                string previousValue = _string;

                if (Set(ref _string, value))
                {
                    // If they were using the guess for the previous value, use the guess for the new value.
                    if (Board.GuessTilePoints(previousValue) == Points)
                    {
                        Points = Board.GuessTilePoints(_string);
                    }
                }
            }
        }

        private int? _points;
        public int? Points
        {
            get => _points;
            set => Set(ref _points, value);
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
