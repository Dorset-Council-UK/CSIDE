namespace CSIDE.Data.Models.Maintenance
{
    public class ProblemType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
