// --- GEREKLİ USING DİREKTİFLERİ ---
using DataAccess.Concrete.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Core.Utilities.Security.Jwt;
using Core.Utilities.Security.Encyption;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Connection String'i al ---
var connectionString = builder.Configuration.GetConnectionString("PostgreSqlCon");

// --- 2. DbContext'i Servislere Ekle ---
builder.Services.AddDbContext<IlacTakipContext>(options =>
    options.UseNpgsql(connectionString)
);

// --- 3. Manager ve Dal Servislerini Ekle (Dependency Injection) ---

// Auth (Yetkilendirme) servisleri
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddSingleton<ITokenHelper, JwtHelper>(); // TokenHelper'ı Singleton olarak ekle

// User
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IUserDal, EfUserDal>();

// Medication
builder.Services.AddScoped<IMedicationService, MedicationManager>();
builder.Services.AddScoped<IMedicationDal, EfMedicationDal>();

// Schedule
builder.Services.AddScoped<IScheduleService, ScheduleManager>();
builder.Services.AddScoped<IScheduleDal, EfScheduleDal>();

// Intake
builder.Services.AddScoped<IIntakeService, IntakeManager>();
builder.Services.AddScoped<IIntakeDal, EfIntakeDal>();

// CaregiverPatient (Takip İlişkisi)
builder.Services.AddScoped<ICaregiverPatientService, CaregiverPatientManager>();
builder.Services.AddScoped<ICaregiverPatientDal, EfCaregiverPatientDal>();

// --- YENİ ADIM: Authentication (Kimlik Doğrulama) Servisini Ekle ---
// API'ye gelen 'Bearer' token'ları nasıl doğrulayacağını burada tanımlıyoruz.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // appsettings.json'dan TokenOptions'ı çek
        var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,         // Yayıncıyı (Issuer) doğrula
            ValidateAudience = true,     // Kitleyi (Audience) doğrula
            ValidateLifetime = true,       // Token'ın süresini (Expiration) doğrula
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true, // İmza anahtarını doğrula
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
    });
// ---------------------------------------------------------------


// --- 4. API Servislerini Ekle ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- UYGULAMAYI OLUŞTUR ---
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); //https devre dışı
app.UseAuthentication(); // <-- Önce kimliğini doğrula
app.UseAuthorization();  // <-- Sonra yetkilerini kontrol etapp.MapControllers();
app.MapControllers();
app.Run();