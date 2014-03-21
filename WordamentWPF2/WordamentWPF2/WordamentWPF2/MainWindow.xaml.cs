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
    // I tried to work with widths briefly but didn't get anywhere useful.
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
      // Make textboxes' isHitTestVisible false, so the game can be played.
      foreach (var c in Board.Children)
      {
        if (c.GetType() == typeof(TextBox))
        {
          TextBox temp = (TextBox)c;
          temp.IsHitTestVisible = false;
        }
        if (c.GetType() == typeof(Border))
        {
          TextBox temp = (TextBox)((Border)((Border)c).Child).Child;
          temp.IsHitTestVisible = false;
        }
      }
    }

    #region Fields
    private bool mouseLeftButtonPressed = false;
    #endregion
  }
}

