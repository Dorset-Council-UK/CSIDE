namespace CSIDE.Data.Models.Shared
{
    public class FileUploadRequest
    {
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public required Stream FileStream { get; set; }
        public long FileSize { get; set; }
    }
}