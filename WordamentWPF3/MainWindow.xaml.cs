﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Timers;

         

namespace WordamentWPF2
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      timer.Tick += new EventHandler(timer_Elapsed);
      timer.Interval = new TimeSpan(0, 0, 1);
      comboBox1.SelectedIndex = 1;
    }

    // Event handler for Window-wide events: MouseLeftButtonUp, MouseLeftButtonDown, MouseEnter
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
      if (!mouseLeftButtonPressed && started)
      {
        
        CheckPathForWord(currentPath);
        currentPath.Clear();
      }
    }

    void CheckPathForWord(List<Border> path)
    {
      StringBuilder builder1 = new StringBuilder();
      StringBuilder builder2 = new StringBuilder();
      for (int i = 0; i < path.Count(); ++i)
      {
        string tileText = ((TextBox)((Border)path[i].Child).Child).Text;
        // Suffix
        if (tileText[0] == '-')
        {
          builder1.Append(tileText.Substring(1));
          builder2.Append(tileText.Substring(1));
        }
        // Prefix
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
        SortWordPointPaths(foundWordPointPaths);
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

    private void AnimateWordSubmission(List<Border> path, Color color)
    {
        SolidColorBrush borderBrush = new SolidColorBrush(color);
        SolidColorBrush textBrush = new SolidColorBrush(Colors.White);
        for (int i = 0; i < path.Count(); ++i)
        {
          // Color it Green.
          //SolidColorBrush brush = new SolidColorBrush(Colors.Green);
          Border childBorder = (Border)path[i].Child;
          path[i].Background = path[i].BorderBrush = childBorder.Background = childBorder.BorderBrush = borderBrush;
          //borderBrush = new SolidColorBrush(Colors.White);
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

    private void ParentBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      Border parentBorder = (Border)sender;
      if ((currentPath.Count() == 0) && started)
      {
        currentPath.Add(parentBorder);
        ColorTile(parentBorder, new SolidColorBrush(Colors.White));
      }
    }

    private void ChildBorder_MouseEnter(object sender, MouseEventArgs e)
    {
      if (mouseLeftButtonPressed && !textbox1.IsHitTestVisible && started)
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
          if (tilesAreAdjacent(tileIndex, previousTileIndex))
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

    // Converting to points in a grid and finding the Euclidean distance between them.
    private bool tilesAreAdjacent(int i, int j)
    {
      --i; --j;
      int ri = i / 4;
      int ci = i - ri * 4;
      int rj = j / 4;
      int cj = j - rj * 4;
      double dist = Math.Sqrt((double)((ri - rj) * (ri - rj) + (ci - cj) * (ci - cj)));
      return (dist <= (Math.Sqrt(2.0) + .01));
    }

    // This is bad, but will hopefully work (display all text) AND look decent (not too small) almost all the time.
    // However, I dangerously sacrifice the former for the latter. I tried to work with widths briefly but didn't get anywhere useful.
    private void textbox_TextChanged(object sender, TextChangedEventArgs e)
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

    private void solveButton_Click(object sender, RoutedEventArgs e)
    {
      Display(solutionBox, solutionWordPointPaths);
    }

    private void startButton_Click(object sender, RoutedEventArgs e)
    {
      started = true;
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
          words = WordamentRecursiveOOSolver.Solver.RunSolver(stringBoard, paths);
          ComputePoints(words, paths, points);
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

        // Good place to use reflection?
        SortWordPointPaths(solutionWordPointPaths);
        
        for (int i = 0; i < solutionWordPointPaths.Count(); ++i)
        {
          totalPoints += solutionWordPointPaths[i].points;
        }
        pointsLabel.Content = "0 of " + totalPoints + " points";
        wordsLabel.Content = "0 of " + solutionWordPointPaths.Count() + " words";
      }
    }

    private void SortWordPointPaths(List<WordPointPath> wordPointPaths)
    {
      switch (comboBox1.SelectedIndex)
      {
        case 0:
          wordPointPaths.Sort(new WordPointPathComparer0());
          break;
        case 1:
          wordPointPaths.Sort(new WordPointPathComparer1());
          break;
        case 2:
          wordPointPaths.Sort(new WordPointPathComparer2());
          break;
        case 3:
          wordPointPaths.Sort(new WordPointPathComparer3());
          break;
        case 4:
          wordPointPaths.Sort(new WordPointPathComparer4());
          break;
        case 5:
          wordPointPaths.Sort(new WordPointPathComparer5());
          break;
        case 6:
          wordPointPaths.Sort(new WordPointPathComparer6());
          break;
        case 7:
          wordPointPaths.Sort(new WordPointPathComparer7());
          break;
        case 8:
          wordPointPaths.Sort(new WordPointPathComparer8());
          break;
        case 9:
          wordPointPaths.Sort(new WordPointPathComparer9());
          break;
        case 10:
          wordPointPaths.Sort(new WordPointPathComparer10());
          break;
        case 11:
          wordPointPaths.Sort(new WordPointPathComparer11());
          break;
        case 12:
          wordPointPaths.Sort(new WordPointPathComparer12());
          break;
        case 13:
          wordPointPaths.Sort(new WordPointPathComparer13());
          break;
        case 14:
          wordPointPaths.Sort(new WordPointPathComparer14());
          break;
        case 15:
          wordPointPaths.Sort(new WordPointPathComparer15());
          break;
        default:
          break;
      }
    }

    private void ComputePoints(List<List<string>> words, List<List<List<int>>> paths, List<List<int>> points)
    {
      int[] pointsBoard = new int[dim * dim];
      for (int i = 1; i <= dim * dim; ++i)
      {
        pointsBoard[i - 1] = Convert.ToInt32((this.FindName("point" + i.ToString()) as TextBox).Text);
      }
      for (int i = 0; i < words.Count(); ++i)
      {
        points.Add(new List<int>());
        for (int j = 0; j < words[i].Count(); ++j)
        {
          points[i].Add(ComputePointsHelper(words[i][j].Count(), paths[i][j], pointsBoard));
        }
      }
    }

    private int ComputePointsHelper(int wordLength, List<int> path, int[] pointsBoard)
    {
      int ret = 0;
      for (int i = 0; i < path.Count(); ++i)
      {
        ret += pointsBoard[path[i]];
      }
      if (wordLength >= 8)
      {
        return (int)(2.5 * ret);
      }
      if (wordLength >= 6)
      {
        return 2 * ret;
      }
      if (wordLength >= 5)
      {
        return (int)(1.5 * ret);
      }
      return ret;
    }

    private void Display(ListBox box, List<WordPointPath> wordPointPaths)
    {
      box.Items.Clear();
      for (int i = 0; i < wordPointPaths.Count(); ++i)
      {
        box.Items.Add(wordPointPaths[i].points + " " + wordPointPaths[i].word.ToLower());
      }
    }


    private void timer_Elapsed(object sender, EventArgs e)
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

    private void clearButton_Click(object sender, RoutedEventArgs e)
    {
      started = false;
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
      SortWordPointPaths(solutionWordPointPaths);
      SortWordPointPaths(foundWordPointPaths);
      if (foundBox.Items.Count != 0)
      {
        Display(foundBox, foundWordPointPaths);
      }
      if (solutionBox.Items.Count != 0)
      {
        Display(solutionBox, solutionWordPointPaths);
      }
    }

    private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ListBox box = (ListBox)sender;
      foreach (var c in Board.Children)
      {
        if (c.GetType() == typeof(Border))
        {
          UncolorTile((Border)c);
        }
      }
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
          List<int> path = wordPointPaths[i].path;
          List<Color> colorGradient = CreateColorGradient(Colors.LightGreen, Colors.Tomato, path.Count());
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
      justSelectedListBox = true;
    }

    private List<Color> CreateColorGradient(Color a, Color b, int steps)
    {
      int rMax = a.R;
      int rMin = b.R;
      int gMax = a.G;
      int gMin = b.G;
      int bMax = a.B;
      int bMin = b.B;
      List<Color> colorGradient = new List<Color>();
      int denominator = steps - 1;
      if (denominator == 0)
      {
        denominator = 1;
      }
      for (int i = 0; i < steps; ++i)
      {
        int rAverage = rMin + (int)((rMax - rMin) * i / (denominator));
        int gAverage = gMin + (int)((gMax - gMin) * i / (denominator));
        int bAverage = bMin + (int)((bMax - bMin) * i / (denominator));
        colorGradient.Add(Color.FromArgb(255, (byte)rAverage, (byte)gAverage, (byte)bAverage));
      }
      return colorGradient;
    }

    private void listBox_MouseLeave(object sender, MouseEventArgs e)
    {
      if (justSelectedListBox)
      {
        foreach (var c in Board.Children)
        {
          if (c.GetType() == typeof(Border))
          {
            UncolorTile((Border)c);
          }
        }
        ((ListBox)sender).SelectedIndex = -1;
        justSelectedListBox = false;
      }
    }

    private void button_MouseLeave(object sender, MouseEventArgs e)
    {
      if (Mouse.LeftButton != MouseButtonState.Pressed)
      {
        if (started && currentPath.Count() != 0)
        {
          mouseLeftButtonPressed = false;
          CheckPathForWord(currentPath);
          currentPath.Clear();
        }
      }
    }

    private class WordPointPath
    {
      public WordPointPath(string word, int points, List<int> path)
      {
        this.word = word;
        this.points = points;
        this.path = path;
      }
      public string word;
      public int points;
      public List<int> path;
    }

    #region Comparers

    // Alphabet.
    private class WordPointPathComparer0 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        return lhs.word.CompareTo(rhs.word);
      }
    }

    // Points.
    private class WordPointPathComparer1 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        if (lhs.points != rhs.points)
        {
          return rhs.points.CompareTo(lhs.points);
        }
        return lhs.word.CompareTo(rhs.word);
      }
    }

    // Word length.
    private class WordPointPathComparer2 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        if (lhs.word.Count() != rhs.word.Count())
        {
          return rhs.word.Count().CompareTo(lhs.word.Count());
        }
        return lhs.word.CompareTo(rhs.word);
      }
    }

    // Path length.
    private class WordPointPathComparer3 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        double a = PhysicalPathLength(lhs.path);
        double b = PhysicalPathLength(rhs.path);
        if (a != b)
        {
          return b.CompareTo(a);
        }
        return lhs.word.CompareTo(rhs.word);
      }
    }

    // Points / word length.
    private class WordPointPathComparer4 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        double a = lhs.points / lhs.word.Count();
        double b = rhs.points / rhs.word.Count();
        if (a != b)
        {
          return b.CompareTo(a);
        }
        return lhs.word.CompareTo(rhs.word);
      }
    }

    // Points / path length.
    private class WordPointPathComparer5 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        double a = lhs.points / PhysicalPathLength(lhs.path);
        double b = rhs.points / PhysicalPathLength(rhs.path);
        if (a != b)
        {
          return b.CompareTo(a);
        }
        return lhs.word.CompareTo(rhs.word);
      }
    }

    // Paths are stored backwards, hence looking at the final element in the following IComparers.
    // Start position by: points.
    private class WordPointPathComparer6 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        int a = lhs.path[lhs.path.Count() - 1];
        int b = rhs.path[rhs.path.Count() - 1];
        if (a != b)
        {
          return a.CompareTo(b);
        }
        return (new WordPointPathComparer1()).Compare(lhs, rhs);
      }
    }

    // Start position by: word length.
    private class WordPointPathComparer7 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        int a = lhs.path[lhs.path.Count() - 1];
        int b = rhs.path[rhs.path.Count() - 1];
        if (a != b)
        {
          return a.CompareTo(b);
        }
        return (new WordPointPathComparer2()).Compare(lhs, rhs);
      }
    }

    // Start position by: points / word length.
    private class WordPointPathComparer8 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        int a = lhs.path[lhs.path.Count() - 1];
        int b = rhs.path[rhs.path.Count() - 1];
        if (a != b)
        {
          return a.CompareTo(b);
        }
        return (new WordPointPathComparer4()).Compare(lhs, rhs);
      }
    }

    // Start position by: points / path length.
    private class WordPointPathComparer9 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        int a = lhs.path[lhs.path.Count() - 1];
        int b = rhs.path[rhs.path.Count() - 1];
        if (a != b)
        {
          return a.CompareTo(b);
        }
        return (new WordPointPathComparer5()).Compare(lhs, rhs);
      }
    }

    // A, B, ... by: points.
    private class WordPointPathComparer10 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        int a = lhs.word[0];
        int b = rhs.word[0];
        if (a != b)
        {
          return a.CompareTo(b);
        }
        return (new WordPointPathComparer1()).Compare(lhs, rhs);
      }
    }

    // A, B, ... by: word length.
    private class WordPointPathComparer11 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        int a = lhs.word[0];
        int b = rhs.word[0];
        if (a != b)
        {
          return a.CompareTo(b);
        }
        return (new WordPointPathComparer2()).Compare(lhs, rhs);
      }
    }

    // A, B, ... by: points / word length.
    private class WordPointPathComparer12 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        int a = lhs.word[0];
        int b = rhs.word[0];
        if (a != b)
        {
          return a.CompareTo(b);
        }
        return (new WordPointPathComparer4()).Compare(lhs, rhs);
      }
    }

    // A, B, ... by: points / path length.
    private class WordPointPathComparer13 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        int a = lhs.word[0];
        int b = rhs.word[0];
        if (a != b)
        {
          return a.CompareTo(b);
        }
        return (new WordPointPathComparer5()).Compare(lhs, rhs);
      }
    }

    // Speed round: word length ascending.
    private class WordPointPathComparer14 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        if (lhs.word.Count() != rhs.word.Count())
        {
          return lhs.word.Count().CompareTo(rhs.word.Count());
        }
        return lhs.word.CompareTo(rhs.word);
      }
    }

    // Speed round: start position by word length ascending.
    private class WordPointPathComparer15 : IComparer<WordPointPath>
    {
      public int Compare(WordPointPath lhs, WordPointPath rhs)
      {
        int a = lhs.path[lhs.path.Count() - 1];
        int b = rhs.path[rhs.path.Count() - 1];
        if (a != b)
        {
          return a.CompareTo(b);
        }
        return (new WordPointPathComparer14()).Compare(lhs, rhs);
      }
    }

    private static double PhysicalPathLength(List<int> path)
    {
      double pathLength = 0;
      for (int i = 0; i < path.Count() - 1; ++i)
      {
        int a = path[i];
        int b = path[i + 1];
        if (a + 1 == b || a - 1 == b || a + dim == b || a - dim == b)
        {
          pathLength += 1;
        }
        // Diagonal.
        else
        {
          pathLength += 1.41421356237;
        }
      }
      return pathLength;
    }

    #endregion Comparers

    #region Fields

    private bool started = false;
    private bool mouseLeftButtonPressed = false;
    private const int dim = 4;
    private static Dictionary<char, int> basicTileValues = new Dictionary<char, int>
                                               {{'A', 2}, {'B', 5}, {'C', 3}, {'D', 3}, {'E', 1}, {'F', 5}, {'G', 4}, {'H', 4}, {'I', 2}, 
                                               {'J', 10}, {'K', 6}, {'L', 3}, {'M', 4}, {'N', 2}, {'O', 2}, {'P', 4}, {'Q', 8}, 
                                               {'R', 2}, {'S', 2}, {'T', 2}, {'U', 4}, {'V', 6}, {'W', 6}, {'X', 9}, {'Y', 5}, {'Z', 8}};
    private DispatcherTimer timer = new DispatcherTimer();
    private int seconds = 0;
    private List<Border> currentPath = new List<Border>();

    // List<string>s within the List<List<string>> contain strings beginning with the same letter.
    private List<List<string>> words = new List<List<string>>();

    // Each string has a corresponding List<int>; its path through the board.
    private List<List<List<int>>> paths = new List<List<List<int>>>();

    // Each string has a corresponding int; its point value.
    private List<List<int>> points = new List<List<int>>();

    // Necessary as a way to make sorting easier. 
    private List<WordPointPath> solutionWordPointPaths = new List<WordPointPath>();

    private List<WordPointPath> foundWordPointPaths = new List<WordPointPath>();

    private int totalPoints = 0;
    private int foundPoints = 0;

    private bool justSelectedListBox = false;

    #endregion
  }
}