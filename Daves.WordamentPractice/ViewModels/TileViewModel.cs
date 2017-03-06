using GalaSoft.MvvmLight;

namespace Daves.WordamentPractice.ViewModels
{
    public class TileViewModel : ViewModelBase
    {
        private string _string;
        public string String
        {
            get => _string;
            set => Set(ref _string, value);
        }

        private int? _points;
        public int? Points
        {
            get => _points;
            set => Set(ref _points, value);
        }

        public bool IsEmpty
            => string.IsNullOrWhiteSpace(String) && !Points.HasValue;

        public void Clear()
        {
            String = null;
            Points = null;
        }
    }
}
