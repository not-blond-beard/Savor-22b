namespace Savor22b.Util;

public static class BlockUtil
{
    public static bool CalculateIsInProgress(long currentBlockIndex, long startedBlockIndex, long durationBlock)
    {
        return (currentBlockIndex - startedBlockIndex) < durationBlock;
    }
}
