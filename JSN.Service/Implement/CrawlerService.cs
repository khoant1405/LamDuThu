using System.Net;
using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using JSN.Core.Entity;
using JSN.Core.Model;
using JSN.Service.Interface;
using JSN.Shared.Enum;
using JSN.Shared.Utilities;

namespace JSN.Service.Implement
{
    public class CrawlerService : ICrawlerService
    {
        private readonly IRepository<ArticleContent> _articleContentRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CrawlerService(IUnitOfWork unitOfWork, IRepository<Article> articleRepository, IRepository<ArticleContent> articleContentRepository)
        {
            _unitOfWork = unitOfWork;
            _articleRepository = articleRepository;
            _articleContentRepository = articleContentRepository;
        }

        public async Task StartCrawlerAsync(int startPage, int endPage)
        {
            using var httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });

            for (var i = startPage; i < endPage + 1; i++)
            {
                var url = $"{Constants.Page}{i}";
                var html = await httpClient.GetStringAsync(url);
                var document = new HtmlDocument();
                document.LoadHtml(html);
                List<HtmlNode> articles = document.DocumentNode.QuerySelectorAll("div.porta-article-item")
                    .ToList();

                foreach (var item in articles)
                {
                    var articleName = FormatString(item.QuerySelector("div > h2 > a")
                        ?.InnerText);
                    var imageThumb = FormatString(item.QuerySelector("div.message-body > div > div > img")
                        ?.Attributes["k-data-src"].Value);
                    var description = FormatString(item.QuerySelector("div.message-body > div")
                        ?.InnerText, true);
                    var content = "ĐÂY LÀ CONTENT: " + description;

                    var newArticle = new Article
                    {
                        ArticleName = articleName,
                        Status = (int)ArticleStatus.Publish,
                        RefUrl = "",
                        ImageThumb = imageThumb,
                        Description = description,
                        CreatedOn = DateTime.Now, // Provide an actual date
                        CreatedBy = Constants.AdminId,
                        UserId = Constants.AdminId,
                        UserName = Constants.AdminName
                    };

                    await _articleRepository.AddAsync(newArticle);

                    var newArticleContent = new ArticleContent { Content = content, ArticleId = newArticle.Id };
                    await _articleContentRepository.AddAsync(newArticleContent);
                }
            }
        }

        private string FormatString(string? text, bool isDescription = false)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var result = text.Replace("\n", "")
                .Replace("\t", "");

            if (isDescription)
            {
                var pattern = @"<!--.*?-->";
                result = FormatString(Regex.Replace(result, pattern, ""));
                result = result.Replace("&#8203;", "");
            }

            return result.Trim();
        }
    }
}
