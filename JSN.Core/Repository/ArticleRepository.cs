using JSN.Core.Entity;
using JSN.Core.Model;
using JSN.Core.Repository.Interface;

namespace JSN.Core.Repository
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        public ArticleRepository(DbFactory dbFactory) : base(dbFactory)
        {
        }

        public Article CreateNewArticle(Article article)
        {
            Add(article);
            return article;
        }
    }
}
