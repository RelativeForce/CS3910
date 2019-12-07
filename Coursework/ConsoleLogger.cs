using System;

namespace Coursework
{
    public sealed class ConsoleLogger
    {
        private readonly bool _showNewBestForRunInProgress;

        public ConsoleLogger(bool showNewBestForRunInProgress)
        {
            _showNewBestForRunInProgress = showNewBestForRunInProgress;
        }

        public void LogRunResult(int runIndex, double cost, double[] weights)
        {
            Console.WriteLine($"Run: {runIndex} Value: {cost} Weights: {weights.Join()}");
        }

        public void LogNewBestForRunInProgress(int iterationIndex, double cost, double[] weights)
        {
            if (!_showNewBestForRunInProgress)
                return;

            Console.WriteLine($"New Best Found at Iteration: {iterationIndex} Value: {cost}]");
        }
    }
}
