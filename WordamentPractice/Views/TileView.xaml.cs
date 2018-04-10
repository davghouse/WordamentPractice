using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WordamentPractice.Views
{
    public partial class TileView : UserControl
    {
        public TileView()
            => InitializeComponent();

        public int Position
        {
            get => (int)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(int), typeof(TileView), new PropertyMetadata(0));

        private void Tile_TextChanged_ResizeFontToFit(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text.Length >= 5) textBox.FontSize = 24;
            else if (textBox.Text.Length >= 4) textBox.FontSize = 36;
            else if (textBox.Text.Length >= 3) textBox.FontSize = 46;
            else if (textBox.Text.Length >= 2) textBox.FontSize = 48;
            else textBox.FontSize = 72;
        }

        // Making enter and space simulate a tab press, see http://stackoverflow.com/q/9025278.
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

        public void SetBackgroundColors(Color color) => SetBackgroundColors(new SolidColorBrush(color));
        public void SetBackgroundColors(SolidColorBrush brush)
            => SquareBorder.Background
            = RoundBorder.Background
            = StringTextBox.Background
            = PointsTextBox.Background = brush;

        public void SetForegroundColors(Color color)
            => StringTextBox.Foreground
            = PointsTextBox.Foreground = new SolidColorBrush(color);

        public void SetColors(Color backgroundColor, Color foregroundColor)
        {
            SetBackgroundColors(backgroundColor);
            SetForegroundColors(foregroundColor);
        }

        public void ResetBackgroundColors()
        {
            SquareBorder.ClearValue(Border.BackgroundProperty);
            RoundBorder.ClearValue(Border.BackgroundProperty);
            StringTextBox.ClearValue(TextBox.BackgroundProperty);
            PointsTextBox.ClearValue(TextBox.BackgroundProperty);
        }

        public void ResetForegroundColors()
        {
            StringTextBox.ClearValue(TextBox.ForegroundProperty);
            PointsTextBox.ClearValue(TextBox.ForegroundProperty);
        }

        public void ResetColors()
        {
            ResetBackgroundColors();
            ResetForegroundColors();
        }

        public static readonly RoutedEvent MouseEnteredInteriorEvent = EventManager.RegisterRoutedEvent(
            "MouseEnteredInterior", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TileView));

        public event RoutedEventHandler MouseEnteredInterior
        {
            add { AddHandler(MouseEnteredInteriorEvent, value); }
            remove { RemoveHandler(MouseEnteredInteriorEvent, value); }
        }

        private void RoundBorder_MouseEnter_RaiseMouseEnteredInterior(object sender, MouseEventArgs e)
            => RaiseEvent(new RoutedEventArgs(MouseEnteredInteriorEvent));
    }
}
