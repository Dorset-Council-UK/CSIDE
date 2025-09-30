namespace CSIDE.Data.Models.Shared
{
    public static class MediaConstants
    {
        public static readonly string[] AllowedFileTypes = [
            "image/jpeg",
            "image/jpg",
            "image/png",
            "image/gif",
            "video/mp4",
            "video/webm",
            "video/ogg",
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/msword",
            "application/vnd.oasis.opendocument.text"
        ];

        public static readonly string[] FriendlyFileTypes = [
            "JPG", "PNG", "GIF", "MP4", "WEBM", "OGG", "PDF", "DOCX", "DOC", "ODT"
        ];
    }
}
