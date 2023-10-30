using JSN.Core.ViewModel;

namespace JSN.Redis.Interface;

public interface IArticleCacheService : IRedis<ArticleView>
{
}