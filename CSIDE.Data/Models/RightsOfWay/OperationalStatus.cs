namespace CSIDE.Data.Models.RightsOfWay
{
    public class OperationalStatus
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool IsClosed { get; set; }
    }
}
