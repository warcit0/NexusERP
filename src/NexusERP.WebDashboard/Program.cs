using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NexusERP.WebDashboard;

using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using NexusERP.WebDashboard.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5174/") });

builder.Services.AddMudServices();

// Authentication setup
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<NexusERP.WebDashboard.Services.NotificationService>();

await builder.Build().RunAsync();
