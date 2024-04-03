using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using UrlShortener.Abstaractions;
using UrlShortener.Services;

namespace UrlShortener.Components.Pages // Update this namespace to match your project's structure
{
    public partial class ShortenUrl
    {
        [Parameter]
        public string? OriginalUrl { get; set; }

        [Parameter]
        public string? ShortenedUrl { get; set; }

        public string? InputShortenedUrl { get; set; }

        public string? FullUrl { get; set; }

        public string? ErrorMessage { get; set; }

        [Inject]
        public IUrlShortenerService UrlShortenerService { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public List<UrlEntry>? UrlList { get; set; }

        private async Task ShortenUrlAsync()
        {
            if (!string.IsNullOrWhiteSpace(OriginalUrl))
            {
                string shortenedUrl = await UrlShortenerService.ShortenUrl(OriginalUrl);
                ShortenedUrl = shortenedUrl;
            }

            UrlList = await UrlShortenerService.GetAllUrls();
        }

        private async Task GetFullUrlAsync()
        {
            ErrorMessage = string.Empty; // Reset the error message each time the method is called
            if (!string.IsNullOrWhiteSpace(InputShortenedUrl))
            {
                FullUrl = await UrlShortenerService.GetFullUrl(InputShortenedUrl);
                if (FullUrl == null)
                {
                    ErrorMessage = "The shortened URL was not found.";
                }
            }
        }

        private async Task CopyToClipboard()
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", ShortenedUrl);
        }

        protected override async Task OnInitializedAsync()
        {
            UrlList = await UrlShortenerService.GetAllUrls();
        }

        private async Task DeleteUrl(string shortenedUrl)
        {
            await UrlShortenerService.DeleteShortUrl(shortenedUrl);
            UrlList = await UrlShortenerService.GetAllUrls();
            StateHasChanged();
        }
    }
}