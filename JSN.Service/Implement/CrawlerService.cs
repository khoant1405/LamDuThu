using System.Net;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using JSN.Core.Entity;
using JSN.Core.Model;
using JSN.Service.Interface;
using JSN.Shared.Enum;
using JSN.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

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
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            var httpClient = new HttpClient(httpClientHandler);
            var listArticle = new List<Article>();
            var lisArticleContent = new List<ArticleContent>();
            var listId = _articleRepository.Where(x => x.Status == (int)ArticleStatus.Publish)
                .AsNoTracking()
                .Select(x => x.Id)
                .ToList();

            for (var i = startPage; i < endPage + 1; i++)
            {
                var url = $"{Constants.Page}{i}";
                var html = await httpClient.GetStringAsync(url);
                var document = new HtmlDocument();
                document.LoadHtml(html);
                List<HtmlNode> articles = document.DocumentNode.QuerySelectorAll("div.zone--timeline > article")
                    .ToList();

                foreach (var item in articles)
                {
                    var article = item.QuerySelector("a");

                    if (article == null) continue;

                    var articleId = int.Parse(item.Attributes["data-id"].Value);
                    if (listId.Contains(articleId) || listArticle.Exists(x => x.Id == articleId)) continue;

                    var articleName = article.Attributes["title"].Value;
                    var href = article.Attributes["href"].Value;
                    var imageThumb = article.QuerySelector("img")
                        ?.Attributes["data-src"]
                        .Value.Replace("150x100", "200x150");
                    var description = item.QuerySelector("div > div.summary > p")
                        ?.InnerText.Replace("\n", "");

                    var htmlArticle = await httpClient.GetStringAsync(href);
                    var documentArticle = new HtmlDocument();
                    documentArticle.LoadHtml(htmlArticle);
                    var content = documentArticle.DocumentNode.QuerySelector("div.cms-body")
                        ?.OuterHtml;
                    var time = documentArticle.DocumentNode.QuerySelector("meta.cms-date")
                        ?.Attributes["content"].Value;
                    var dateTime = time != null ? DateTime.Parse(time) : DateTime.MinValue;

                    var newArticle = new Article
                    {
                        Id = articleId,
                        ArticleName = articleName,
                        Status = (int)ArticleStatus.Publish,
                        CreatedOn = dateTime,
                        CreatedBy = Constants.AdminId,
                        RefUrl = href,
                        ImageThumb = imageThumb,
                        Description = description,
                        UserId = Constants.AdminId,
                        UserName = Constants.AdminName
                    };
                    listArticle.Add(newArticle);

                    var newArticleContent = new ArticleContent { Content = content, ArticleId = articleId };
                    lisArticleContent.Add(newArticleContent);
                }
            }

            if (listArticle.Count > 0)
            {
                await _articleRepository.AddRangeAsync(listArticle);
                await _articleContentRepository.AddRangeAsync(lisArticleContent);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
