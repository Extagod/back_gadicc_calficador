using Capa_Abstracciones.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Panel_Admin;

public partial class LoginForm : Form
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _serviceProvider;
    private int _intentosFallidos;
    private DateTime? _bloqueadoHasta;

    public LoginForm(IAuthService authService, IServiceProvider serviceProvider)
    {
        _authService = authService;
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        lblErrorUsuario.Text = "";
        lblErrorPassword.Text = "";
        lblError.Text = "";

        // Check lockout
        if (_bloqueadoHasta.HasValue && DateTime.UtcNow < _bloqueadoHasta.Value)
        {
            var restante = (int)(_bloqueadoHasta.Value - DateTime.UtcNow).TotalSeconds;
            lblError.Text = $"Cuenta bloqueada. Espere {restante} segundos.";
            return;
        }

        // Validate empty fields
        if (string.IsNullOrWhiteSpace(txtUsuario.Text))
        {
            lblErrorUsuario.Text = "El nombre de usuario es obligatorio.";
            return;
        }
        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            lblErrorPassword.Text = "La contraseña es obligatoria.";
            return;
        }

        try
        {
            var resultado = await _authService.AutenticarAsync(
                txtUsuario.Text.Trim(), txtPassword.Text);

            if (resultado.IsAuthenticated)
            {
                _intentosFallidos = 0;
                var mainForm = new MainForm(_serviceProvider);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                _intentosFallidos++;
                if (_intentosFallidos >= 5)
                {
                    _bloqueadoHasta = DateTime.UtcNow.AddSeconds(60);
                    lblError.Text = "Cuenta bloqueada por 60 segundos.";
                }
                else
                {
                    lblError.Text = "Credenciales incorrectas.";
                }
            }
        }
        catch (Exception)
        {
            lblError.Text = "No se puede conectar al servidor. Intente nuevamente.";
        }
    }
}
