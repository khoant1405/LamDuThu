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

public class ArticleService : IArticleService
{
    private readonly IArticlePaginationCacheService _articlePaginationCacheService;
    private readonly IRepository<Article> _articleRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ArticleService(IRepository<Article> articleRepository, IMapper mapper, IArticlePaginationCacheService articlePaginationCacheService, IUnitOfWork unitOfWork)
    {
        _articleRepository = articleRepository;
        _mapper = mapper;
        _articlePaginationCacheService = articlePaginationCacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedList<ArticleView>> GetArticleFromPageAsync(int page, int pageSize)
    {
        var cacheData = await _articlePaginationCacheService.GetPageAsync(page);
        if (cacheData != null)
        {
            return cacheData;
        }

        var query = _articleRepository.Where(x => x.Status == (int)ArticleStatus.Publish).OrderByDescending(x => x.Id).AsNoTracking();

        var count = await query.CountAsync();

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => _mapper.Map<ArticleView>(x)).ToListAsync();

        var data = new PaginatedList<ArticleView>(items, count, page, pageSize);

        if (count > 0)
        {
            await _articlePaginationCacheService.AddPageAsync(data);
        }

        return data;
    }

    public async Task<bool> PublishArticleAsync()
    {
        try
        {
            var listArticle = await _articleRepository.Where(x => x.Status == (int)ArticleStatus.Editing).OrderBy(x => x.Id).Take(AppConfig.NumberPublish).ToListAsync();

            if (listArticle.Any())
            {
                listArticle.ForEach(x => x.Status = (int)ArticleStatus.Publish);
                _articleRepository.UpdateRange(listArticle);
                await _unitOfWork.CommitAsync();

                foreach (var jsonData in listArticle.Select(item => ConvertHelper.ToJson(item, true)))
                {
                    KafkaHelper.Instance.PublishMessage("PublishArticle" + "-" + AppConfig.KafkaConfig.KafkaPrefix, "", jsonData);
                }

                return true;
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return false;
    }
}