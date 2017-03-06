using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Daves.WordamentPractice
{
    public partial class PracticeView2 : Window
    {
        private bool _started = false;

        public PracticeView2()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(Timer_Elapsed);
            timer.Interval = new TimeSpan(0, 0, 1);
            comboBox1.SelectedIndex = 1;
        }

        #region Button clicks

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            _started = true;
            for (int i = 0; i < currentPath.Count(); ++i)
            {
                UncolorTile(currentPath[i]);
            }
            currentPath.Clear();
            mouseLeftButtonPressed = false;
            // Make TextBoxes' isHitTestVisible false, so the game can be played, and fill in any missing values.
            Random randAscii = new Random();
            foreach (var c in Board.Children)
            {
                if (c.GetType() == typeof(Border))
                {
                    TextBox letterTextBox = (TextBox)((Border)((Border)c).Child).Child;
                    letterTextBox.IsHitTestVisible = false;
                    // Get some random letters; potential to make this into something that generates decent boards.
                    if (letterTextBox.Text.Count() == 0)
                    {
                        int ascii = randAscii.Next(65, 91);
                        letterTextBox.Text = ((char)ascii).ToString();
                    }
                }
            }
            foreach (var c in Board.Children)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    TextBox pointTextBox = (TextBox)c;
                    pointTextBox.IsHitTestVisible = false;
                    if (pointTextBox.Text.Count() == 0)
                    {
                        int pointIndex = Convert.ToInt32(pointTextBox.Name.Substring(5));
                        Border d = this.FindName("pborder" + pointIndex.ToString()) as Border;
                        TextBox letterTextBox = (TextBox)((Border)((Border)d).Child).Child;
                        int letterIndex = Convert.ToInt32(letterTextBox.Name.Substring(7));
                        if (pointIndex == letterIndex)
                        {
                            string temp = letterTextBox.Text;
                            int tileValue = 0;
                            for (int i = 0; i < temp.Count(); ++i)
                            {
                                if (Char.IsLetter(temp[i]))
                                {
                                    tileValue += basicTileValues[temp[i]];
                                }
                            }
                            // Special tile type.
                            if (temp.Count() > 1)
                            {
                                tileValue += 5;
                            }
                            // Either/or tile.
                            if (temp.Count() == 3 && (temp[1] == '\\' || temp[1] == '/'))
                            {
                                tileValue = 20;
                            }
                            pointTextBox.Text = tileValue.ToString();
                        }
                    }
                }
            }
            timer.Start();
            pauseButton.Content = "Pause";
            if (paths.Count() == 0)
            {
                string[] stringBoard = new string[dim * dim];
                for (int i = 1; i <= dim * dim; ++i)
                {
                    stringBoard[i - 1] = (this.FindName("textbox" + i.ToString()) as TextBox).Text;
                }
                try
                {
                    words = Solvera.RunSolver(stringBoard, paths);
                    ComputePoints();
                }
                catch
                {
                    // If there's a problem it's hopefully with what was input by the user. Let them figure it out.
                }
                // Fill this out for use in sorting.
                for (int i = 0; i < words.Count(); ++i)
                {
                    for (int j = 0; j < words[i].Count(); ++j)
                    {
                        solutionWordPointPaths.Add(new WordPointPath(words[i][j], points[i][j], paths[i][j]));
                    }
                }

                WordPointPath.SortWordPointPaths(comboBox1.SelectedIndex, solutionWordPointPaths);

                for (int i = 0; i < solutionWordPointPaths.Count(); ++i)
                {
                    totalPoints += solutionWordPointPaths[i].points;
                }
                pointsLabel.Content = "0 of " + totalPoints + " points";
                wordsLabel.Content = "0 of " + solutionWordPointPaths.Count() + " words";
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            _started = false;
            timer.Stop();
            seconds = 0;
            timerLabel.Content = "0:00";
            pauseButton.Content = "Pause";
            words.Clear();
            solutionWordPointPaths.Clear();
            foundWordPointPaths.Clear();
            points.Clear();
            paths.Clear();
            totalPoints = 0;
            foundPoints = 0;
            pointsLabel.Content = "";
            wordsLabel.Content = "";
            currentWordLabel.Content = "";
            foundBox.Items.Clear();
            solutionBox.Items.Clear();
            foreach (var c in Board.Children)
            {
                if (c.GetType() == typeof(Border))
                {
                    UncolorTile((Border)c);
                    TextBox letterTextBox = (TextBox)((Border)((Border)c).Child).Child;
                    letterTextBox.IsHitTestVisible = true;
                    letterTextBox.Text = "";
                }
                if (c.GetType() == typeof(TextBox))
                {
                    TextBox pointTextBox = (TextBox)c;
                    pointTextBox.IsHitTestVisible = true;
                    pointTextBox.Text = "";
                }
            }
        }

        // Pausing just stops the timer; the game can still be played.
        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < currentPath.Count(); ++i)
            {
                UncolorTile(currentPath[i]);
            }
            currentPath.Clear();
            mouseLeftButtonPressed = false;
            if ((string)pauseButton.Content == "Pause")
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    pauseButton.Content = "Unpause";
                }
            }
            else if ((string)pauseButton.Content == "Unpause")
            {
                if (!timer.IsEnabled)
                {
                    timer.Start();
                    pauseButton.Content = "Pause";
                }
            }
        }

        private void solveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_started)
            {
                for (int i = 1; i < dim * dim; ++i)
                {
                    if (((TextBox)this.FindName("textbox" + i.ToString())).Text == "")
                    {
                        return;
                    }
                }
                startButton_Click(null, new RoutedEventArgs());
                pauseButton_Click(null, new RoutedEventArgs());
            }
            Display(solutionBox, solutionWordPointPaths);
        }

        #endregion

        #region Creating/validating words

        // Clicking in a border begins a word.
        private void ParentBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border parentBorder = (Border)sender;
            if ((string)currentWordLabel.Content != "")
            {
                for (int i = 1; i <= dim * dim; ++i)
                {
                    UncolorTile(((Border)this.FindName("pborder" + i.ToString())));
                }
                currentWordLabel.Content = "";
            }
            if ((currentPath.Count() == 0) && _started)
            {
                currentPath.Add(parentBorder);
                ColorTile(parentBorder, new SolidColorBrush(Colors.White));
            }
        }

        // Moving through the underlying elliptical child border with the LMB held down continues a path.
        private void ChildBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (mouseLeftButtonPressed && !textbox1.IsHitTestVisible && _started)
            {
                bool color = false;
                Border parentBorder = (Border)((Border)sender).Parent;
                if ((currentPath.Count() > 1) && currentPath[currentPath.Count() - 2] == parentBorder)
                {
                    UncolorTile(currentPath[currentPath.Count() - 1]);
                    currentPath.RemoveAt(currentPath.Count() - 1);
                    return;
                }
                // Checking for prefix and suffix tiles.
                if ((currentPath.Count() != 0))
                {
                    string text = ((TextBox)((Border)parentBorder.Child).Child).Text;
                    if (text[text.Count() - 1] == '-')
                    {
                        return;
                    }
                    if (((TextBox)(((Border)currentPath[currentPath.Count() - 1].Child).Child)).Text[0] == '-')
                    {
                        return;
                    }
                }
                if (currentPath.Count() == 0)
                {
                    color = true;
                }
                else if (!currentPath.Contains(parentBorder))
                {
                    // The tile isn't first, and isn't already taken; check if it is adjacent to the previous tile in the path.
                    int tileIndex = Convert.ToInt32(parentBorder.Name.Substring(7));
                    int previousTileIndex = Convert.ToInt32(currentPath[currentPath.Count() - 1].Name.Substring(7));
                    if (Helpersa.tilesAreAdjacent(tileIndex, previousTileIndex))
                    {
                        color = true;
                    }
                }
                if (color)
                {
                    currentPath.Add(parentBorder);
                    ColorTile(parentBorder, new SolidColorBrush(Colors.White));
                }
            }
        }

        private void CheckPathForWord(List<Border> path)
        {
            StringBuilder builder1 = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            for (int i = 0; i < path.Count(); ++i)
            {
                string tileText = ((TextBox)((Border)path[i].Child).Child).Text;
                // Suffix.
                if (tileText[0] == '-')
                {
                    builder1.Append(tileText.Substring(1));
                    builder2.Append(tileText.Substring(1));
                }
                // Prefix.
                else if (tileText[tileText.Count() - 1] == '-')
                {
                    builder1.Append(tileText.Substring(0, tileText.Count() - 1));
                    builder2.Append(tileText.Substring(0, tileText.Count() - 1));
                }
                else if (tileText.Count() == 3 && (tileText[1] == '/' || tileText[1] == '\\'))
                {
                    builder1.Append(tileText[0]);
                    builder2.Append(tileText[2]);
                }
                else
                {
                    builder1.Append(tileText);
                    builder2.Append(tileText);
                }
            }
            bool found = false;
            bool alreadyFound = false;
            string string1 = builder1.ToString();
            string string2 = builder2.ToString();
            for (int i = 0; i < solutionWordPointPaths.Count(); ++i)
            {
                if (string1 == solutionWordPointPaths[i].word)
                {
                    if (!foundWordPointPaths.Contains(solutionWordPointPaths[i]))
                    {
                        foundWordPointPaths.Add(solutionWordPointPaths[i]);
                        found = true;
                    }
                    else
                    {
                        alreadyFound = true;
                    }
                    break;
                }
            }
            // Either/or tile was chosen.
            if (string1 != string2 && !found)
            {
                for (int i = 0; i < solutionWordPointPaths.Count(); ++i)
                {
                    if (string2 == solutionWordPointPaths[i].word)
                    {
                        if (!foundWordPointPaths.Contains(solutionWordPointPaths[i]))
                        {
                            foundWordPointPaths.Add(solutionWordPointPaths[i]);
                            found = true;
                        }
                        else
                        {
                            alreadyFound = true;
                        }
                        break;
                    }
                }
            }
            if (string1.Count() < 3)
            {
                for (int i = 0; i < currentPath.Count(); i++)
                {
                    UncolorTile(currentPath[i]);
                }
                return;
            }
            if (found)
            {
                AnimateWordSubmission(path, Colors.Green);
                foundPoints += foundWordPointPaths[foundWordPointPaths.Count() - 1].points;
                pointsLabel.Content = foundPoints + " of " + totalPoints + " points";
                wordsLabel.Content = foundWordPointPaths.Count() + " of " + solutionWordPointPaths.Count() + " words";
                WordPointPath.SortWordPointPaths(comboBox1.SelectedIndex, foundWordPointPaths);
                Display(foundBox, foundWordPointPaths);
            }
            else if (alreadyFound)
            {
                AnimateWordSubmission(path, Color.FromArgb(255, 205, 189, 31));
            }
            else
            {
                AnimateWordSubmission(path, Colors.Red);
            }
        }

        #endregion

        #region Coloring and animation

        private void ColorTile(Border parentBorder, SolidColorBrush brush)
        {
            Border childBorder = (Border)parentBorder.Child;
            parentBorder.Background = parentBorder.BorderBrush = childBorder.Background = childBorder.BorderBrush = brush;
            brush = new SolidColorBrush(Colors.Black);
            TextBox letterTextBox = (TextBox)(childBorder.Child);
            letterTextBox.Foreground = brush;
            TextBox pointTextBox = this.FindName("point" + letterTextBox.Name.Substring(7)) as TextBox;
            pointTextBox.Foreground = brush;
        }

        private void UncolorTile(Border parentBorder)
        {
            Border childBorder = (Border)parentBorder.Child;
            SolidColorBrush brush = new SolidColorBrush(Colors.DarkOrange);
            parentBorder.Background = parentBorder.BorderBrush = childBorder.Background = childBorder.BorderBrush = brush;
            brush = new SolidColorBrush(Colors.White);
            TextBox letterTextBox = (TextBox)(childBorder.Child);
            letterTextBox.Foreground = brush;
            TextBox pointTextBox = this.FindName("point" + letterTextBox.Name.Substring(7)) as TextBox;
            pointTextBox.Foreground = brush;
        }

        // Animating to DarkOrange from Green/Gold-ish/Red for Word/Already found word/Not a word
        private void AnimateWordSubmission(List<Border> path, Color color)
        {
            SolidColorBrush borderBrush = new SolidColorBrush(color);
            SolidColorBrush textBrush = new SolidColorBrush(Colors.White);
            for (int i = 0; i < path.Count(); ++i)
            {
                Border childBorder = (Border)path[i].Child;
                path[i].Background = path[i].BorderBrush = childBorder.Background = childBorder.BorderBrush = borderBrush;
                TextBox letterTextBox = (TextBox)(childBorder.Child);
                letterTextBox.Foreground = textBrush;
                TextBox pointTextBox = this.FindName("point" + letterTextBox.Name.Substring(7)) as TextBox;
                pointTextBox.Foreground = textBrush;
            }
            // Animate back (all at once -- they're sharing references to SolidColorBrush) to DarkOrange.
            ColorAnimation animation = new ColorAnimation();
            animation.To = Colors.DarkOrange;
            animation.AccelerationRatio = .99;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1.2));
            borderBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        #endregion

        // Event handler for Window-wide events: MouseLeftButtonUp, MouseLeftButtonDown, MouseEnter.
        private void Window_CheckMouseLeftButtonState(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                mouseLeftButtonPressed = true;
            }
            else
            {
                mouseLeftButtonPressed = false;
            }
            if (!mouseLeftButtonPressed && _started)
            {

                CheckPathForWord(currentPath);
                currentPath.Clear();
            }
        }

        // This is bad, but will hopefully work (display all text) AND look decent (not too small) almost all the time.
        // However, I sacrifice the former for the latter. I tried to work with widths briefly but didn't get anywhere useful.
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox temp = (TextBox)sender;

            if (temp.Text.Count() >= 5)
            {
                temp.FontSize = 24;
            }
            else if (temp.Text.Count() >= 4)
            {
                temp.FontSize = 36;
            }
            else if (temp.Text.Count() >= 3)
            {
                temp.FontSize = 48;
            }
            else if (temp.Text.Count() >= 2)
            {
                temp.FontSize = 48;
            }
            else
            {
                temp.FontSize = 72;
            }
        }

        // Displaying words in one of the two ListBoxes.
        private void Display(ListBox box, List<WordPointPath> wordPointPaths)
        {
            box.Items.Clear();
            for (int i = 0; i < wordPointPaths.Count(); ++i)
            {
                box.Items.Add(wordPointPaths[i].points + " " + wordPointPaths[i].word.ToLower());
            }
        }

        // 00:00:00 is default display. This just cuts off excess zeroes until they're necessary.
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            seconds++;
            TimeSpan temp = TimeSpan.FromSeconds(seconds);
            if (temp.TotalMinutes < 10)
            {
                timerLabel.Content = temp.ToString().Substring(4);
            }
            else if (temp.TotalHours < 1)
            {
                timerLabel.Content = temp.ToString().Substring(3);
            }
            else
            {
                timerLabel.Content = temp.ToString().Substring(1);
            }
        }

        // Displaying a word on the board with a color gradient, other tiles made transparent.
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = (ListBox)sender;
            int selectedIndex = box.SelectedIndex;
            if (selectedIndex == -1 || box.Items[selectedIndex].ToString().Count() == 0)
            {
                return;
            }
            List<WordPointPath> wordPointPaths;
            if (box.Name == foundBox.Name)
            {
                wordPointPaths = foundWordPointPaths;
            }
            else
            {
                wordPointPaths = solutionWordPointPaths;
            }
            string word = (box.Items[box.SelectedIndex].ToString().Split(' '))[1].ToUpper();
            for (int i = 0; i < wordPointPaths.Count(); ++i)
            {
                if (word == wordPointPaths[i].word)
                {
                    currentWordLabel.Content = word;
                    List<int> path = wordPointPaths[i].path;
                    List<Color> colorGradient = Helpersa.CreateColorGradient(Colors.LightGreen, Colors.Tomato, path.Count());
                    for (int j = 0; j < path.Count(); ++j)
                    {
                        Border pborder = this.FindName("pborder" + (path[j] + 1)) as Border;
                        SolidColorBrush brush = new SolidColorBrush(colorGradient[j]);
                        ColorTile(pborder, brush);
                    }
                    for (int j = 0; j < 16; ++j)
                    {
                        if (!path.Contains(j))
                        {
                            Border pborder = this.FindName("pborder" + (j + 1)) as Border;
                            SolidColorBrush brush = new SolidColorBrush(Colors.White);
                            brush.Opacity = 0;
                            ColorTile(pborder, brush);
                        }
                    }
                    break;
                }
            }
        }

        // Letting go of LMB when over one of these controls (buttons/comboBox) with a path selected doesn't submit the path.
        // Submit it once the mouse leaves the control.
        private void Control_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                if (_started && currentPath.Count() != 0)
                {
                    mouseLeftButtonPressed = false;
                    CheckPathForWord(currentPath);
                    currentPath.Clear();
                }
            }
        }

        // I want to move this out of MainWindow.xaml.cs (into a partial class MainWindow) but trying to causes problems.
        private void ComputePoints()
        {
            int[] pointsBoard = new int[PracticeView2.dim * PracticeView2.dim];
            for (int i = 1; i <= PracticeView2.dim * PracticeView2.dim; ++i)
            {
                pointsBoard[i - 1] = Convert.ToInt32((this.FindName("point" + i.ToString()) as TextBox).Text);
            }
            for (int i = 0; i < words.Count(); ++i)
            {
                points.Add(new List<int>());
                for (int j = 0; j < words[i].Count(); ++j)
                {
                    points[i].Add(WordPointPath.ComputePointsHelper(words[i][j].Count(), paths[i][j], pointsBoard));
                }
            }
        }

        #region Order by: ComboBox

        private void comboBox1_DropDownOpened(object sender, EventArgs e)
        {
            comboBox1.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            comboBox1.Foreground = new SolidColorBrush(Colors.White);
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WordPointPath.SortWordPointPaths(comboBox1.SelectedIndex, solutionWordPointPaths);
            WordPointPath.SortWordPointPaths(comboBox1.SelectedIndex, foundWordPointPaths);
            if (foundBox.Items.Count != 0)
            {
                Display(foundBox, foundWordPointPaths);
            }
            if (solutionBox.Items.Count != 0)
            {
                Display(solutionBox, solutionWordPointPaths);
            }
        }

        #endregion

        #region File Menu

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            //dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".text"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    string[] stringBoard = new string[dim * dim];
                    string[] pointsBoard = new string[dim * dim];
                    for (int i = 1; i <= dim * dim; ++i)
                    {
                        stringBoard[i - 1] = ((TextBox)this.FindName("textbox" + i.ToString())).Text;
                        pointsBoard[i - 1] = ((TextBox)this.FindName("point" + i.ToString())).Text;
                    }
                    for (int i = 0; i < dim * dim; ++i)
                    {
                        writer.WriteLine(stringBoard[i]);
                    }
                    for (int i = 0; i < dim * dim; ++i)
                    {
                        writer.WriteLine(pointsBoard[i]);
                    }
                }
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Text documents (.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                clearButton_Click(null, new RoutedEventArgs());
                // Load document
                string filename = dlg.FileName;
                using (StreamReader reader = new StreamReader(filename))
                {
                    string[] stringBoard = new string[dim * dim];
                    string[] pointsBoard = new string[dim * dim];
                    for (int i = 1; i <= dim * dim; ++i)
                    {
                        stringBoard[i - 1] = reader.ReadLine();
                    }
                    for (int i = 1; i <= dim * dim; ++i)
                    {
                        pointsBoard[i - 1] = reader.ReadLine();
                    }
                    for (int i = 1; i <= dim * dim; ++i)
                    {
                        ((TextBox)this.FindName("textbox" + i.ToString())).Text = stringBoard[i - 1];
                        ((TextBox)this.FindName("point" + i.ToString())).Text = pointsBoard[i - 1];
                    }
                }
            }
        }

        #endregion

        #region Fields

        public const int dim = 4;
        private static Dictionary<char, int> basicTileValues = new Dictionary<char, int>
                                               {{'A', 2}, {'B', 5}, {'C', 3}, {'D', 3}, {'E', 1}, {'F', 5}, {'G', 4}, {'H', 4}, {'I', 2},
                                               {'J', 10}, {'K', 6}, {'L', 3}, {'M', 4}, {'N', 2}, {'O', 2}, {'P', 4}, {'Q', 8},
                                               {'R', 2}, {'S', 2}, {'T', 2}, {'U', 4}, {'V', 6}, {'W', 6}, {'X', 9}, {'Y', 5}, {'Z', 8}};
        private bool mouseLeftButtonPressed = false;

        private DispatcherTimer timer = new DispatcherTimer();
        private int seconds = 0;

        // List<string>s within the List<List<string>> contain strings beginning with the same letter.
        private List<List<string>> words = new List<List<string>>();
        // Each string has a corresponding List<int>; its path through the board.
        private List<List<List<int>>> paths = new List<List<List<int>>>();
        // Each string has a corresponding int; its point value.
        private List<List<int>> points = new List<List<int>>();

        private List<WordPointPath> solutionWordPointPaths = new List<WordPointPath>();
        private List<WordPointPath> foundWordPointPaths = new List<WordPointPath>();
        private List<Border> currentPath = new List<Border>();

        private int totalPoints = 0;
        private int foundPoints = 0;

        #endregion
    }
}
