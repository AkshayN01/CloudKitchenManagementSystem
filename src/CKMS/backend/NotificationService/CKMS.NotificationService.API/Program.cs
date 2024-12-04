using CKMS.Interfaces.UserNotification;
using CKMS.Library.UserNotification;
using CKMS.NotificationService.API.Hubs;
using CKMS.NotificationService.API.UserNotificationService;
using CKMS.NotificationService.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Database") ?? "";
builder.Services.AddDbContext<NotificationServiceDbContext>(options => options.UseNpgsql(connectionString));

// Register all Notification Handlers
builder.Services.AddScoped<IUserNotification, EmailService>();
builder.Services.AddScoped<IUserNotification, WebNotificationService>();

// Register the NotificationService
builder.Services.AddScoped<NotificationHandler>();

builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<AdminNotificationHub>("/admin-notification-hub");
app.MapHub<CustomerNotificationHub>("/customer-notification-hub");

app.Run();
