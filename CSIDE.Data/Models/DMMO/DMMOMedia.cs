using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.DMMO;

public class DMMOMedia
{
    public int DMMOId { get; set; }
    public int MediaId { get; set; }
    public int MediaTypeId { get; set; }
    public Application DMMOApplication { get; set; } = null!;
    public Media Media { get; set; } = null!;
    public DMMOMediaType MediaType { get; set; } = null!;
}
