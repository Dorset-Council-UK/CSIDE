using System.ComponentModel.DataAnnotations;

namespace CSIDE.Data.Models.Maintenance;

public class JobSubscriptionRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(500)]
    public required string EmailAddress { get; set; }
}
