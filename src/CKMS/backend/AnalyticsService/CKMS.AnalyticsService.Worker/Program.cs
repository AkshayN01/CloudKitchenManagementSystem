using CKMS.AnalyticsService.Worker;
using CKMS.AnalyticsService.Worker.OrderWorker;
using CKMS.Contracts.Configuration;
using CKMS.Interfaces.Queue;
using CKMS.Library.Queue;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<Application>(builder.Configuration.GetSection("Application"));
builder.Services.AddSingleton<IRabbitMQ, RabbitMQConnection>();
builder.Services.AddHostedService<OrderServiceWorker>();

var host = builder.Build();
host.Run();
