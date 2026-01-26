using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.Repositories.Test;
using MyBackend.Services.Inspection;
using MyBackend.Services.Test;


var builder = WebApplication.CreateBuilder(args);

// 1. เพิ่ม Service เพื่อให้ระบบรู้จัก Controller
builder.Services.AddControllers()
.AddJsonOptions(options =>
    {
        // ตั้งค่าให้ข้ามการโชว์ Property ที่มีค่าเป็น null ทั้งโปรเจกต์
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        // (แถม) ตั้งค่าภาษาไทยที่คุณเคยทำไว้
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });
// ให้ .NET รู้จัก Service
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInspectionService, InspectionService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// เพิ่มบรรทัดนี้ก่อน var app = builder.Build();
//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=MyDb.db"));

// ดึง Connection String จาก appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// เปลี่ยนจาก .UseSqlite เป็น .UseNpgsql
//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));
// เปลี่ยนจาก .UseNpgsql เป็น .UseSqlServer
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

// 2. เพิ่มคำสั่ง MapControllers เพื่อบอกให้ระบบนำ Route จาก Controller มาใช้
app.MapControllers();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
