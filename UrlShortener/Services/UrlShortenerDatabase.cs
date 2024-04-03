using LiteDB;

namespace UrlShortener.Services
{
    public class UrlEntry
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortenedUrl { get; set; } = string.Empty;
    }

    public class UrlShortenerDatabase(string databasePath)
    {
        private readonly LiteDatabase db = new LiteDatabase(databasePath);

        public void InsertUrl(UrlEntry urlEntry)
        {
            var urlCollection = db.GetCollection<UrlEntry>("urls");
            urlCollection.Insert(urlEntry);
        }

        public UrlEntry GetUrl(string shortenedUrl)
        {
            var urlCollection = db.GetCollection<UrlEntry>("urls");
            return urlCollection.FindOne(x => x.ShortenedUrl == shortenedUrl);
        }

        public UrlEntry GetUrlByOriginalUrl(string originalUrl)
        {
            var urlCollection = db.GetCollection<UrlEntry>("urls");
            return urlCollection.FindOne(x => x.OriginalUrl == originalUrl);
        }

        public IEnumerable<UrlEntry> GetAllUrls()
        {
            return db.GetCollection<UrlEntry>("urls").FindAll().ToList();
        }

        public void DeleteUrl(string shortenedUrl)
        {
            db.GetCollection<UrlEntry>("urls").DeleteMany(x => x.ShortenedUrl == shortenedUrl);
        }
    }
}