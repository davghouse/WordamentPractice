using System.Windows;

namespace Daves.WordamentPractice.Views
{
    public partial class BoardGenerationQualityFactorDialog : Window
    {
        private readonly int _originalboardGenerationQualityFactor;

        public BoardGenerationQualityFactorDialog(int boardGenerationQualityFactor)
        {
            InitializeComponent();

            _originalboardGenerationQualityFactor = boardGenerationQualityFactor;
            BoardGenerationQualityFactorTextBox.Text = boardGenerationQualityFactor.ToString();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
            => DialogResult = true;

        public int BoardGenerationQualityFactor
        {
            get
            {
                if (int.TryParse(BoardGenerationQualityFactorTextBox.Text, out int boardGenerationQualityFactor))
                    return boardGenerationQualityFactor;
                return _originalboardGenerationQualityFactor;
            }
        }
    }
}
