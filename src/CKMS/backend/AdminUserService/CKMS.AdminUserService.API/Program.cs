using CKMS.AdminUserService.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Polly.Extensions.Http;
using Polly;
using CKMS.Interfaces.HttpClientServices;
using CKMS.Library.Services;
using CKMS.Contracts.Configuration;
using System.Text.Json;
using CKMS.Interfaces.UserNotification;
using CKMS.Library.UserNotification;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.Configure<Application>(
    builder.Configuration.GetSection("Application"));

var connectionString = builder.Configuration.GetConnectionString("Database") ?? "";
builder.Services.AddDbContext<AdminUserServiceDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IAdminUserRepository), typeof(AdminUserRepository));
builder.Services.AddScoped(typeof(IKitchenRepository), typeof(KitchenRepository));

builder.Services.AddHttpClient<INotificationHttpService, NotificationHttpService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
}).AddPolicyHandler(GetRetryPolicy());

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.MaxDepth = 64;
    //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Seed test data.
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AdminUserServiceDbContext>();

        // Ensure database is created.
        dbContext.Database.EnsureCreated();

        // Seed test data.
        await dbContext.SeedTestDataAsync();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
