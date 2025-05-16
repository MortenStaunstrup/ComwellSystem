using API.Repositories;
using API.Repositories.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ISubGoalRepository, SubGoalRepositoryMongoDB>();
DotNetEnv.Env.Load();



builder.Services.AddCors(options =>
{
    options.AddPolicy("policy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("policy");
app.UseAuthorization();
app.MapControllers();

app.Run();