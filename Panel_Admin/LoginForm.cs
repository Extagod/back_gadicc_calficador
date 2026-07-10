using Capa_Abstracciones.Interfaces;

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

        // Bloqueo temporal por intentos fallidos
        if (_bloqueadoHasta.HasValue && DateTime.UtcNow < _bloqueadoHasta.Value)
        {
            var restante = (int)(_bloqueadoHasta.Value - DateTime.UtcNow).TotalSeconds;
            lblError.Text = $"Cuenta bloqueada. Espere {restante} segundos.";
            return;
        }

        // Validación de campos vacíos
        bool valido = true;
        if (string.IsNullOrWhiteSpace(txtUsuario.Text))
        {
            lblErrorUsuario.Text = "El usuario es obligatorio.";
            valido = false;
        }
        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            lblErrorPassword.Text = "La contraseña es obligatoria.";
            valido = false;
        }
        if (!valido) return;

        // Indicador de carga
        btnLogin.Enabled = false;
        btnLogin.Text = "Verificando...";
        lblCargando.Text = "Validando credenciales, por favor espere...";
        Application.DoEvents();

        try
        {
            var resultado = await _authService.AutenticarAsync(
                txtUsuario.Text.Trim(), txtPassword.Text);

            if (resultado.IsAuthenticated)
            {
                _intentosFallidos = 0;
                lblCargando.Text = "Acceso concedido. Abriendo panel...";
                var mainForm = new MainForm(_serviceProvider, resultado.NombreUsuario ?? "Administrador");
                mainForm.FormClosed += (_, _) => Close();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                _intentosFallidos++;
                lblCargando.Text = "";
                if (_intentosFallidos >= 5)
                {
                    _bloqueadoHasta = DateTime.UtcNow.AddSeconds(60);
                    lblError.Text = "Cuenta bloqueada por 60 segundos (5 intentos fallidos).";
                }
                else
                {
                    lblError.Text = $"Credenciales incorrectas. Intento {_intentosFallidos} de 5.";
                }
            }
        }
        catch (Exception)
        {
            lblCargando.Text = "";
            lblError.Text = "No se puede conectar al servidor. Intente nuevamente.";
        }
        finally
        {
            btnLogin.Enabled = true;
            btnLogin.Text = "Iniciar Sesión";
            if (lblError.Text != "") lblCargando.Text = "";
        }
    }
}
