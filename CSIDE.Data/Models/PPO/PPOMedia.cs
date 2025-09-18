using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.PPO;

public class PPOMedia
{
    public int PPOId { get; set; }
    public int MediaId { get; set; }
    public int MediaTypeId { get; set; }
    public PPOApplication PPOApplication { get; set; } = null!;
    public Media Media { get; set; } = null!;
    public PPOMediaType MediaType { get; set; } = null!;
}
