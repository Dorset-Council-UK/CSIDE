using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Tests.Data;

namespace CSIDE.Tests.Extensions;

public class LandownerDepositExtensionsTests
{
    [Theory]
    [InlineData(nameof(LandownerDepositTestData.LandownerDeposit_1), 0)]
    [InlineData(nameof(LandownerDepositTestData.LandownerDeposit_2), 1)]
    [InlineData(nameof(LandownerDepositTestData.LandownerDeposit_3), 2)]
    [InlineData(nameof(LandownerDepositTestData.LandownerDeposit_4), 2)]
    public void ToPublicViewModel_ReturnsCorrectContactCount(string depositName, int expectedContactCount)
    {
        // Arrange
        var deposit = depositName switch
        {
            nameof(LandownerDepositTestData.LandownerDeposit_1) => LandownerDepositTestData.LandownerDeposit_1,
            nameof(LandownerDepositTestData.LandownerDeposit_2) => LandownerDepositTestData.LandownerDeposit_2,
            nameof(LandownerDepositTestData.LandownerDeposit_3) => LandownerDepositTestData.LandownerDeposit_3,
            nameof(LandownerDepositTestData.LandownerDeposit_4) => LandownerDepositTestData.LandownerDeposit_4,
            _ => throw new ArgumentOutOfRangeException(nameof(depositName)),
        };

        // Act
        var viewModel = deposit.ToPublicViewModel("LD");

        // Assert
        Assert.Equal(expectedContactCount, viewModel.Contacts.Count);
    }
}
