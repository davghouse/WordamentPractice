using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Daves.WordamentPractice.Views
{
    public partial class TileView : UserControl
    {
        public TileView()
            => InitializeComponent();

        private void Tile_TextChanged_ResizeFontToFit(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text.Length >= 5) textBox.FontSize = 24;
            else if (textBox.Text.Length >= 4) textBox.FontSize = 36;
            else if (textBox.Text.Length >= 3) textBox.FontSize = 48;
            else if (textBox.Text.Length >= 2) textBox.FontSize = 48;
            else textBox.FontSize = 72;
        }

        // Making enter and space simulate a tab press, see http://stackoverflow.com/q/9025278/1676558.
        private void TextBox_PreviewKeyDown_MapKeysToTab(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                e.Handled = true;

                var traversalRequest = new TraversalRequest(FocusNavigationDirection.Next);
                traversalRequest.Wrapped = true;
                ((TextBox)sender).MoveFocus(traversalRequest);
            }
        }

        public void SetBackgroundColors(Color color)
            => SquareBorder.Background
            = RoundBorder.Background
            = StringTextBox.Background
            = PointsTextBox.Background = new SolidColorBrush(color);

        public void ResetBackgroundColors()
        {
            SquareBorder.ClearValue(Border.BackgroundProperty);
            RoundBorder.ClearValue(Border.BackgroundProperty);
            StringTextBox.ClearValue(Border.BackgroundProperty);
            PointsTextBox.ClearValue(Border.BackgroundProperty);
        }
    }
}
