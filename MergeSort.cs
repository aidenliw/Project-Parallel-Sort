using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectParallelSort
{
    internal class MergeSort
    {
        // Initialize the minDepth
        public int minDepth;

        public MergeSort()
        {
            // Initialize the Parallel loops and regions
            Parallel.For(0, Environment.ProcessorCount, i => { });
            // Initialize the ThreadPool
            //ThreadPool.SetMinThreads(Environment.ProcessorCount, 0);
            //ThreadPool.SetMaxThreads(Environment.ProcessorCount, 0);
        }

        // MergeSort main function. Divide the data into two halves by calling itself, then merge them.
        public void Merge_Sort(List<Name> data, int left, int right)
        {
            int middle;

            if (left < right)
            {
                middle = (right + left) / 2;

                Merge_Sort(data, left, middle);
                Merge_Sort(data, middle + 1, right);
                Merge(data, left, middle, right);
            }
        }

        // MergeSort Using Parallel Invoke
        public void Merge_Sort_Parallel(List<Name> data, int left, int right, int depth)
        {
            int middle;

            if (left < right)
            {
                middle = (right + left) / 2;

                if (depth > minDepth)
                {
                    Merge_Sort_Parallel(data, left, middle, depth + 1);
                    Merge_Sort_Parallel(data, middle + 1, right, depth + 1);
                }
                else
                {
                    Parallel.Invoke(
                        () => Merge_Sort_Parallel(data, left, middle, depth + 1),
                        () => Merge_Sort_Parallel(data, middle + 1, right, depth + 1)
                        );
                }
                Merge(data, left, middle, right);
            }
        }

        // MergeSort Using ThreadPool.
        // Not working properly now, since we do not know when the threadings complete the job.
        public void Merge_Sort_ThreadPool(List<Name> data, int left, int right)
        {
            int middle;

            if (left < right)
            {
                middle = (right + left) / 2;

                ThreadPool.QueueUserWorkItem(_ => Merge_Sort_ThreadPool(data, left, middle));
                ThreadPool.QueueUserWorkItem(_ => Merge_Sort_ThreadPool(data, middle + 1, right));
                Merge(data, left, middle, right);
            }
        }

        // Sort and merge the separated data 
        public void Merge(List<Name> data, int left, int mid, int right)
        {
            // Calculate left/right array lenth n1 and n2 
            int n1 = mid - left + 1;
            int n2 = right - mid;
            int i, j;

            // Copy sublists from the data to the new name list
            List<Name> left_list = new();
            List<Name> right_list = new();
            for (i = 0; i < n1; i++)
            {
                left_list.Add(data[left + i]);
            }
            for (j = 0; j < n2; j++)
            {
                right_list.Add(data[mid + 1 + j]);
            }
            // Copy sublists from the data to the new name list (another approach)
            //List<Name> left_list = data.Skip(left).Take(n1).ToList();
            //List<Name> right_list = data.Skip(mid + 1).Take(n2).ToList();

            // Add Maximum Value of Ending Name
            left_list.Add(new Name("ZZZZZZZZZZZZZZZ", "ZZZZZZZZZZZZZZZ"));
            right_list.Add(new Name("ZZZZZZZZZZZZZZZ", "ZZZZZZZZZZZZZZZ"));

            i = 0;
            j = 0;

            // Sort the list
            for (int k = left; k <= right; k++)
            {
                // Left Lastname < Right Lastname 
                if (String.Compare(left_list[i].lastName, right_list[j].lastName) == -1)
                {
                    data[k] = left_list[i];
                    i++;
                }
                // Left Lastname == Right Lastname 
                else if (String.Compare(left_list[i].lastName, right_list[j].lastName) == 0)
                {
                    // Left Firstname <= Right Firstname. Chose Left
                    if (String.Compare(left_list[i].firstName, right_list[j].firstName) == -1 ||
                        String.Compare(left_list[i].firstName, right_list[j].firstName) == 0)
                    {
                        data[k] = left_list[i];
                        i++;
                    }
                    // Left Firstname > Right Firstname. Chose Right
                    else
                    {
                        data[k] = right_list[j];
                        j++;
                    }
                }
                // Other cases, Chose Right
                else
                {
                    data[k] = right_list[j];
                    j++;
                }
            }
        }
    }
}
