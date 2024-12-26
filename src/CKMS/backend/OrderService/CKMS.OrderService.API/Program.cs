using CKMS.Contracts.Configuration;
using CKMS.Interfaces.HttpClientServices;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;
using CKMS.Library.Services;
using CKMS.Library.Storage;
using CKMS.OrderService.DataAccess.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Database") ?? "";
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "";

builder.Services.Configure<Application>(builder.Configuration.GetSection("Application"));

builder.Services.AddDbContext<OrderServiceDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
builder.Services.AddScoped(typeof(IDicountRepository), typeof(DiscountRepository));
builder.Services.AddScoped(typeof(IDiscountUsageRepository), typeof(DiscountUsageRepository));
builder.Services.AddScoped(typeof(IPersonalDiscountRespository), typeof(PersonalDiscountRepository));
builder.Services.AddScoped(typeof(IPaymentRepository), typeof(PaymentRepository));

builder.Services.AddScoped<IOrderUnitOfWork, OrderUnitOfWork>();

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddScoped<CKMS.Interfaces.Storage.IRedis, Redis>();

builder.Services.AddHttpClient<INotificationHttpService, NotificationHttpService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
}).AddPolicyHandler(CKMS.Library.Generic.Utility.GetRetryPolicy());

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: "CorsPolicy", builder =>
    {
        builder.WithOrigins(configuration["Application:AllowedDomain"])
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
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();

        // Ensure database is created.
        dbContext.Database.EnsureCreated();

        // Seed test data.
        await dbContext.SeedTestDataAsync();
    }
}
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
