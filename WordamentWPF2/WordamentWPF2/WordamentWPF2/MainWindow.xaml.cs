﻿using System;
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
      if (!mouseLeftButtonPressed)
      {
        foreach (var c in Board.Children)
        {
          if (c.GetType() == typeof(Border) && ((Border)c).Child.GetType() == typeof(Border))
          {
            Border parentBorder = (Border)c;
            Border childBorder = (Border)((Border)c).Child;
            parentBorder.Background = new SolidColorBrush(Colors.DarkOrange);
            parentBorder.BorderBrush = new SolidColorBrush(Colors.DarkOrange);
            childBorder.Background = new SolidColorBrush(Colors.DarkOrange);
            childBorder.BorderBrush = new SolidColorBrush(Colors.DarkOrange);
          }
        }
      }
    }

    private void ParentBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      Border parentBorder = (Border)sender;
      Border childBorder = (Border)parentBorder.Child;
      parentBorder.Background = new SolidColorBrush(Colors.LightGreen);
      parentBorder.BorderBrush = new SolidColorBrush(Colors.LightGreen);
      childBorder.Background = new SolidColorBrush(Colors.LightGreen);
      childBorder.BorderBrush = new SolidColorBrush(Colors.LightGreen);
    }

    private void ChildBorder_MouseEnter(object sender, MouseEventArgs e)
    {
      if (mouseLeftButtonPressed)
      {

        Border childBorder = (Border)sender;
        Border parentBorder = (Border)childBorder.Parent;
        parentBorder.Background = new SolidColorBrush(Colors.LightGreen);
        parentBorder.BorderBrush = new SolidColorBrush(Colors.LightGreen);
        childBorder.Background = new SolidColorBrush(Colors.LightGreen);
        childBorder.BorderBrush = new SolidColorBrush(Colors.LightGreen);
      }
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

    private void startButton_Click(object sender, RoutedEventArgs e)
    {
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
            // No parent/child relationship for point TextBoxes, so it's necessary to loop through looking for the correct Rectangle.
            foreach (var d in Board.Children)
            {
              if (d.GetType() == typeof(Border))
              {
                TextBox letterTextBox  = (TextBox)((Border)((Border)d).Child).Child;
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
                  break;
                }
              }
            }
          }
        }
      }
    }

    #region Fields

    private bool mouseLeftButtonPressed = false;
    private static Dictionary<char, int> basicTileValues = new Dictionary<char, int>
                                               {{'A', 2}, {'B', 5}, {'C', 3}, {'D', 3}, {'E', 1}, {'F', 5}, {'G', 4}, {'H', 4}, {'I', 2}, 
                                               {'J', 10}, {'K', 6}, {'L', 3}, {'M', 4}, {'N', 2}, {'O', 2}, {'P', 4}, {'Q', 8}, 
                                               {'R', 2}, {'S', 2}, {'T', 2}, {'U', 4}, {'V', 6}, {'W', 6}, {'X', 9}, {'Y', 5}, {'Z', 8}};
    #endregion
  }
}

