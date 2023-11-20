using JSN.Core.Entity;
using JSN.Core.Model;
using JSN.Core.Repository.Interface;

namespace JSN.Core.Repository;

public class ArticleRepository(DbFactory dbFactory) : Repository<Article>(dbFactory), IArticleRepository
{
    public Article CreateNewArticle(Article article)
    {
        Add(article);
        return article;
    }
}