using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectParallelSort
{
    internal class QuickSort
    {
        // Initialize the minDepth
        public int minDepth;

        public QuickSort()
        {
            // Initialize the Parallel loops and regions
            Parallel.For(0, Environment.ProcessorCount, i => { });
            // Initialize the ThreadPool
            //ThreadPool.SetMinThreads(Environment.ProcessorCount, 0);
            //ThreadPool.SetMaxThreads(Environment.ProcessorCount, 0);
        }

        // QuickSort main function.
        // Sort the data by pivot, then divide the data into two halves by pivot point.
        // Then keep calling itself to find the pivot in the devided data.
        public void Quick_Sort(List<Name> data, int left, int right)
        {
            int middle;

            if (left < right)
            {
                middle = Partition(data, left, right);
                Quick_Sort(data, left, middle - 1);
                Quick_Sort(data, middle + 1, right);
            }
        }

        // QuickSort Using Parallel Invoke
        public void Quick_Sort_Parallel(List<Name> data, int left, int right, int depth)
        {
            int middle;

            if (left < right)
            {
                middle = Partition(data, left, right);
                if (depth > minDepth)
                {
                    Quick_Sort_Parallel(data, left, middle - 1, depth + 1);
                    Quick_Sort_Parallel(data, middle + 1, right, depth + 1);
                }
                else
                {
                    Parallel.Invoke(
                        () => Quick_Sort_Parallel(data, left, middle - 1, depth + 1),
                        () => Quick_Sort_Parallel(data, middle + 1, right, depth + 1)
                        );
                }
            }
        }

        // QuickSort Using ThreadPool.
        // Not working properlynow, since we do not know when the threadings complete the job.
        public void Quick_Sort_ThreadPool(List<Name> data, int left, int right)
        {
            int middle;

            if (left < right)
            {
                middle = Partition(data, left, right);
                ThreadPool.QueueUserWorkItem(_ => Quick_Sort_ThreadPool(data, left, middle - 1));
                ThreadPool.QueueUserWorkItem(_ => Quick_Sort_ThreadPool(data, middle + 1, right));

            }
        }

        // Sort the data by using the rightmost data as the pivot, then return the pivot index.
        public int Partition(List<Name> data, int left, int right)
        {
            // Get the rightmost name
            Name endingName = data[right];
            int i = left;

            // Sort the data by comparing each name to the rightmost (ending) name.
            for (int j = left; j < right; j++)
            {
                // Current Lastname < Rightmost Lastname. 
                if (String.Compare(data[j].lastName, endingName.lastName) == -1)
                {
                    // Swap the names
                    (data[j], data[i]) = (data[i], data[j]);
                    i++;
                }
                // Current Lastname == Rightmost Lastname 
                else if (String.Compare(data[j].lastName, endingName.lastName) == 0)
                {
                    // Current Firstname <= Rightmost Firstname. 
                    if (String.Compare(data[j].firstName, endingName.firstName) == -1 ||
                        String.Compare(data[j].firstName, endingName.firstName) == 0)
                    {
                        // Swap the names
                        (data[j], data[i]) = (data[i], data[j]);
                        i++;
                    }
                }
            }
            // Swap the pivot to the partition location
            (data[right], data[i]) = (data[i], data[right]);
            return i;
        }
    }
}
