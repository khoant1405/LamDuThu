using JSN.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JSN.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CrawlerController : ControllerBase
{
    private readonly ICrawlerService _crawlerService;

    public CrawlerController(ICrawlerService crawlerService)
    {
        _crawlerService = crawlerService;
    }

    [HttpPost("[action]")]
    //[Authorize]
    public async Task<ActionResult> StartCrawler(int startPage, int endPage)
    {
        if (double.IsNaN(startPage) || double.IsNaN(endPage))
        {
            return BadRequest("Invalid Page");
        }

        await _crawlerService.StartCrawlerAsync(startPage, endPage);

        return Ok();
    }
}