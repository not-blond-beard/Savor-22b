namespace Savor22b.Tests.Util;

using Savor22b.Util;

public class BlockUtilTests
{
    [Theory]
    [InlineData(20, 4, 10)]
    [InlineData(1, 4, 10)]
    [InlineData(10, 5, 5)]
    [InlineData(10, 4, 10)]
    [InlineData(1001, 4, 1000)]
    public void CalculateIsInProgress_InProgress(long currentBlockIndex, long startedBlockIndex, long durationBlock)
    {
        var result = BlockUtil.CalculateIsInProgress(currentBlockIndex, startedBlockIndex, durationBlock);
        Assert.True(result);
    }

    [Theory]
    [InlineData(1, 4, 1)]
    [InlineData(11, 5, 5)]
    [InlineData(10, 4, 3)]
    [InlineData(401, 201, 200)]
    public void CalculateIsInProgress_NotInProgress(long currentBlockIndex, long startedBlockIndex, long durationBlock)
    {
        var result = BlockUtil.CalculateIsInProgress(currentBlockIndex, startedBlockIndex, durationBlock);
        Assert.False(result);
    }
}
