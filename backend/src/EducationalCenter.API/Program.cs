using EducationalCenter.Application;
using EducationalCenter.Domain.Grades;
using EducationalCenter.Domain.Teachers;
using EducationalCenter.Domain.Students;
using EducationalCenter.Domain.Shared;
using EducationalCenter.Persistence.Contexts;
using EducationalCenter.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة التحكم بالـ Controllers وتفعيل المعايير القياسية
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. قراءة بيانات الاتصال بشكل آمن من الإعدادات (Best Practice)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 3. تسجيل سياق قاعدة البيانات
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("EducationalCenter.Persistence")));

// 4. تسجيل البنية التحتية والمستودعات وعلاقات الحقن (Dependency Inversion)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// 5. تسجيل خدمات طبقة الـ Application وتفعيل الـ MediatR
builder.Services.AddApplicationServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            // هنا نسمح برابط الواجهة الأمامية (تأكد من منفذ الـ Frontend لديك، غالباً 5173)
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// 6. تفعيل بيئة Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// تفعيل سياسة السماح التي أنشأناها في الأعلى
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();

app.Run();
