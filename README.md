# Parallel Alphabetical Sort of Names

## Table of Contents
1. [Introduction](#introduction)
2. [Problem Statement](#problem-statement)
3. [Solution](#solution)
4. [Discussion and Limitation](#discussion-and-limitation)
5. [Conclusion](#conclusion)
6. [Instructions to Execute the Code](#instructions-to-execute-the-code)
7. [References](#references)

## Introduction
In this project, we implemented and tested Quick Sort, Merge Sort, and Bubble Sort algorithms, comparing them with the default .NET LINQ sorting method. This report details our findings and the problems encountered, including comparisons of the sorting methods and different implementations of Bubble Sort in sequential and parallel methods.

## Problem Statement
The primary objective of this project is to sort names alphabetically using different sorting algorithms and compare their performance. The sorting algorithms implemented are:
- Quick Sort
- Merge Sort
- Bubble Sort
- .NET LINQ Sort (used as a benchmark)

We implemented both sequential and parallel versions of the sorting algorithms to evaluate the performance gains achieved through parallelism.

## Solution
### Quick Sort vs. Merge Sort vs. .NET LINQ Sort Algorithms
The results were calculated on a computer with 6 cores and 6 threads.

- **.NET LINQ sorting method:** 13 milliseconds
- **Quick Sort algorithm:** 17 milliseconds
- **Quick Sort with Parallel Invoke:** 7 milliseconds
- **Merge Sort algorithm:** 20 milliseconds
- **Merge Sort with Parallel Invoke:** 10 milliseconds

The basic Quick Sort and Merge Sort algorithms were slower than the .NET LINQ sorting method. However, using the Parallel Invoke method greatly improved the calculation speed, surpassing the .NET LINQ sorting method.

### Bubble Sort Algorithm
We implemented both sequential and parallel versions of the Bubble Sort algorithm. The parallel version included options to enable batching, limiting the number of threads to the number of processors, and using the ThreadPool package instead of the Parallel package.

#### Debug Mode Results
- **Without Batch:**
  - 500 names: 1.04x speed up
  - 1000 names: 0.95x speed up
  - 1500 names: 0.98x speed up
  - 2500 names: 1.39x speed up
  - 5000 names: 2.54x speed up
  - 7500 names: 3.25x speed up
  - 10000 names: 2.85x speed up

- **With Batch:**
  - 500 names: 1.07x speed up
  - 1000 names: 0.94x speed up
  - 1500 names: 0.75x speed up
  - 2500 names: 1.59x speed up
  - 5000 names: 2.19x speed up
  - 7500 names: 2.51x speed up
  - 10000 names: 2.53x speed up

#### Release Mode Results
- **Without Batch:**
  - 500 names: 1.17x speed up
  - 1000 names: 1.10x speed up
  - 1500 names: 0.88x speed up
  - 2500 names: 1.87x speed up
  - 5000 names: 3.38x speed up
  - 7500 names: 4.08x speed up
  - 10000 names: 4.45x speed up

- **With Batch:**
  - 500 names: 1.23x speed up
  - 1000 names: 1.06x speed up
  - 1500 names: 0.81x speed up
  - 2500 names: 1.95x speed up
  - 5000 names: 2.93x speed up
  - 7500 names: 3.14x speed up
  - 10000 names: 3.24x speed up

Switching from the .NET Parallel package to the ThreadPool package for batch distribution showed different speed-ups, with the ThreadPool package often providing better performance due to more efficient thread distribution.

## Discussion and Limitation
The use of parallelism significantly improved the performance of Quick Sort and Merge Sort algorithms, with the parallel versions surpassing the default .NET LINQ sorting method. However, implementing parallelism with Bubble Sort showed mixed results, depending on the batch usage and the number of names being sorted. The results suggest that parallelism is highly beneficial for divide-and-conquer algorithms like Quick Sort and Merge Sort but may require more careful tuning for algorithms like Bubble Sort.

## Conclusion
The Parallel Invoke method in C# greatly enhances the performance of sorting algorithms that utilize divide-and-conquer logic. The improvements in speed even surpassed the default .NET LINQ sorting method. While the ThreadPool package showed promise in efficiently distributing threads, further tuning and experimentation are necessary for achieving optimal performance with parallel Bubble Sort.

## Instructions to Execute the Code
1. Open the `ProjectParallelSort` project in Visual Studio 2022 with .NET 6.0 installed.
2. Change the configuration of the project to release mode.
3. Build the solution if necessary.
4. Place the `names.txt` file under the path `ProjectParallelSort\bin\Release\net6.0`.
5. Run the project, and the results will be displayed on the terminal screen.

## References
- [Parallel Invoke Method in C# With Examples](https://www.tutorialsteacher.com/articles/parallel-invoke-method-in-csharp)

