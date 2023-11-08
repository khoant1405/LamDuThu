using JSN.Core.ViewModel;
using JSN.Shared.Model;

namespace JSN.Redis.Interface;

public interface IArticlePaginationCacheService
{
    void AddPage(PaginatedList<ArticleView> entity);
    Task AddPageAsync(PaginatedList<ArticleView> entity);

    void DeletePage(int page);

    //void DeleteAllPage();
    Task DeletePageAsync(int page);

    //Task DeleteAllPageAsync();
    PaginatedList<ArticleView>? GetPage(int page);
    Task<PaginatedList<ArticleView>?> GetPageAsync(int page);
}
