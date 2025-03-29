using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelTasks
{
    public class ArrayTasks
    {
        private int[] _array;

        public ArrayTasks(int[] array)
        {
            _array = array;
        }

        public int Min()
        {
            return _array.Min();
        }

        public int Max()
        {
            return _array.Max();
        }

        public int Sum()
        {
            return _array.Sum();
        }

        public double Average()
        {
            return _array.Average();
        }

        public int[] CopyPart(int startIndex, int length)
        {
            return _array.Skip(startIndex).Take(length).ToArray();
        }
    }

    public class TextTasks
    {
        private string _text;

        public TextTasks(string text)
        {
            _text = text;
        }

        public Dictionary<char, int> CharacterFrequency()
        {
            return _text.GroupBy(c => c)
                        .ToDictionary(g => g.Key, g => g.Count());
        }

        public Dictionary<string, int> WordFrequency()
        {
            var words = _text.Split(new[] { ' ', '\n', '\r', '.', ',', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            return words.GroupBy(w => w.ToLower())
                        .ToDictionary(g => g.Key, g => g.Count());
        }
    }

    public class ParallelProcessor
    {
        private int _numThreads;
        private Random _random;

        public ParallelProcessor(int numThreads)
        {
            _numThreads = numThreads;
            _random = new Random();
        }

        public int[] GenerateRandomArray(int size)
        {
            var array = new int[size];
            Parallel.For(0, size, i =>
            {
                array[i] = _random.Next(0, 100);
            });
            return array;
        }

        public void ExecuteTasks(int[] array, string text)
        {
            ArrayTasks arrayTasks = new ArrayTasks(array);
            TextTasks textTasks = new TextTasks(text);

            var tasks = new List<Task>
            {
                Task.Run(() => Console.WriteLine("Мінімальне значення: " + arrayTasks.Min())),
                Task.Run(() => Console.WriteLine("Максимальне значення: " + arrayTasks.Max())),
                Task.Run(() => Console.WriteLine("Сума: " + arrayTasks.Sum())),
                Task.Run(() => Console.WriteLine("Середнє: " + arrayTasks.Average())),
                Task.Run(() => Console.WriteLine("Частотний словник символів: " + string.Join(", ", textTasks.CharacterFrequency()))),
                Task.Run(() => Console.WriteLine("Частотний словник слів: " + string.Join(", ", textTasks.WordFrequency())))
            };

            Task.WhenAll(tasks).Wait();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введіть кількість потоків:");
            int numThreads = int.Parse(Console.ReadLine());

            Console.WriteLine("Введіть розмір масиву:");
            int arraySize = int.Parse(Console.ReadLine());

            Console.WriteLine("Введіть текст (для аналізу частоти слів):");
            string text = Console.ReadLine();

            ParallelProcessor processor = new ParallelProcessor(numThreads);
            int[] array = processor.GenerateRandomArray(arraySize);

            processor.ExecuteTasks(array, text);
        }
    }
}
