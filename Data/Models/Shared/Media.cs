using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.RightsOfWay;
using NodaTime;
using System.Globalization;

namespace CSIDE.Data.Models.Shared
{
    public class Media
    {
        public int Id { get; set; }
        public Instant? UploadDate { get; set; }
        public required string URL { get; set; }
        public string? Title { get; set; }

        
        public ICollection<JobMedia>? JobMedia { get; set; }
        public ICollection<DMMOMedia>? DMMOMedia { get; set; }
        public ICollection<PPOMedia>? PPOMedia { get; set; }
        public ICollection<LandownerDepositMedia>? LandownerDepositMedia { get; set; }
        public ICollection<InfrastructureMedia>? InfrastructureMedia { get; set; }
        public ICollection<RouteMedia>? RouteMedia { get; set; }

        public MediaType Format
        {
            get
            {
                var extension = new System.IO.FileInfo(URL).Extension.ToLower(CultureInfo.InvariantCulture);
                switch (extension ?? "") // URL.Substring(URL.Length - 4).ToLower
                {
                    case ".gif":
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".bmp":
                    case ".tif":
                    case ".tiff":
                    case ".webp":
                        {
                            return MediaType.Image;
                        }

                    case ".wmv":
                    case ".mov":
                    case ".mp4":
                    case ".mpg":
                    case ".mpeg":
                    case ".qt":
                    case ".3gp":
                    case ".webm":
                    case ".ogg":
                        {
                            return MediaType.Video;
                        }

                    case ".pdf":
                        {
                            return MediaType.PDF_Document;
                        }

                    case ".doc":
                    case ".docx":
                        {
                            return MediaType.Word_Document;
                        }

                    case ".xls":
                    case ".xlsx":
                        {
                            return MediaType.Excel_Spreadsheet;
                        }

                    default:
                        {
                            return MediaType.Unknown;
                        }
                }
            }
        }
    }
}
