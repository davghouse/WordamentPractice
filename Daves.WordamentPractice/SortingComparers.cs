 ﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Daves.WordamentPractice
{
  // Alphabet.
  class WordPointPathComparer0 : IComparer<WordPointPath>
  {
    public int Compare(WordPointPath lhs, WordPointPath rhs)
    {
      return lhs.word.CompareTo(rhs.word);
    }
  }

  // Points.
  class WordPointPathComparer1 : IComparer<WordPointPath>
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
  class WordPointPathComparer2 : IComparer<WordPointPath>
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
  class WordPointPathComparer3 : IComparer<WordPointPath>
  {
    public int Compare(WordPointPath lhs, WordPointPath rhs)
    {
      double a = WordPointPath.PhysicalPathLength(lhs.path);
      double b = WordPointPath.PhysicalPathLength(rhs.path);
      if (a != b)
      {
        return b.CompareTo(a);
      }
      return lhs.word.CompareTo(rhs.word);
    }
  }

  // Points / word length.
  class WordPointPathComparer4 : IComparer<WordPointPath>
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
  class WordPointPathComparer5 : IComparer<WordPointPath>
  {
    public int Compare(WordPointPath lhs, WordPointPath rhs)
    {
      double a = lhs.points / WordPointPath.PhysicalPathLength(lhs.path);
      double b = rhs.points / WordPointPath.PhysicalPathLength(rhs.path);
      if (a != b)
      {
        return b.CompareTo(a);
      }
      return lhs.word.CompareTo(rhs.word);
    }
  }

  // Paths are stored backwards, hence looking at the final element in the following IComparers.
  // Start position by: points.
  class WordPointPathComparer6 : IComparer<WordPointPath>
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
  class WordPointPathComparer7 : IComparer<WordPointPath>
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
  class WordPointPathComparer8 : IComparer<WordPointPath>
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
  class WordPointPathComparer9 : IComparer<WordPointPath>
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
  class WordPointPathComparer10 : IComparer<WordPointPath>
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
  class WordPointPathComparer11 : IComparer<WordPointPath>
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
  class WordPointPathComparer12 : IComparer<WordPointPath>
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
  class WordPointPathComparer13 : IComparer<WordPointPath>
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
  class WordPointPathComparer14 : IComparer<WordPointPath>
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
  class WordPointPathComparer15 : IComparer<WordPointPath>
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
}