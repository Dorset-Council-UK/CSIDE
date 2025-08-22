namespace CSIDE.Data.Models.Shared
{
    public class ParishCode
    {
        public int ParishId { get; set; }
        public required string Code { get; set; }
        public virtual Parish? Parish { get; set; }
    }
}
