using AutoMapper;
using JSN.Core.Entity;
using JSN.Core.Model;
using JSN.Core.ViewModel;
using JSN.Redis.Interface;
using JSN.Service.Interface;
using JSN.Shared.Enum;
using JSN.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace JSN.Service.Implement;

public class ArticleService : IArticleService
{
    private readonly IArticlePaginationCacheService _articlePaginationCacheService;
    private readonly IRepository<Article> _articleRepository;
    private readonly IMapper _mapper;

    public ArticleService(IRepository<Article> articleRepository, IMapper mapper,
        IArticlePaginationCacheService articlePaginationCacheService)
    {
        _articleRepository = articleRepository;
        _mapper = mapper;
        _articlePaginationCacheService = articlePaginationCacheService;
    }

    public async Task<PaginatedList<ArticleView>> GetArticleFromPageAsync(int page, int pageSize)
    {
        try
        {
            var cacheData = await _articlePaginationCacheService.GetPageAsync(page);
            if (cacheData != null)
            {
                return cacheData;
            }

            var query = _articleRepository.Where(x => x.Status == (int)ArticleStatus.Publish)
                .OrderByDescending(x => x.Id)
                .AsNoTracking();

            var count = await query.CountAsync();

            var items = await query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => _mapper.Map<ArticleView>(x))
                .ToListAsync();

            var data = new PaginatedList<ArticleView>(items, count, page, pageSize);

            if (count > 0)
            {
                await _articlePaginationCacheService.AddPageAsync(data);
            }

            return data;
        }
        catch (Exception exception)
        {
            throw exception;
        }
    }
}