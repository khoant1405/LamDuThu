using AutoMapper;
using JSN.Core.Model;
using JSN.Core.ViewModel;

namespace JSN.Core.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Article, ArticleView>();
    }
}