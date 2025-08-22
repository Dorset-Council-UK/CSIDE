namespace CSIDE.Data.Models.DMMO;

public class DMMOMediaType
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int NumFilesLimit { get; set; }
    public string? FileTypesLimit { get; set; }
}
