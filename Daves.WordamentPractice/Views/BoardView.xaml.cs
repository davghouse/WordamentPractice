using Daves.WordamentPractice.Helpers;
using Daves.WordamentSolver;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Daves.WordamentPractice.Views
{
    public partial class BoardView : UserControl
    {
        private IReadOnlyList<TileView> _tileViews;

        public BoardView()
        {
            InitializeComponent();
            _tileViews = TileGrid.Children.OfType<TileView>().ToArray();
        }

        public IReadOnlyList<Tile> HighlightedPath
        {
            get => (IReadOnlyList<Tile>)GetValue(HighlightedPathProperty);
            set => SetValue(HighlightedPathProperty, value);
        }

        public static readonly DependencyProperty HighlightedPathProperty =
            DependencyProperty.Register(
                "HighlightedPath",
                typeof(IReadOnlyList<Tile>),
                typeof(BoardView),
                new PropertyMetadata((d, e) => ((BoardView)d).OnHighlightedPathChanged()));

        private void OnHighlightedPathChanged()
        {
            if (HighlightedPath != null)
            {
                var pathColorGradient = HighlightedPath.Zip(
                    ColorHelper.GetColorGradient(Colors.LimeGreen, Colors.Firebrick, HighlightedPath.Count),
                    (Tile, Color) => new { Tile, Color })
                    .ToArray();

                for (int i = 0; i < _tileViews.Count; ++i)
                {
                    var brush = new SolidColorBrush(pathColorGradient.FirstOrDefault(a => a.Tile.Position == i)?.Color ?? Colors.Transparent);
                    _tileViews[i].SquareBorder.Background
                        = _tileViews[i].RoundBorder.Background
                        = _tileViews[i].StringTextBox.Background
                        = _tileViews[i].PointsTextBox.Background = brush;
                }
            }
            else
            {
                foreach (var tileView in _tileViews)
                {
                    tileView.SquareBorder.ClearValue(Border.BackgroundProperty);
                    tileView.RoundBorder.ClearValue(Border.BackgroundProperty);
                    tileView.StringTextBox.ClearValue(Border.BackgroundProperty);
                    tileView.PointsTextBox.ClearValue(Border.BackgroundProperty);
                }
            }
        }
    }
}
