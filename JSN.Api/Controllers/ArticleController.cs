using JSN.Core.ViewModel;
using JSN.Service.Interface;
using JSN.Shared.Config;
using JSN.Shared.Model;
using Microsoft.AspNetCore.Mvc;

namespace JSN.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet("[action]")]
    public async Task<ActionResult<PaginatedList<ArticleView>>> GetArticleFromPage(int page)
    {
        if (double.IsNaN(page) || double.IsNaN(AppConfig.ArticlePageSize)) return BadRequest("Invalid Page");

        var articles = await _articleService.GetArticleFromPageAsync(page, AppConfig.ArticlePageSize);

        return Ok(articles);
    }
}