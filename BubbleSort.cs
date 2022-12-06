using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectParallelSort
{
    internal class BubbleSort
    {
        // The below are test results of bubble sort slgorithm under Debug mode.
        // ---------------------------------------------------------
        // # Before pre run Parallel.For when initialize algorithm #
        // When    500 names, speed up (no batch)  is 1.04x.
        // When  1,000 names, speed up (no batch)  is 0.95x.
        // When  1,500 names, speed up (no batch)  is 0.98x.
        // When  2,500 names, speed up (no batch)  is 1.39x.
        // When  5,000 names, speed up (no batch)  is 2.54x.
        // When  7,500 names, speed up (no batch)  is 3.25x.
        // When 10,000 names, speed up (no batch)  is 2.85x.
        // When    500 names, speed up (use batch) is 1.07x.
        // When  1,000 names, speed up (use batch) is 0.94x.
        // When  1,500 names, speed up (use batch) is 0.75x.
        // When  2,500 names, speed up (use batch) is 1.59x.
        // When  5,000 names, speed up (use batch) is 2.19x.
        // When  7,500 names, speed up (use batch) is 2.51x.
        // When 10,000 names, speed up (use batch) is 2.53x.
        // --------------------------------------------------------
        // # After pre run Parallel.For when initialize algorithm #
        // When    500 names, speed up (no batch)  is 1.17x.
        // When  1,000 names, speed up (no batch)  is 1.10x.
        // When  1,500 names, speed up (no batch)  is 0.88x.
        // When  2,500 names, speed up (no batch)  is 1.87x.
        // When  5,000 names, speed up (no batch)  is 3.38x.
        // When  7,500 names, speed up (no batch)  is 4.08x.
        // When 10,000 names, speed up (no batch)  is 4.45x.
        // When    500 names, speed up (use batch) is 1.23x.
        // When  1,000 names, speed up (use batch) is 1.06x.
        // When  1,500 names, speed up (use batch) is 0.81x.
        // When  2,500 names, speed up (use batch) is 1.95x.
        // When  5,000 names, speed up (use batch) is 2.93x.
        // When  7,500 names, speed up (use batch) is 3.14x.
        // When 10,000 names, speed up (use batch) is 3.24x.
        // --------------------------------------------------------
        // # After switch the .NET Parallel package to ThreadPool package for batch distribution #
        // When    500 names, speed up (use batch) is 0.96x.
        // When  1,000 names, speed up (use batch) is 0.95x.
        // When  1,500 names, speed up (use batch) is 0.82x.
        // When  2,500 names, speed up (use batch) is 2.62x.
        // When  5,000 names, speed up (use batch) is 4.08x.
        // When  7,500 names, speed up (use batch) is 4.21x.
        // When 10,000 names, speed up (use batch) is 4.20x.
        // --------------------------------------------------------
        // Parallel without using batches seems more faster.
        // Maybe .NET Parallel package already does some optimization about distributing threads to idle processors.
        // Or the .NET Parallel package may not distribute threads to processors fairly
        // like some batch threads may be line on the same processor.
        // Actually, if using batches, there are more basic operations inside a thread function,
        // so if the number of swap pairs becomes too many, using batches will be slower than to let Parallel package
        // take care of those many threads to run. Since Parallel package uses ThreadPool package, it should reuse exist
        // threads and not spend much time to create thread objects again.

        public BubbleSort()
        {

            // seems the .NET ThreadPool package works better than the .NET Parallel package on batch distribution
            ThreadPool.SetMinThreads(Environment.ProcessorCount, 0);
            ThreadPool.SetMaxThreads(Environment.ProcessorCount, 0);
            // seems first time call Parallel.For invokes threads initialization,
            // so just pre run this, and it actually affect the results which I tested
            //Parallel.For(0, Environment.ProcessorCount, i => { });
        }

        private bool swap(List<Name> names, int left, int right)
        {
            // swap when the left item is greater than the right item
            if (names[left].CompareTo(names[right]) > 0)
            {
                Name temp = names[left];
                names[left] = names[right];
                names[right] = temp;
                return true;
            }
            // no need to swap
            return false;
        }

        public void SequentialSort(List<Name> names)
        {
            // i represents the current index that the bubble should go to
            for (int i = 1; i < names.Count; i++)
            {
                // j and j+1 are the indexes of comparing items
                // names.Count-i is to limit the remained unsorted range of the name list
                for (int j = 0; j < names.Count - i; j++)
                {
                    // bubble the current item to the next position if needed.
                    swap(names, j, j + 1);
                }
            }
        }

        public void ParallelSort(List<Name> names, bool useBatch, bool useThreadPoolForBatches)
        {
            // no need to sort if name list is empty or has only one item
            if (names.Count < 2)
            {
                return;
            }

            int numOfThreads = Environment.ProcessorCount;

            // number of odd swap pairs is floored half of the number of list items
            int numOfOddSwapPairs = names.Count / 2;
            // if the list contains odd numbers of items,
            // number of even swap pairs is equal to number of odd swap pairs
            int numOfEvenSwapPairs = names.Count % 2 == 0 ? numOfOddSwapPairs - 1 : numOfOddSwapPairs;

            // use sequential sort instead because number of swap pairs is less than 78x number of threads,
            // which time for initializing threads may be enough to finish sorting already
            // Note: the 78x multiplier is just a number manually picked due to serveral manual runs of the program
            // From the test results, there is speed up after number of names is greater than 2500.
            // Number of swap pairs = 2500 / 2 = 1250, then divided by 16 as number of my laptop processors,
            // which is about 78.
            if (numOfOddSwapPairs < numOfThreads * 78)
            {
                SequentialSort(names);
                return;
            }

            bool hasSwap = true; // state represents if current odd/even step has swap occured

            if (useBatch) // with setting batches to limit threads to as same as the amount of processors
            {
                // there is a situation that one last thread may do nothing after redistribution of batches in order
                // to have the same size for every batch

                // when using batch, the speed up is 2.57x in my laptop
                // with 8 cores / 16 logical processors and 10,000 names on the list

                // prepare for distributing swap pairs to batches in order to limit the number of threads to create
                int numOfOddThreads = numOfThreads; // number of threads to use for odd step
                int sizeOfOddBatch = numOfOddSwapPairs / numOfThreads; // number of odd swap pairs for each batch
                int sizeOfOddExtraBatch = numOfOddSwapPairs % numOfThreads; // number of the extra odd swap pairs left
                int numOfEvenThreads = numOfThreads; // number of threads to use for even step
                int sizeOfEvenBatch = numOfEvenSwapPairs / numOfThreads; // number of even swap pairs for each batch
                int sizeOfEvenExtraBatch = numOfEvenSwapPairs % numOfThreads; // number of the extra even swap pairs left
                if (sizeOfOddExtraBatch > 0) // if there are extra odd swap pairs left
                {
                    // distribute more items to other threads except the last one thread remained to carry the rest
                    sizeOfOddBatch = numOfOddSwapPairs / (numOfThreads - 1);
                    sizeOfOddExtraBatch = numOfOddSwapPairs % (numOfThreads - 1);
                    // if there is no extra after redistributing, set number of odd-use threads to minus 1
                    if (sizeOfOddExtraBatch == 0)
                    {
                        numOfOddThreads -= 1;
                    }
                }
                if (sizeOfEvenExtraBatch > 0) // if there are extra even swap pairs left
                {
                    // distribute more items to other threads except the last one thread remained to carry the rest
                    sizeOfEvenBatch = numOfEvenSwapPairs / (numOfThreads - 1);
                    sizeOfEvenExtraBatch = numOfEvenSwapPairs % (numOfThreads - 1);
                    // if there is no extra after redistributing, set number of even-use threads to minus 1
                    if (sizeOfEvenExtraBatch == 0)
                    {
                        numOfEvenThreads -= 1;
                    }
                }

                while (hasSwap) // swapping until no swap needed for both odd and even steps
                {
                    // during the swapping, items that need to be bubbled
                    // will continuously move to the back of list step by step

                    // each swap here is independent because each swap walks over 2 items,
                    // so it is NOT like to swap from 0 and 1, 1 and 2, 2 and 3 ... in sequential sort,
                    // which each swap walks over 1 item and depends on the right item of previous swap.
                    // if to walk over 2 items, each swap depends on each other and they can be parallel.

                    hasSwap = false; // reset state

                    if (useThreadPoolForBatches)
                    {
                        // odd step
                        CountdownEvent countOddStep = new CountdownEvent(numOfOddThreads);
                        for (int n = 0; n < numOfOddThreads; n++)
                        {
                            ThreadPool.QueueUserWorkItem(threadNum =>
                            {
                                int i = (int)threadNum;
                                int startInclusive = i * sizeOfOddBatch;
                                int endExclusive = startInclusive + sizeOfOddBatch;
                                if (i == numOfOddThreads - 1 && sizeOfOddExtraBatch != 0)
                                {
                                    endExclusive = startInclusive + sizeOfOddExtraBatch;
                                }
                                for (int j = startInclusive; j < endExclusive; j++)
                                {
                                    if (swap(names, 2 * j, 2 * j + 1)) // swap 0 and 1, 2 and 3, 4 and 5 ... 
                                    {
                                        hasSwap = true;
                                    }
                                }
                                countOddStep.Signal();
                            }, n);
                        }
                        countOddStep.Wait();

                        // even step
                        CountdownEvent countEvenStep = new CountdownEvent(numOfEvenThreads);
                        for (int n = 0; n < numOfEvenThreads; n++)
                        {
                            ThreadPool.QueueUserWorkItem(threadNum =>
                            {
                                int i = (int)threadNum;
                                int startInclusive = i * sizeOfEvenBatch;
                                int endExclusive = startInclusive + sizeOfEvenBatch;
                                if (i == numOfEvenThreads - 1 && sizeOfEvenExtraBatch != 0)
                                {
                                    endExclusive = startInclusive + sizeOfEvenExtraBatch;
                                }
                                for (int j = startInclusive; j < endExclusive; j++)
                                {
                                    if (swap(names, 2 * j + 1, 2 * j + 2)) // swap 1 and 2, 3 and 4, 5 and 6 ... 
                                    {
                                        hasSwap = true;
                                    }
                                }
                                countEvenStep.Signal();
                            }, n);
                        }
                        countEvenStep.Wait();
                    }
                    else
                    {
                        // odd step
                        Parallel.For(0, numOfOddThreads, n =>
                        {
                            int startInclusive = n * sizeOfOddBatch;
                            int endExclusive = startInclusive + sizeOfOddBatch;
                            if (n == numOfOddThreads - 1 && sizeOfOddExtraBatch != 0)
                            {
                                endExclusive = startInclusive + sizeOfOddExtraBatch;
                            }
                            for (int j = startInclusive; j < endExclusive; j++)
                            {
                                if (swap(names, 2 * j, 2 * j + 1)) // swap 0 and 1, 2 and 3, 4 and 5 ... 
                                {
                                    hasSwap = true;
                                }
                            }
                        });

                        // even step
                        Parallel.For(0, numOfEvenThreads, n =>
                        {
                            int startInclusive = n * sizeOfEvenBatch;
                            int endExclusive = startInclusive + sizeOfEvenBatch;
                            if (n == numOfEvenThreads - 1 && sizeOfEvenExtraBatch != 0)
                            {
                                endExclusive = startInclusive + sizeOfEvenExtraBatch;
                            }
                            for (int j = startInclusive; j < endExclusive; j++)
                            {
                                if (swap(names, 2 * j + 1, 2 * j + 2)) // swap 1 and 2, 3 and 4, 5 and 6 ... 
                                {
                                    hasSwap = true;
                                }
                            }
                        });
                    }
                }
            }
            else // without setting batches to limit threads to as same as the amount of processors
            {
                // when not using batch, the speed up is 4.06x in my laptop
                // with 8 cores / 16 logical processors and 10,000 names on the list

                while (hasSwap) // swapping until no swap needed for both odd and even steps
                {
                    // during the swapping, items that need to be bubbled
                    // will continuously move to the back of list step by step

                    // each swap here is independent because each swap walks over 2 items,
                    // so it is NOT like to swap from 0 and 1, 1 and 2, 2 and 3 ... in sequential sort,
                    // which each swap walks over 1 item and depends on the right item of previous swap.
                    // if to walk over 2 items, each swap depends on each other and they can be parallel.

                    hasSwap = false; // reset state

                    // odd step
                    Parallel.For(0, numOfOddSwapPairs, i =>
                    {
                        if (swap(names, 2 * i, 2 * i + 1)) // swap 0 and 1, 2 and 3, 4 and 5 ... 
                        {
                            hasSwap = true;
                        }
                    });

                    // even step
                    Parallel.For(0, numOfEvenSwapPairs, i =>
                    {
                        if (swap(names, 2 * i + 1, 2 * i + 2)) // swap 1 and 2, 3 and 4, 5 and 6 ... 
                        {
                            hasSwap = true;
                        }
                    });
                }
            }

        }
    }
}
