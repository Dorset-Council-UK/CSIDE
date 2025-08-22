namespace CSIDE.Shared.Options;

public record AzureBlobStorageOptions
{
    public const string SectionName = "AzureBlobStorage";

    public string ConnectionString { get; init; } = "";
    public string ContainerName { get; init; } = "";
}
