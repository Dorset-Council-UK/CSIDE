using System.Globalization;

namespace CSIDE.Data.Models.Shared
{
    public class MediaPublicViewModel
    {
        public DateOnly? UploadDate { get; set; }
        public required string URL { get; set; }
        public string? Title { get; set; }
        public string? DocumentType { get; set; }
        public string? Format
        {
            get
            {
                var extension = new FileInfo(URL).Extension.ToLower(CultureInfo.InvariantCulture);
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
                            return MediaType.Image.ToString();
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
                            return MediaType.Video.ToString();
                        }

                    case ".pdf":
                        {
                            return MediaType.PDF_Document.ToString();
                        }

                    case ".doc":
                    case ".docx":
                        {
                            return MediaType.Word_Document.ToString();
                        }

                    case ".xls":
                    case ".xlsx":
                        {
                            return MediaType.Excel_Spreadsheet.ToString();
                        }

                    default:
                        {
                            return MediaType.Unknown.ToString();
                        }
                }
            }
        }
    }
}
