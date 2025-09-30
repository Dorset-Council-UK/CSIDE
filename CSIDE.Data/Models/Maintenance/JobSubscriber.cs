namespace CSIDE.Data.Models.Maintenance;

public class JobSubscriber
{
    public int JobId { get; set; }
    public required string EmailAddress { get; set; }
    public Guid UnsubscribeToken { get; set; }
}
