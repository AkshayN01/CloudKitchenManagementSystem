using CKMS.AdminUserService.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Polly.Extensions.Http;
using Polly;
using CKMS.Interfaces.HttpClientServices;
using CKMS.Library.Services;
using CKMS.Contracts.Configuration;
using System.Text.Json;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;
using StackExchange.Redis;
using CKMS.Library.Storage;
using CKMS.Library.SeedData.RedisSeed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Database") ?? "";
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "";

builder.Services.Configure<Application>(builder.Configuration.GetSection("Application"));

builder.Services.AddDbContext<AdminUserServiceDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IAdminUserRepository), typeof(AdminUserRepository));
builder.Services.AddScoped(typeof(IKitchenRepository), typeof(KitchenRepository));
builder.Services.AddScoped(typeof(IKitchenAuditRespository), typeof(KitchenAuditRepository));

builder.Services.AddScoped<IAdminUserUnitOfWork, AdminUserUnitOfWork>();

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddScoped<CKMS.Interfaces.Storage.IRedis, Redis>();

builder.Services.AddSingleton<RedisAdminServiceSeed>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<INotificationHttpService, NotificationHttpService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
}).AddPolicyHandler(CKMS.Library.Generic.Utility.GetRetryPolicy());

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

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Application:JWTAuthentication:secretKey"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var allowedDomains = builder.Configuration.GetSection("Application:AllowedDomains").Get<string[]>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: "CorsPolicy", builder =>
    {
        builder.WithOrigins(allowedDomains)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

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

    using(var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<RedisAdminServiceSeed>();
        await seeder.SeedData();
    }
}
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
