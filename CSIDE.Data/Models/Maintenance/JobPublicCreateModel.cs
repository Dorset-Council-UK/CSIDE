using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDE.Data.Models.Maintenance
{
    public class JobPublicCreateModel
    {
        public required string ProblemDescription { get; set; }
        public required double Easting { get; set; }
        public required double Northing { get; set; }
        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPrimaryNo { get; set; }
        public string? ContactSecondaryNo { get; set; }
        public string? ContactOrganisationName { get; set; }
        public bool ReceiveUpdates { get; set; }
        public string? LandownerName { get; set; }
        public string? LandownerEmail { get; set; }
        public string? LandownerPrimaryNo { get; set; }
        public string? LandownerSecondaryNo { get; set; }
        public string? LandownerOrganisationName { get; set; }


    }
}
