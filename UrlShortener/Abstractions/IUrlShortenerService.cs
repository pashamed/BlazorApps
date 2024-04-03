using UrlShortener.Services;

namespace UrlShortener.Abstaractions
{
    public interface IUrlShortenerService
    {
        Task<string> ShortenUrl(string url);

        Task<string?> GetFullUrl(string url);

        Task<List<UrlEntry>> GetAllUrls();

        Task DeleteShortUrl(string shortenedUrl);
    }
}