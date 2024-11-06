using NodaTime;

namespace CSIDE.Data.Models.Shared
{
    public class Media
    {
        public int Id { get; set; }
        public Instant? UploadDate { get; set; }
        public required string URL { get; set; }
        public string? Title { get; set; }

        public MediaType Format
        {
            get
            {
                var extension = new System.IO.FileInfo(URL).Extension.ToLower();
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
