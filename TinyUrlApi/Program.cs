using Microsoft.EntityFrameworkCore;
using TinyUrlApi.Data;
using TinyUrlApi.Models;
using TinyUrlApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddSingleton<ShortCodeService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//  Default route
app.MapGet("/", () => "Tiny URL API is running...");

//  Create Short URL
app.MapPost("/shorten", async (AppDbContext db, ShortCodeService service, UrlRequest request) =>
{
    //  Validation
    if (string.IsNullOrWhiteSpace(request.OriginalUrl))
        return Results.BadRequest("URL is required");

    if (!Uri.IsWellFormedUriString(request.OriginalUrl, UriKind.Absolute))
        return Results.BadRequest("Invalid URL format");

    try
    {
        var shortCode = await service.GenerateUniqueCode(db);

        var url = new UrlMapping
        {
            OriginalUrl = request.OriginalUrl,
            ShortCode = shortCode
        };

        db.Urls.Add(url);
        await db.SaveChangesAsync();

        return Results.Ok(new { shortCode });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Something went wrong: {ex.Message}");
    }
});


//  Redirect to Original URL


app.MapGet("/{code}", async (AppDbContext db, string code) =>
{
    var url = await db.Urls.FirstOrDefaultAsync(x => x.ShortCode == code);

    if (url == null)
        return Results.NotFound("Short URL not found");

    //  Track clicks
    url.ClickCount++;
    await db.SaveChangesAsync();

    return Results.Redirect(url.OriginalUrl);
});

// Get All URLs (optional)


app.MapGet("/stats/{code}", async (AppDbContext db, string code) =>
{
    var url = await db.Urls.FirstOrDefaultAsync(x => x.ShortCode == code);

    if (url == null)
        return Results.NotFound();

    return Results.Ok(new
    {
        url.OriginalUrl,
        url.ShortCode,
        url.ClickCount,
        url.CreatedAt
    });
});

app.Run();