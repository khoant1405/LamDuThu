namespace JSN.Core.ViewModel
{
    public class ArticleView
    {
        public int Id { get; set; }
        public string ArticleName { get; set; } = null!;
        public DateTime? CreationDate { get; set; }
        public string RefUrl { get; set; } = null!;
        public string? ImageThumb { get; set; }
        public string? Description { get; set; }
        public string? UserName { get; set; }
    }
}
