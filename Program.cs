using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ProjectParallelSort
{
    class Program
    {
        public static List<Name>? ImportFile(string fileName)
        {
            List<Name> Names = new();
            int namesCount = 0;
            try
            {
                // populate the list of names from a file
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while (sr.Peek() >= 0)
                    {
                        string[] s = sr.ReadLine().Split(' ');
                        Names.Add(new Name(s[0], s[1]));
                        namesCount++;
                    }
                }
                Console.WriteLine(" > Loaded names.txt successfully, and {0} names had been read.", namesCount);
                return Names;
            }
            catch (Exception e)
            {
                Console.WriteLine(" ERROR: failed to load names.txt, {0}", e.Message);
                return null;
            }
            

        }

        public static void ExportFile(List<Name> data, string fileName)
        {
            try
            {
                // write the names to a txt file
                using (TextWriter tw = new StreamWriter(fileName))
                {
                    foreach (Name s in data)
                        tw.WriteLine(s.firstName + " " + s.lastName);
                }
                Console.WriteLine(" > Saved {0} successfully.", fileName);
            } 
            catch (Exception e) 
            {
                Console.WriteLine(" ERROR: failed to save {0}, {1}", fileName, e.Message);
            }
            
        }

        private static void exit()
        {
            Console.WriteLine("\nPress enter to exit...");
            Console.ReadLine();
        }

        static void Main()
        {
            // Load the data
            List<Name> names = ImportFile("names.txt");
            // Copy the data for usage
            List<Name> quickSortNames = names.ToList();                         // for quick sort
            List<Name> quickSortParallelNames = names.ToList();                 // for quick sort using parallel invoke
            List<Name> quickSortThreadPoolNames = names.ToList();               // for quick sort using ThreadPool
            List<Name> mergeSortNames = names.ToList();                         // for merge sort
            List<Name> mergeSortParallelNames = names.ToList();                 // for merge sort using parallel invoke
            List<Name> mergeSortThreadPoolNames = names.ToList();               // for merge sort using ThreadPool
            List<Name> namesForSequentialSort = names.ToList();                 // for sequential sort
            List<Name> namesForParallelSort = names.ToList();                   // for parallel sort without using batches
            List<Name> namesForParallelSortBatched = names.ToList();            // for parallel sort with using batches and Parallel package
            List<Name> namesForParallelSortBatchedThreadPool = names.ToList();  // for parallel sort with using batches and ThreadPool package

            // Set up new instances
            MergeSort mergeSort = new();
            QuickSort quickSort = new();
            BubbleSort bubble = new();


            // Get how many processors current computer has for the purpose of running the program in parallel
            int processors = Environment.ProcessorCount;
            Console.WriteLine("\n The number of processors on this computer is {0}.", processors);
            Console.WriteLine(" Set the MinDepth of the Parallel Process to {0}.", processors);
            mergeSort.minDepth = processors;
            quickSort.minDepth = processors;

            // time the sort.
            Stopwatch stopwatch = new();

            // Apply the .NET LINQ sorting algorithm
            Console.WriteLine("\n Sorting by .NET Linq Algorithm");
            stopwatch.Restart();
            List<Name> sortedNames = names.OrderBy(s => s.lastName).ThenBy(s => s.firstName).ToList();
            stopwatch.Stop();
            Console.WriteLine(" > Code took {0} milliseconds to execute.", stopwatch.ElapsedMilliseconds);



            /** Quick Sort Algorithm **/
            // Apply the Quick Sort basic algorithm
            Console.WriteLine("\n Sorting by Quicksort Algorithm");
            stopwatch.Restart();
            quickSort.Quick_Sort(quickSortNames, 0, quickSortNames.Count() - 1);
            stopwatch.Stop();
            Console.WriteLine(" > Code took {0} milliseconds to execute.", stopwatch.ElapsedMilliseconds);

            // Apply the Quick Sort algorithm using Parallel
            Console.WriteLine("\n Sorting by Quicksort Algorithm using Parallel Invoke");
            stopwatch.Restart();
            quickSort.Quick_Sort_Parallel(quickSortParallelNames, 0, quickSortParallelNames.Count() - 1, 0);
            stopwatch.Stop();
            Console.WriteLine(" > Code took {0} milliseconds to execute.", stopwatch.ElapsedMilliseconds);

            //// Apply the Quick Sort algorithm using ThreadPool
            //Console.WriteLine("\n Sorting by Quicksort Algorithm using ThreadPool");
            //stopwatch.Restart();
            //quickSort.Quick_Sort_ThreadPool(quickSortThreadPoolNames, 0, quickSortThreadPoolNames.Count() - 1);
            //stopwatch.Stop();
            //ExportFile(quickSortThreadPoolNames, "QuickSort ThreadPool.txt");
            //Console.WriteLine(" > Code took {0} milliseconds to execute.", stopwatch.ElapsedMilliseconds);



            /** Merge Sort Algorithm **/
            // Apply the Merge Sort Basic algorithm 
            Console.WriteLine("\n Sorting by Mergesort Algorithm");
            stopwatch.Restart();
            mergeSort.Merge_Sort(mergeSortNames, 0, mergeSortNames.Count() - 1);
            stopwatch.Stop();
            Console.WriteLine(" > Code took {0} milliseconds to execute.", stopwatch.ElapsedMilliseconds);

            // Apply the Merge Sort algorithm using Parallel 
            Console.WriteLine("\n Sorting by Mergesort Algorithm using Parallel Invoke");
            stopwatch.Restart();
            mergeSort.Merge_Sort_Parallel(mergeSortParallelNames, 0, mergeSortParallelNames.Count() - 1, 0);
            stopwatch.Stop();
            Console.WriteLine(" > Code took {0} milliseconds to execute.", stopwatch.ElapsedMilliseconds);

            //// Apply the Merge Sort algorithm using ThreadPool
            //Console.WriteLine("\n Sorting by Mergesort Algorithm using ThreadPool");
            //stopwatch.Restart();
            //mergeSort.Merge_Sort_ThreadPool(mergeSortThreadPoolNames, 0, mergeSortThreadPoolNames.Count() - 1);
            //stopwatch.Stop();
            //ExportFile(mergeSortThreadPoolNames, "Mergesort ThreadPool.txt");
            //Console.WriteLine(" > Code took {0} milliseconds to execute.", stopwatch.ElapsedMilliseconds);



            /** Bubble Sort Algorithm **/
            // Start sorting and timing for sequential bubble sort
            Console.WriteLine("\n Start sequential bubble sorting...");
            stopwatch.Restart();
            bubble.SequentialSort(namesForSequentialSort);
            stopwatch.Stop();
            long sequentialTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(" > Sequential bubble sort took {0} milliseconds to execute.", sequentialTime);

            // Start sorting and timing for parallel bubble sort without using batches
            Console.WriteLine("\n Start parallel bubble sorting (batch: N)...");
            stopwatch.Restart();
            bubble.ParallelSort(namesForParallelSort, false, false);
            stopwatch.Stop();
            long parallelTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(" > Parallel bubble sort (batch: N) took {0} milliseconds to execute.", parallelTime);

            // Calculate parallel (without using batches) speed up
            if (parallelTime != 0)
            {
                double speedup = (double)sequentialTime / parallelTime;
                Console.WriteLine(" > Parallel bubble sort (batch: N) is {0:N2}x faster than Sequential bubble sort.", speedup);
            }

            // Start sorting and timing for parallel bubble sort with using batches and Parallel package
            Console.WriteLine("\n Start parallel bubble sorting (batch: Y, package: Parallel)...");
            stopwatch.Restart();
            bubble.ParallelSort(namesForParallelSortBatched, true, false);
            stopwatch.Stop();
            long parallelBatchTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(" > Parallel bubble sort (batch: Y, package: Parallel) took {0} milliseconds to execute.", parallelBatchTime);

            // Calculate parallel (with using batches and Parallel package) speed up
            if (parallelBatchTime != 0)
            {
                double speedup = (double)sequentialTime / parallelBatchTime;
                Console.WriteLine(" > Parallel bubble sort (batch: Y, package: Parallel) is {0:N2}x faster than Sequential bubble sort.", speedup);
            }

            // Start sorting and timing for parallel bubble sort with using batches and ThreadPool package
            Console.WriteLine("\n Start parallel bubble sorting (batch: Y, package: ThreadPool)...");
            stopwatch.Restart();
            bubble.ParallelSort(namesForParallelSortBatchedThreadPool, true, true);
            stopwatch.Stop();
            long parallelBatchThreadPoolTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(" > Parallel bubble sort (batch: Y, package: ThreadPool) took {0} milliseconds to execute.", parallelBatchThreadPoolTime);

            // Calculate parallel (with using batches) speed up
            if (parallelBatchThreadPoolTime != 0)
            {
                double speedup = (double)sequentialTime / parallelBatchThreadPoolTime;
                Console.WriteLine(" > Parallel bubble sort (batch: Y, package: ThreadPool) is {0:N2}x faster than Sequential bubble sort.", speedup);
            }

            // Validate if the results of sequential and parallel sort are the same
            bool pass = true;
            for (int i = 0; i < names.Count(); i++)
            {
                if (sortedNames[i].CompareTo(namesForSequentialSort[i]) != 0)
                {
                    pass = false;
                    Console.WriteLine("\n ERROR: result of sequential bubble sort is wrong");
                    break;
                }
                if (sortedNames[i].CompareTo(namesForParallelSort[i]) != 0)
                {
                    pass = false;
                    Console.WriteLine("\n ERROR: result of parallel bubble sort (batch: N) is wrong");
                    break;
                }
                if (sortedNames[i].CompareTo(namesForParallelSortBatched[i]) != 0)
                {
                    pass = false;
                    Console.WriteLine("\n ERROR: result of parallel bubble sort (batch: Y, package: Parallel) is wrong");
                    break;
                }
                if (sortedNames[i].CompareTo(namesForParallelSortBatchedThreadPool[i]) != 0)
                {
                    pass = false;
                    Console.WriteLine("\n ERROR: result of parallel bubble sort (batch: Y, package: ThreadPool) is wrong");
                    break;
                }
            }
            if (!pass)
            {
                exit();
                return;
            }

            // Export data as a txt file
            //ExportFile(sortedNames, "Linqsort Names.txt");
            //ExportFile(mergeSortNames, "MergeSort Names.txt");
            //ExportFile(quickSortNames, "QuickSort Names.txt");

            exit();
        }
    }
}