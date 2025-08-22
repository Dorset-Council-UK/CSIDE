namespace CSIDE.Data.Models.Shared
{
    public class ContactType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
