using Microsoft.Graph.Beta.Models;

namespace CSIDE.Data.Models.Shared
{
    public class ContactPublicViewModel
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Organisation { get; set; }
        public string? ContactType { get; set; }
    }
}
