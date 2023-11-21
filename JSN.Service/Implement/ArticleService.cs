using AutoMapper;
using JSN.Core.Entity;
using JSN.Core.Model;
using JSN.Core.ViewModel;
using JSN.Kafka.Helper;
using JSN.Redis.Interface;
using JSN.Service.Interface;
using JSN.Shared.Config;
using JSN.Shared.Enum;
using JSN.Shared.Model;
using JSN.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace JSN.Service.Implement;

public class ArticleService(IRepository<Article> articleRepository, IMapper mapper, IArticlePaginationCacheService articlePaginationCacheService, IUnitOfWork unitOfWork) : IArticleService
{
    public async Task<PaginatedList<ArticleView>> GetArticleFromPageAsync(int page, int pageSize)
    {
        var cacheData = await articlePaginationCacheService.GetPageAsync(page);
        if (cacheData != null)
        {
            return cacheData;
        }

        var query = articleRepository.Where(x => x.Status == (int)ArticleStatus.Publish).OrderByDescending(x => x.Id).AsNoTracking();

        var count = await query.CountAsync();

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => mapper.Map<ArticleView>(x)).ToListAsync();

        var data = new PaginatedList<ArticleView>(items, count, page, pageSize);

        if (count > 0)
        {
            await articlePaginationCacheService.AddPageAsync(data);
        }

        return data;
    }

    public async Task<bool> PublishArticleAsync()
    {
        try
        {
            var listArticle = await articleRepository.Where(x => x.Status == (int)ArticleStatus.Editing).OrderBy(x => x.Id).Take(AppConfig.NumberPublish).ToListAsync();

            if (listArticle.Any())
            {
                listArticle.ForEach(x => x.Status = (int)ArticleStatus.Publish);
                articleRepository.UpdateRange(listArticle);
                await unitOfWork.CommitAsync();

                await articlePaginationCacheService.DeleteAllPageAsync();

                foreach (var item in listArticle)
                {
                    var jsonData = ConvertHelper.ToJson(item, true);
                    if (item.Id % 2 == 0)
                    {
                        KafkaHelper.Instance.PublishMessage("PublishArticleX" + "-" + AppConfig.KafkaConfig.KafkaPrefix, $"Category_{item.CategoryId}", jsonData);
                    }
                    else
                    {
                        KafkaHelper.Instance.PublishMessage("PublishArticleY" + "-" + AppConfig.KafkaConfig.KafkaPrefix, $"Category_{item.CategoryId}", jsonData);
                    }
                }

                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }
}