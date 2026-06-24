using Capa_Abstracciones.Interfaces;
using Capa_Datos;
using Capa_Datos.Repositories;
using Capa_Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Panel_Admin;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            MessageBox.Show(
                "La cadena de conexión 'DefaultConnection' no está configurada en App.config.",
                "Error de Configuración",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        var services = new ServiceCollection();

        // Configuration (for services that need IConfiguration)
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:FrontendUrl"] = System.Configuration.ConfigurationManager.AppSettings["FrontendUrl"] ?? "http://localhost:5173"
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // EF Core + Oracle
        services.AddDbContext<AppDbContext>(options =>
            options.UseOracle(connectionString));

        // Repositories
        services.AddScoped<IEncargadoRepository, EncargadoRepository>();
        services.AddScoped<ICalificacionRepository, CalificacionRepository>();
        services.AddScoped<IUsuarioAdminRepository, UsuarioAdminRepository>();

        // Services
        services.AddScoped<IEncargadoService, EncargadoService>();
        services.AddScoped<ICalificacionService, CalificacionService>();
        services.AddScoped<IQRService, QRServiceImpl>();
        services.AddScoped<IAuthService, AuthService>();

        var serviceProvider = services.BuildServiceProvider();

        var authService = serviceProvider.GetRequiredService<IAuthService>();
        Application.Run(new LoginForm(authService, serviceProvider));
    }
}
