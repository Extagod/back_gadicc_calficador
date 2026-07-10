using System.Globalization;
using Panel_Admin.UI;
using Panel_Admin.Views;

namespace Panel_Admin;

public partial class MainForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _nombreUsuario;

    private DashboardView? _dashboard;
    private FuncionariosView? _funcionarios;
    private CalificacionesView? _calificaciones;
    private ReportesView? _reportes;

    private string _moduloActual = "";

    public MainForm(IServiceProvider serviceProvider, string nombreUsuario = "Administrador")
    {
        _serviceProvider = serviceProvider;
        _nombreUsuario = nombreUsuario;
        InitializeComponent();

        // Header: usuario y fecha
        lblUsuario.Text = _nombreUsuario;
        lblAvatar.Text = string.IsNullOrWhiteSpace(_nombreUsuario) ? "A" : _nombreUsuario.Substring(0, 1).ToUpper();
        var cultura = new CultureInfo("es-ES");
        lblFecha.Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy", cultura);

        this.Load += (_, _) => Navegar("dashboard");
    }

    private void Navegar(string modulo)
    {
        if (_moduloActual == modulo) return;
        _moduloActual = modulo;

        // Estado visual de la navegación
        navDashboard.Active = modulo == "dashboard";
        navFuncionarios.Active = modulo == "funcionarios";
        navCalificaciones.Active = modulo == "calificaciones";
        navReportes.Active = modulo == "reportes";

        content.Controls.Clear();
        Control? vista = null;
        string titulo = "", desc = "";

        switch (modulo)
        {
            case "dashboard":
                _dashboard ??= new DashboardView(_serviceProvider);
                vista = _dashboard;
                titulo = "Dashboard";
                desc = "Resumen general y estadísticas de calificaciones";
                _dashboard.CargarDatos();
                break;
            case "funcionarios":
                _funcionarios ??= new FuncionariosView(_serviceProvider);
                vista = _funcionarios;
                titulo = "Gestión de Funcionarios";
                desc = "Administre los funcionarios registrados y sus códigos QR";
                _funcionarios.CargarDatos();
                break;
            case "calificaciones":
                _calificaciones ??= new CalificacionesView(_serviceProvider);
                vista = _calificaciones;
                titulo = "Calificaciones recibidas";
                desc = "Consulte y analice las valoraciones registradas por los ciudadanos";
                _calificaciones.CargarDatos();
                break;
            case "reportes":
                _reportes ??= new ReportesView(_serviceProvider);
                vista = _reportes;
                titulo = "Reportes";
                desc = "Genere y exporte reportes por rango de fechas y funcionario";
                _reportes.Inicializar();
                break;
        }

        lblSeccion.Text = titulo;
        lblSeccionDesc.Text = desc;

        if (vista != null)
        {
            vista.Dock = DockStyle.Fill;
            content.Controls.Add(vista);
        }
    }

    private void CerrarSesion()
    {
        if (Notificador.Confirmar(this, "¿Desea cerrar la sesión actual y volver a la pantalla de inicio?",
            "Cerrar sesión", "Cerrar sesión"))
        {
            this.Close();
        }
    }
}
