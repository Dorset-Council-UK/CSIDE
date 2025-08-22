using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.Models.Shared
{
    public class Contact
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PrimaryContactNo {  get; set; }
        public string? SecondaryContactNo { get; set; }
        public string? OrganisationName { get; set; }
        public int? ContactTypeId { get; set; }
        public ContactType? ContactType { get; set; }

        public ICollection<JobContact>? JobContact { get; set; }
        public ICollection<DMMOContact>? DMMOContact { get; set; }
        public ICollection<LandownerDepositContact>? LandownerDepositContact { get; set; }
    }
}
