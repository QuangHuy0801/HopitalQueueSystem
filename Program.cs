using backend.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyProject.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SqlConnectionFactory>();
builder.Services.AddSingleton<PatientQueueRepository>();
builder.Services.AddSingleton<ViewQueueRepository>();

// Thêm SignalR service
builder.Services.AddSignalR();

// 👉 Thêm CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Thay bằng domain frontend nếu khác
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();  // cần nếu dùng SignalR với websockets
        });
});

var app = builder.Build();

app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHub<NotificationHub>("/notificationHub");

app.MapControllers();

app.Run();
