using System;
using System.Globalization;
using System.Windows.Data;

namespace WordamentPractice.Converters
{
    public class IsTileHitTestVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isStarted = (bool)values[0];
            bool isBeingPopulated = (bool)values[1];

            // If the game's started or the board is being populated, don't allow editing (make hit test visible false).
            return !(isStarted || isBeingPopulated);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
