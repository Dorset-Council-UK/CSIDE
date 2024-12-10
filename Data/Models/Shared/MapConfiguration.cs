namespace CSIDE.Data.Models.Shared
{
    public class MapConfiguration
    {
        public required string MapId { get; init; }
        public required string OSMapsAPIKey { get; init; }
        public required string OSLicenceNumber { get; init; }
        public BasemapLayer Basemap { get; init; }
        public List<OverlayConfiguration>? Overlays { get; init; }

        private double[] _bounds = new double[4];
        public double[] Bounds
        {
            get { return _bounds; }
            set
            {
                if (value.Length != 4)
                {
                    throw new ArgumentException("Array must have exactly 4 numbers");
                }
                _bounds = value;
            }
        }

    }

    public class OverlayConfiguration
    {
        public required string MapServiceBaseURL { get; init; }
        public string? LayerName { get; init; }

    }

    public enum BasemapLayer
    {
        Road,
        Outdoor,
        Light
    }
}
