using System;
using System.Collections.Generic;
using System.Linq;
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
            int min = int.MaxValue;
            int partitionSize = _array.Length / Environment.ProcessorCount;
            int[] localMins = new int[Environment.ProcessorCount];

            Parallel.For(0, Environment.ProcessorCount, i =>
            {
                int start = i * partitionSize;
                int end = (i == Environment.ProcessorCount - 1) ? _array.Length : (i + 1) * partitionSize;

                int localMin = _array[start];

                for (int j = start + 1; j < end; j++)
                {
                    if (_array[j] < localMin)
                        localMin = _array[j];
                }

                localMins[i] = localMin;
            });

            min = localMins.Min();
            return min;
        }

      
        public int Max()
        {
            int max = int.MinValue;
            int partitionSize = _array.Length / Environment.ProcessorCount;
            int[] localMaxs = new int[Environment.ProcessorCount];

            Parallel.For(0, Environment.ProcessorCount, i =>
            {
                int start = i * partitionSize;
                int end = (i == Environment.ProcessorCount - 1) ? _array.Length : (i + 1) * partitionSize;

                int localMax = _array[start];

                for (int j = start + 1; j < end; j++)
                {
                    if (_array[j] > localMax)
                        localMax = _array[j];
                }

                localMaxs[i] = localMax;
            });

            max = localMaxs.Max();
            return max;
        }

       
        public int Sum()
        {
            int sum = 0;
            int partitionSize = _array.Length / Environment.ProcessorCount;
            int[] localSums = new int[Environment.ProcessorCount];

            Parallel.For(0, Environment.ProcessorCount, i =>
            {
                int start = i * partitionSize;
                int end = (i == Environment.ProcessorCount - 1) ? _array.Length : (i + 1) * partitionSize;

                int localSum = 0;

                for (int j = start; j < end; j++)
                {
                    localSum += _array[j];
                }

                localSums[i] = localSum;
            });

            sum = localSums.Sum();
            return sum;
        }

       
        public double Average()
        {
            int sum = 0;
            int partitionSize = _array.Length / Environment.ProcessorCount;
            int[] localSums = new int[Environment.ProcessorCount];

            Parallel.For(0, Environment.ProcessorCount, i =>
            {
                int start = i * partitionSize;
                int end = (i == Environment.ProcessorCount - 1) ? _array.Length : (i + 1) * partitionSize;

                int localSum = 0;

                for (int j = start; j < end; j++)
                {
                    localSum += _array[j];
                }

                localSums[i] = localSum;
            });

            sum = localSums.Sum();
            return (double)sum / _array.Length;
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
            } );
            return array;
        }

        public void ExecuteTasks(int[] array, string text)
        {
            ArrayTasks arrayTasks = new ArrayTasks(array);
            TextTasks textTasks = new TextTasks(text);

            var tasks = new List<Task>
            {
                Task.Run(() => Console.WriteLine("Мінімальне значення: " + arrayTasks.Min())),
                Task.Run(() => Console.WriteLine(" Максимальне значення: " + arrayTasks.Max())),
                Task.Run(() => Console.WriteLine("Сума: " + arrayTasks.Sum())),
                Task.Run(() => Console.WriteLine("Середнє: " + arrayTasks.Average())),
                Task.Run(() => Console.WriteLine(" Частотний словник символів: " + string.Join(", ", textTasks.CharacterFrequency()))),
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

            Console.WriteLine("Введіть розмір ммасиву:");
            int arraySize = int.Parse(Console.ReadLine());
  
            Console.WriteLine("Введіть текст (для аналізу частоти слів):");
            string text = Console.ReadLine();
     
            ParallelProcessor processor = new ParallelProcessor(numThreads);
            int[] array = processor.GenerateRandomArray(arraySize);

            processor.ExecuteTasks(array, text);
        }
    }
}
