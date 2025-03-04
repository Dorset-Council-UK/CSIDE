namespace CSIDE.Data.Models.LandownerDeposits;

public class LandownerDepositMediaType
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int NumFilesLimit { get; set; }
    public string? FileTypesLimit { get; set; }
}
