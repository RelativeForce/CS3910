namespace Coursework
{
    public interface IConsoleLogger
    {
        void LogRunResult(int runIndex, double cost, double[] weights);

        void LogNewBestForRunInProgress(int iterationIndex, double cost, double[] weights);
    }
}