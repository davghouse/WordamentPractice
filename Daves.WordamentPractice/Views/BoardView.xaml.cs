using Daves.WordamentPractice.Helpers;
using Daves.WordamentSolver;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Daves.WordamentPractice.Views
{
    public partial class BoardView : UserControl
    {
        private readonly IReadOnlyList<TileView> _tileViews;

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
            DependencyProperty.Register(nameof(HighlightedPath), typeof(IReadOnlyList<Tile>), typeof(BoardView),
                new PropertyMetadata((d, e) => ((BoardView)d).OnHighlightedPathChanged()));

        private void OnHighlightedPathChanged()
        {
            if (HighlightedPath != null)
            {
                _tileViews.ForEach(v => v.SetColors(Colors.Transparent, Colors.White));

                Color[] colorGradient = ColorHelper
                    .GetColorGradient(Colors.LimeGreen, Colors.Firebrick, HighlightedPath.Count)
                    .ToArray();
                for (int i = 0; i < HighlightedPath.Count; ++i)
                {
                    _tileViews[HighlightedPath[i].Position].SetBackgroundColors(colorGradient[i]);
                }
            }
            else
            {
                _tileViews.ForEach(v => v.ResetColors());
            }
        }
    }
}
