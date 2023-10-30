using JSN.Core.Entity;
using JSN.Core.Model;

namespace JSN.Core.Repository.Interface;

public interface IArticleRepository : IRepository<Article>
{
    Article CreateNewArticle(Article article);
}