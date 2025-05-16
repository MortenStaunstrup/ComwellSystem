using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ComwellWeb;
using ComwellWeb.Services;
using ComwellWeb.Services.Interfaces;
using Blazored.LocalStorage;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddSingleton(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddSingleton<ISubGoalService, SubGoalServiceServer>();
builder.Services.AddScoped<IUserService, UserService>(); //undtagelsesvist brug af scoped
builder.Services.AddSingleton<ICommentService, CommentServiceServer>();



await builder.Build().RunAsync();