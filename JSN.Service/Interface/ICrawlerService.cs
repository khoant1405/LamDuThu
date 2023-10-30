namespace JSN.Service.Interface;

public interface ICrawlerService
{
    Task StartCrawlerAsync(int startPage, int endPage);
}