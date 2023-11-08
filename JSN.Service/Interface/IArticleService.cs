using JSN.Core.ViewModel;
using JSN.Shared.Model;

namespace JSN.Service.Interface;

public interface IArticleService
{
    Task<PaginatedList<ArticleView>> GetArticleFromPageAsync(int page, int pageSize);
    Task PublishArticleAsync();
}