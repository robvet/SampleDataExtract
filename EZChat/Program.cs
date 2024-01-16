using ChatBotService.Services;
using Contracts;
using EZChat.Client;
using EZChat.Client.Contracts;
using EZChat.Client.Pages;
using EZChat.Client.Services;
using EZChat.Components;
using TokenManager.Services;
using DataExtractionService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

//builder.Services.AddRazorPages();
builder.Services.AddControllers();
//builder.Services.AddHttpClient();


//// This works
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration.GetSection("BaseAddress").Value!),
    Timeout = TimeSpan.FromMinutes(5) // Increase the timeout to 5 minutes
});




// Extra code
//builder.Services.AddHttpClient("MyHttpClient", client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration.GetSection("BaseUri").Value!);
//});

//builder.Services.AddScoped(sp => new HttpClient
//{
//    BaseAddress = new Uri(builder.Configuration.GetSection("BaseUri").Value!)
//});


//builder.Services.AddHttpClient<ChatService>(client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration.GetSection("BaseUri").Value!);
//});

//builder.Services.AddHttpClient<IPOCService, POCService>(client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration.GetSection("BaseAddress").Value!);
//});

//Console.WriteLine(builder.Configuration.GetSection("BaseUri").Value!);
//builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetSection("BaseUri").Value!) });
//builder.Services.AddHttpClient();
//builder.Services.AddScoped<EZCompletionOptions, CompletionOptions>();




// Register external services with DI container
builder.Services.AddSingleton<IChatCompletion, ChatCompletion>();
builder.Services.AddSingleton<ISimpleChatCompletion, SimpleChatCompletion>();
builder.Services.AddSingleton<IDataExtraction, DataExtraction>();
builder.Services.AddSingleton<IDataExtractionCompletion, DataExtractionCompletion>();

//builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddSingleton<ITokenManager, SlidingWindowWithDecay>();
builder.Services.AddSingleton<IWeatherService, EZChat.WeatherService>();
//builder.Services.AddSingleton<IChatService, ChatService>();

// Engage plumbing for IConfiguration and user secrets
IConfiguration configuration = builder.Configuration;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.MapGet("/api/weather", (IWeatherService weatherService) => weatherService.GetWeather());

//app.MapFallbackToPage("/_Host");

app.Run();
