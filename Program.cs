using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.Repositories.Inspection;
using MyBackend.Repositories.Sequence;
using MyBackend.Repositories.Test;
using MyBackend.Services.Inspection;
using MyBackend.Services.Test;


var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuration & Services ---
builder.Services.AddControllers()
.AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // ตั้งค่าให้ข้ามการโชว์ Property ที่มีค่าเป็น null ทั้งโปรเจกต์
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping; // set support lang thai
    });

// --- 2. Data Access (Database) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// สำหรับ Config AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); }, typeof(Program).Assembly); // Scan Profile
builder.Services.AddScoped<IBaseMapper, BaseMapper>();
// สำหรับ Dapper ให้รองรับ Snake Case (วิธีแก้แบบถาวรทั้งโปรเจกต์)
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
// สำหรับ Dapper => Inject เข้าไปใน Repository
builder.Services.AddSingleton<DbConnectionFactory>(); // ใช้ Singleton เพราะ ConnectionString ไม่เปลี่ยน
// สำหรับ Entity Framework
//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString)); //PostgreSQL
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString)); //SqlServer

// --- 3. Dependency Injection (Business Logic) ---
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInspectionRepository, InspectionRepository>();
builder.Services.AddScoped<IInspectionService, InspectionService>();
builder.Services.AddScoped<ISequenceRepository, SequenceRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. Middleware Pipeline --- // เปิด Swagger ทั้งใน Dev และ Production (ตามความต้องการ)
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

// ปิด HTTPS Redirection เฉพาะในโหมด Dev/Docker เพื่อลด Warning
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// MapControllers เพื่อบอกให้ระบบนำ Route จาก Controller มาใช้
app.MapControllers();
app.Run();


