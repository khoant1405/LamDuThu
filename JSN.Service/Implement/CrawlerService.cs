using System.Net;
using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using JSN.Core.Entity;
using JSN.Core.Model;
using JSN.Service.Interface;
using JSN.Shared.Enum;
using JSN.Shared.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace JSN.Service.Implement;

public class CrawlerService(IUnitOfWork unitOfWork, IRepository<Article> articleRepository) : ICrawlerService
{
    public async Task StartCrawlerAsync(int startPage, int endPage)
    {
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        var random = new Random();
        var listArticle = new List<Article>();

        for (var i = startPage; i < endPage + 1; i++)
        {
            var url = $"{JsnStatic.Page}{i}";
            var html = await httpClient.GetStringAsync(url);
            var document = new HtmlDocument();
            document.LoadHtml(html);
            List<HtmlNode> articles = document.DocumentNode.QuerySelectorAll("div.porta-article-item").ToList();

            foreach (var item in articles)
            {
                var articleName = CleanHtmlEntities(item.QuerySelector("div > h2 > a")?.InnerText);
                if (articleName.IsNullOrEmpty())
                {
                    continue;
                }

                var imageThumb = CleanHtmlEntities(item.QuerySelector("div.message-body > div > div > img")?.Attributes["k-data-src"].Value);
                var description = CleanHtmlEntities(item.QuerySelector("div.message-body > div")?.InnerText, true);
                var content = "CONTENT: " + description;

                var daysToAdd = random.Next(1, 101);
                var currentDate = DateTime.Now;
                var time = currentDate.AddDays(-daysToAdd);

                var categoryId = random.Next(1, 6);

                var newArticle = new Article
                {
                    ArticleName = articleName,
                    Status = (int)ArticleStatus.Editing,
                    RefUrl = "",
                    ImageThumb = imageThumb,
                    Description = description,
                    CreatedOn = time,
                    CreatedBy = JsnStatic.AdminId,
                    UserId = JsnStatic.AdminId,
                    UserName = JsnStatic.AdminName,
                    CategoryId = categoryId,
                    ArticleContent = new ArticleContent
                    {
                        Content = content,
                        CreatedOn = time,
                        CreatedBy = JsnStatic.AdminId
                    }
                };
                listArticle.Add(newArticle);
            }

            await Task.Delay(5000);
        }

        if (listArticle.Any())
        {
            listArticle = listArticle.OrderBy(x => x.CreatedOn).ToList();
            await articleRepository.AddRangeAsync(listArticle);
            await unitOfWork.CommitAsync();
        }
    }

    private static string CleanHtmlEntities(string? inputText, bool isDescription = false)
    {
        if (string.IsNullOrEmpty(inputText))
        {
            return string.Empty;
        }

        // Remove specific HTML entities
        var cleanedText = inputText.Replace("\n", "").Replace("\t", "").Replace("&#8203;", "").Replace("&lt;", "").Replace("&gt;", "").Replace("&amp;", "").Replace("&quot;", "").Replace("&apos;", "");

        if (!isDescription)
        {
            return cleanedText.Trim();
        }

        // Remove HTML comments
        const string commentPattern = @"<!--.*?-->";
        cleanedText = Regex.Replace(cleanedText, commentPattern, "");

        // Trim and return the cleaned text
        return cleanedText.Trim();
    }
}