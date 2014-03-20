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
        foreach (var r in Board.Children)
        {
          if (r.GetType() == typeof(Rectangle))
          {
            Rectangle rect = (Rectangle)r;
            rect.Fill = new SolidColorBrush(Colors.DarkOrange);
            rect.Stroke = new SolidColorBrush(Colors.DarkOrange);
          }
        }
      }
    }

    private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      foreach (Rectangle r in Board.Children)
      {
        if (((Rectangle)sender).Name == r.Name)
        {
          r.Fill = new SolidColorBrush(Colors.LightGreen);
          r.Stroke = new SolidColorBrush(Colors.LightGreen);
          return;
        }
      }
    }

    private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
    {
      if (mouseLeftButtonPressed)
      {
        Rectangle rect = (Rectangle)sender;
        foreach (Rectangle r in Board.Children)
        {
          if (rect.Name == r.Name)
          {
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
