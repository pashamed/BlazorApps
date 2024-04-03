using System.Security.Cryptography;
using System.Text;

using UrlShortener.Abstaractions;

namespace UrlShortener.Services
{
    public class UrlShortenerService(UrlShortenerDatabase database, IHttpContextAccessor httpContextAccessor) : IUrlShortenerService
    {
        private readonly UrlShortenerDatabase _database = database;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public Task<string> ShortenUrl(string url)
        {
            url = url.StartsWith("https") ? url : "https://" + url;
            var existingEntry = _database.GetUrlByOriginalUrl(url);
            if (existingEntry != null)
            {
                return Task.FromResult(existingEntry.ShortenedUrl);
            }
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(url));

            string hashBase64 = Convert.ToBase64String(hashBytes)[..8];

            string shortenedUrl = hashBase64.Replace('+', '-').Replace('/', '_');

            var baseUrl = _httpContextAccessor.HttpContext?.Request.Scheme + "://" + _httpContextAccessor.HttpContext?.Request.Host;

            var urlEntry = new UrlEntry
            {
                OriginalUrl = url,
                ShortenedUrl = string.Join("/", baseUrl, shortenedUrl)
            };

            _database.InsertUrl(urlEntry);
            return Task.FromResult(urlEntry.ShortenedUrl);
        }

        public Task<string?> GetFullUrl(string url)
        {
            var urlEntry = _database.GetUrl(url);
            return Task.FromResult(urlEntry?.OriginalUrl);
        }

        public Task<List<UrlEntry>> GetAllUrls()
        {
            var data = _database.GetAllUrls().ToList();
            return Task.FromResult(data);
        }

        public Task DeleteShortUrl(string shortenedUrl)
        {
            _database.DeleteUrl(shortenedUrl);
            return Task.CompletedTask;
        }
    }
}