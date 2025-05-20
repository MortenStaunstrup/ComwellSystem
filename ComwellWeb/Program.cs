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


builder.Services.AddSingleton<ISubGoalService, SubGoalServiceServer>();
builder.Services.AddScoped<IUserService, UserService>(); //jeg har spammet scoped
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<ICommentService, CommentServiceServer>();
builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();




await builder.Build().RunAsync();