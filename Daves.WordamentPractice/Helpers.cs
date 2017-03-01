﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Daves.WordamentPractice
{
  class Helpersa
  {
    // Converting to points in a grid and finding the Euclidean distance between them.
    public static bool tilesAreAdjacent(int i, int j)
    {
      --i; --j;
      int ri = i / 4;
      int ci = i - ri * 4;
      int rj = j / 4;
      int cj = j - rj * 4;
      double dist = Math.Sqrt((double)((ri - rj) * (ri - rj) + (ci - cj) * (ci - cj)));
      return (dist <= (Math.Sqrt(2.0) + .01));
    }

    public static List<Color> CreateColorGradient(Color a, Color b, int steps)
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
  }
}