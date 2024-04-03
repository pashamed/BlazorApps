using UrlShortener.Abstaractions;
using UrlShortener.Components;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IUrlShortenerService, UrlShortenerService>();

builder.Services.AddSingleton<UrlShortenerDatabase>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var databasePath = configuration.GetValue<string>("LiteDbOptions:DatabasePath");
    return new UrlShortenerDatabase(databasePath!);
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapFallback(async context =>
{
    var urlShortenerService = context.RequestServices.GetRequiredService<IUrlShortenerService>();
    var path = context.Request.Scheme + "://" + context.Request.Host + context.Request.Path;
    var fullUrl = await urlShortenerService.GetFullUrl(path);
    if (fullUrl != null)
    {
        context.Response.Redirect(fullUrl);
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("URL not found.");
    }
});

app.Run();