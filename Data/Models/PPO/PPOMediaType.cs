namespace CSIDE.Data.Models.PPO;

public class PPOMediaType
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int NumFilesLimit { get; set; }
    public string? FileTypesLimit { get; set; }
}
