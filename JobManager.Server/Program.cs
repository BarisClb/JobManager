using JobManager.Server.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);
builder.Services.RegisterLogging(builder.Configuration);
builder.Services.RegisterApplicationServices(builder.Configuration);
builder.Services.RegisterHangfireServices(builder.Configuration);

var app = builder.Build();

app.RegisterApps();
app.RegisterHangfireApps(builder.Services, builder.Configuration);

app.Run();
