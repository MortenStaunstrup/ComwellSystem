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
    BaseAddress = new Uri("http://localhost:5116/api/")
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<ISubGoalService, SubGoalServiceServer>();
builder.Services.AddSingleton<ICommentService, CommentServiceServer>();
builder.Services.AddScoped<INotificationService, NotificationService>();




await builder.Build().RunAsync();