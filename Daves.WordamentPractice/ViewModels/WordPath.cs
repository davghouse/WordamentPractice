using Daves.WordamentSolver;

namespace Daves.WordamentPractice
{
    public class WordPath
    {
        public WordPath(Word word, Path path)
        {
            Word = word;
            Path = path;
            Points = word.GetPoints(path);
        }

        public Word Word { get; }
        public Path Path { get; }
        public int Points { get; }

        public override string ToString()
            => $"{Points}\t{Word.String}";
    }
}
