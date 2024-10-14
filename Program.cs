using BlazorAISample1.Components;
using BlazorAISample1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(sp => new HttpClient());


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register the ChatGPTService
builder.Services.AddSingleton<ChatGPTService>();
builder.Services.AddScoped<OllamaService>();

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

app.Run();
