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
    }
}
