using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Kontur.Search;

namespace Kontur.ConsoleApp
{
    class Program
    {
        public static readonly SearchManager _searchManager = new SearchManager(SearchManager.CreateDefaultProvider());

#if QUERY_FROM_FILE
        // Char codes in UTF-8
        public const int ZeroCharCode = 48, NineCharCode = 57;
        public const int ACharCode = 65, ZCharCode = 90, aCharCode = 97, zCharCode = 122, SpaceCharCode = 32;
#endif

        static void Main(string[] args)
        {
            var dataFilePath = string.Empty;


            if (args.Length == 0)
            {
                Console.WriteLine("Неверные аргументы, пример: data.ini");
                return;
            }

            dataFilePath = args[0];
            if (!File.Exists(dataFilePath))
            {
                Console.WriteLine("Файл не существует");
                return;
            }

            Console.BufferHeight = short.MaxValue - 1;

            var addingStopwatch = new Stopwatch();
            var totalStopwatch = new Stopwatch();
            var numberOfEntries = default(int);

            addingStopwatch.Start();
            totalStopwatch.Start();

            using (var streamReader = new StreamReader(dataFilePath))
            {
                var counter = 0;

                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();

                    if (numberOfEntries == default(int))
                    {
                        if (!int.TryParse(line, out numberOfEntries))
                            throw new FormatException("Wrong file format");
                    }
                    else if (counter < numberOfEntries - 1)
                    {
                        var splittedLine = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (splittedLine.Length <= 1)
                            continue;

                        var priority = 0;
                        if (!int.TryParse(splittedLine[1], out priority))
                            continue;

                        if (_searchManager.SupportsPriorityNodes)
                        {
                            _searchManager.AddEntry(splittedLine[0], priority);
                            counter++;
                        }
                        else
                        {
                            _searchManager.AddEntry(splittedLine[0]);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            addingStopwatch.Stop();
            Console.WriteLine("Add time: {0}", addingStopwatch.ElapsedMilliseconds);

#if QUERY_FROM_FILE

            var stopwatch = new Stopwatch();
            var stringBuilder = new StringBuilder();
            var skipCount = 0;
            stopwatch.Start();

            using (var streamReader = new StreamReader(dataFilePath))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var byteLine = Encoding.UTF8.GetBytes(line);

                    if (byteLine.Length > 0 && byteLine[0] >= ZeroCharCode && byteLine[0] <= NineCharCode)
                    {
                        skipCount++;
                    }
                    else if (skipCount == 2 && byteLine.Length > 0 && byteLine[0] >= aCharCode && byteLine[0] <= zCharCode)
                    {
                        foreach (var item in _searchManager.Search(line, 10))
                        {
                            stringBuilder.AppendLine(item);
                        }

                        stringBuilder.AppendLine();
                    }
                }
            }
            stopwatch.Stop();

            Console.WriteLine(stringBuilder);
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
#else

            var query = Console.ReadLine();
            foreach (var item in searchManager.Search(query, 10))
            {
                Console.WriteLine(item);
            }
#endif
            totalStopwatch.Stop();
            Console.WriteLine("Total: " + totalStopwatch.ElapsedMilliseconds);
        }
    }
}
