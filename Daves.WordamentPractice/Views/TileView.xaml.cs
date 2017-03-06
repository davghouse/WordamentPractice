using System.Windows.Controls;

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
    }
}
