﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace Daves.WordamentPractice
{
  class WordPointPath
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

    public static double PhysicalPathLength(List<int> path)
    {
      double pathLength = 0;
      for (int i = 0; i < path.Count() - 1; ++i)
      {
        int a = path[i];
        int b = path[i + 1];
        if (a + 1 == b || a - 1 == b || a + MainWindow.dim == b || a - MainWindow.dim == b)
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

    public static int ComputePointsHelper(int wordLength, List<int> path, int[] pointsBoard)
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

    public static void SortWordPointPaths(int index, List<WordPointPath> wordPointPaths)
    {
      switch (index)
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
  }
}
