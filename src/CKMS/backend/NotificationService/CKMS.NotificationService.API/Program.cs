using CKMS.Contracts.Configuration;
using CKMS.Interfaces.Repository;
using CKMS.Interfaces.UserNotification;
using CKMS.Library.Interfaces;
using CKMS.Library.UserNotification;
using CKMS.NotificationService.API.Hubs;
using CKMS.NotificationService.API.UserNotificationService;
using CKMS.NotificationService.DataAccess.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Database") ?? "";
builder.Services.AddDbContext<NotificationServiceDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(INotificationRepository), typeof(NotificationRepository));
builder.Services.AddScoped(typeof(INotificationAuditRepository), typeof(NotificationAuditRepository));

builder.Services.AddScoped<INotificationUnitOfWork, NotificationUnitOfWork>();

// Register all Notification Handlers
builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("Application:Connection:SendGridOptions"));
builder.Services.AddScoped<IUserNotification, EmailService>();
builder.Services.AddScoped<IUserNotification, WebNotificationService>();

// Register the NotificationService
builder.Services.AddScoped<NotificationHandler>();

builder.Services.AddSignalR();
builder.Services.AddControllers();
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
}
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AdminNotificationHub>("/admin-notification-hub");
app.MapHub<CustomerNotificationHub>("/customer-notification-hub");

app.Run();
