namespace CSIDE.Data.Models.Surveys
{
    public class Condition
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int SortOrder { get; set; }
    }
}
