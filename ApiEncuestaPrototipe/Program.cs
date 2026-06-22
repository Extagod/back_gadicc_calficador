using Capa_Datos;
using Capa_Datos.Repositories;
using Capa_Abstracciones.Interfaces;
using Capa_Servicios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IEncargadoRepository, EncargadoRepository>();
builder.Services.AddScoped<ICalificacionRepository, CalificacionRepository>();
builder.Services.AddScoped<IUsuarioAdminRepository, UsuarioAdminRepository>();

// Services
builder.Services.AddScoped<IQRService, QRServiceImpl>();
builder.Services.AddScoped<IEncargadoService, EncargadoService>();
builder.Services.AddScoped<ICalificacionService, CalificacionService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS — se registra el policy ANTES de builder.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ─── A partir de aquí ya existe `app` ───────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS — se aplica el policy DESPUÉS de builder.Build()
// Debe ir ANTES de UseAuthorization y MapControllers
app.UseCors("FrontendPolicy");

app.UseAuthorization();
app.MapControllers();
app.Run();
