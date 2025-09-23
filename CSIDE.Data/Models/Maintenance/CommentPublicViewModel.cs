namespace CSIDE.Data.Models.Maintenance
{
    public class CommentPublicViewModel
    {

        public DateOnly CommentDate { get; set; }
        public required string CommentText { get; set; } = string.Empty;
        public string? AuthorName {  get; set; }


    }
}
