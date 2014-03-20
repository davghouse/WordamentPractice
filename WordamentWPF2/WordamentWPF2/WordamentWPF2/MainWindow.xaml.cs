using System;
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
          if (c.GetType() == typeof(Border))
          {
            Border bord = (Border)c;
            Rectangle rect = (Rectangle)((Border)c).Child;
            bord.Background = new SolidColorBrush(Colors.DarkOrange);
            bord.BorderBrush = new SolidColorBrush(Colors.DarkOrange);
            rect.Fill = new SolidColorBrush(Colors.DarkOrange);
            rect.Stroke = new SolidColorBrush(Colors.DarkOrange);
          }
        }
      }
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      foreach (var c in Board.Children)
      {
        if (c.GetType() == typeof(Border) && ((Border)c).Name == ((Border)sender).Name)
        {
          Border bord = (Border)c;
          Rectangle rect = (Rectangle)((Border)c).Child;
          bord.Background = new SolidColorBrush(Colors.LightGreen);
          bord.BorderBrush = new SolidColorBrush(Colors.LightGreen);
          rect.Fill = new SolidColorBrush(Colors.LightGreen);
          rect.Stroke = new SolidColorBrush(Colors.LightGreen);
          return;
        }
      }
    }

    private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
    {
      if (mouseLeftButtonPressed)
      {
        foreach (var c in Board.Children)
        {
          if (c.GetType() == typeof(Border) && ((Rectangle)((Border)c).Child).Name == ((Rectangle)sender).Name)
          {
            Border bord = (Border)c;
            Rectangle rect = (Rectangle)((Border)c).Child;
            bord.Background = new SolidColorBrush(Colors.LightGreen);
            bord.BorderBrush = new SolidColorBrush(Colors.LightGreen);
            rect.Fill = new SolidColorBrush(Colors.LightGreen);
            rect.Stroke = new SolidColorBrush(Colors.LightGreen);
            return;
          }
        }
      }
    }

    private bool mouseLeftButtonPressed = false;

  }
}
