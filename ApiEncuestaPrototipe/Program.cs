using ApiEncuestaPrototipe.Data;
using ApiEncuestaPrototipe.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicio QR
builder.Services.AddScoped<QRService>();

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
