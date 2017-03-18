using nBayes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClickbaitDetector.PoC
{
    class Program
    {
        static FileIndex _clickbait; // Index.CreateMemoryIndex();
        static FileIndex _genuine; //Index.CreateMemoryIndex();

        static int _clickbaitCorrect = 0;
        static int _clickbaitWrong = 0;
        static int _genuineCorrect = 0;
        static int _genuineWrong = 0;

        static string Language => "Dutch";

        static void Main(string[] args)
        {
            _clickbait = new FileIndex($"{Language}/clickbait.xml");
            _clickbait.Open();

            _genuine = new FileIndex($"{Language}/genuine.xml");
            _genuine.Open();

            // train clickbait
            TrainClickbait();
            _clickbait.Save();

            // train genuine
            TrainGenuine();
            _genuine.Save();

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Clickbait      : {_clickbait.EntryCount} total |" +
                    $" {_clickbaitCorrect} right |" +
                    $" {_clickbaitWrong} wrong");

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Genuine        : {_genuine.EntryCount} total |" +
                    $" {_genuineCorrect} right |" +
                    $" {_genuineWrong} wrong");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();

                Console.WriteLine("Enter the sentence to test:");
                string word = Console.ReadLine();
                var result = AnalyzeWord(word);

                Console.WriteLine("Enter 1 for Clickbait, 2 for Genuine or ESC to quit.");

                var option = Console.ReadKey(true);
                if (option.Key == ConsoleKey.D1)
                {
                    // add to clickbait
                    AddClickbait(word);
                    UpdateCounter(result, CategorizationResult.First);
                }
                else if (option.Key == ConsoleKey.D2)
                {
                    // add to genuine
                    AddGenuine(word);
                    UpdateCounter(result, CategorizationResult.Second);
                }
                else if (option.Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Invalid key");
                }
            }

        }

        private static void UpdateCounter(CategorizationResult result, CategorizationResult actual)
        {
            switch (result)
            {
                case CategorizationResult.First when actual == CategorizationResult.First:
                    _clickbaitCorrect++;
                    break;
                case CategorizationResult.First when actual == CategorizationResult.Second:
                    _clickbaitWrong++;
                    break;
                case CategorizationResult.Second when actual == CategorizationResult.Second:
                    _genuineCorrect++;
                    break;
                case CategorizationResult.Second when actual == CategorizationResult.First:
                    _genuineWrong++;
                    break;
            }
        }

        private static void AddClickbait(string word)
        {
            _clickbait.Add(Entry.FromString(word));
            _clickbait.Save();

            Console.WriteLine("Sentence has been added to the clickbait list.");
        }

        private static void AddGenuine(string word)
        {
            _genuine.Add(Entry.FromString(word));
            _genuine.Save();

            Console.WriteLine("Sentence has been added to the genuine list.");
        }

        private static CategorizationResult AnalyzeWord(string word)
        {
            Analyzer analyzer = new Analyzer();
            CategorizationResult result = analyzer.Categorize(
                Entry.FromString(word),
                 _clickbait,
                 _genuine);

            switch (result)
            {
                case CategorizationResult.First:
                    Console.WriteLine("Clickbait");
                    break;
                case CategorizationResult.Undetermined:
                    Console.WriteLine("Undecided");
                    break;
                case CategorizationResult.Second:
                    Console.WriteLine("Genuine");
                    break;
            }

            return result;
        }

        static void TrainClickbait()
        {
            // open training file and import into index
            string path = $"{Language}/train-clickbait.txt";
            if (!File.Exists(path))
                return;

            var lines = File.ReadLines(path);
            foreach(string line in lines)
            {
                _clickbait.Add(Entry.FromString(line));
            };
        }

        static void TrainGenuine()
        {
            // open training file and import into index
            string path = $"{Language}/train-genuine.txt";
            if (!File.Exists(path))
                return;

            var lines = File.ReadLines(path);
            foreach (string line in lines)
            {
                _genuine.Add(Entry.FromString(line));
            };
        }

    }
}
