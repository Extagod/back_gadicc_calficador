using Panel_Admin.UI;
using System.Drawing.Drawing2D;

namespace Panel_Admin;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;
    private TextBox txtUsuario;
    private TextBox txtPassword;
    private UIButton btnLogin;
    private Label lblErrorUsuario;
    private Label lblErrorPassword;
    private Label lblError;
    private CheckBox chkMostrar;
    private Label lblCargando;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.txtUsuario = new TextBox();
        this.txtPassword = new TextBox();
        this.btnLogin = new UIButton();
        this.lblErrorUsuario = new Label();
        this.lblErrorPassword = new Label();
        this.lblError = new Label();
        this.chkMostrar = new CheckBox();
        this.lblCargando = new Label();

        this.SuspendLayout();

        // === Form ===
        int W = 1040, H = 660;
        this.ClientSize = new Size(W, H);
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = UITheme.Background;
        this.Text = "GADICC Calificador - Acceso";

        // === Panel lateral izquierdo (branding) ===
        int brandW = 440;
        var panelBrand = new Panel
        {
            Dock = DockStyle.Left,
            Width = brandW,
            BackColor = UITheme.Sidebar
        };
        panelBrand.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var brush = new LinearGradientBrush(panelBrand.ClientRectangle,
                UITheme.Primary, UITheme.Sidebar, 55f);
            e.Graphics.FillRectangle(brush, panelBrand.ClientRectangle);
        };

        // Logo institucional (imagen real con respaldo a texto)
        var logoImg = LogoHelper.Logo;
        if (logoImg != null)
        {
            int lw = 300, lh = 240;
            var pic = new PictureBox
            {
                Image = logoImg,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Size = new Size(lw, lh),
                Location = new Point((brandW - lw) / 2, 150)
            };
            panelBrand.Controls.Add(pic);
        }
        else
        {
            var logo = new Label
            {
                Text = "CAÑAR",
                Font = new Font(UITheme.FontFamilySemibold, 38f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(brandW, 66),
                Location = new Point(0, 210),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var logoSub = new Label
            {
                Text = "MUNICIPIO INTERCULTURAL",
                Font = new Font(UITheme.FontFamily, 12f, FontStyle.Regular),
                ForeColor = UITheme.PrimaryLight,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(brandW, 26),
                Location = new Point(0, 280),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelBrand.Controls.AddRange(new Control[] { logo, logoSub });
        }

        var lblTagline = new Label
        {
            Text = "GADICC Calificador\nSistema de Calificación de Funcionarios",
            Font = new Font(UITheme.FontFamily, 11f, FontStyle.Regular),
            ForeColor = Color.FromArgb(185, 205, 230),
            BackColor = Color.Transparent,
            AutoSize = false,
            Size = new Size(brandW, 56),
            Location = new Point(0, 450),
            TextAlign = ContentAlignment.MiddleCenter
        };
        panelBrand.Controls.Add(lblTagline);
        this.Controls.Add(panelBrand);

        // === Área derecha (formulario) ===
        int baseX = brandW + 60;          // 500
        int fieldW = W - baseX - 70;      // ~470

        var lblBienvenida = new Label
        {
            Text = "Bienvenido",
            Font = new Font(UITheme.FontFamilySemibold, 24f, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            AutoSize = false,
            Size = new Size(fieldW, 44),
            Location = new Point(baseX, 110)
        };
        var lblSubtitulo = new Label
        {
            Text = "Panel Administrativo\nInicie sesión para continuar",
            Font = new Font(UITheme.FontFamily, 11f, FontStyle.Regular),
            ForeColor = UITheme.TextSecondary,
            AutoSize = false,
            Size = new Size(fieldW, 56),
            Location = new Point(baseX, 158)
        };

        var lblUsuario = new Label
        {
            Text = "USUARIO",
            Font = new Font(UITheme.FontFamilySemibold, 8.5f, FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            AutoSize = true,
            Location = new Point(baseX, 224)
        };
        var contUsuario = new UITextBox { Location = new Point(baseX, 247), Size = new Size(fieldW, 46) };
        contUsuario.PlaceholderText = "Ingrese su usuario";
        this.txtUsuario = contUsuario.Inner;

        this.lblErrorUsuario.Text = "";
        this.lblErrorUsuario.ForeColor = UITheme.Danger;
        this.lblErrorUsuario.Font = UITheme.Small;
        this.lblErrorUsuario.AutoSize = false;
        this.lblErrorUsuario.AutoEllipsis = true;
        this.lblErrorUsuario.Size = new Size(fieldW, 18);
        this.lblErrorUsuario.Location = new Point(baseX, 296);

        var lblPassword = new Label
        {
            Text = "CONTRASEÑA",
            Font = new Font(UITheme.FontFamilySemibold, 8.5f, FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            AutoSize = true,
            Location = new Point(baseX, 322)
        };
        var contPassword = new UITextBox { Location = new Point(baseX, 345), Size = new Size(fieldW, 46) };
        contPassword.PlaceholderText = "Ingrese su contraseña";
        contPassword.UseSystemPasswordChar = true;
        this.txtPassword = contPassword.Inner;

        this.chkMostrar.Text = "Mostrar contraseña";
        this.chkMostrar.Font = UITheme.Small;
        this.chkMostrar.ForeColor = UITheme.TextSecondary;
        this.chkMostrar.AutoSize = true;
        this.chkMostrar.Location = new Point(baseX, 398);
        this.chkMostrar.CheckedChanged += (s, e) => txtPassword.UseSystemPasswordChar = !chkMostrar.Checked;

        this.lblErrorPassword.Text = "";
        this.lblErrorPassword.ForeColor = UITheme.Danger;
        this.lblErrorPassword.Font = UITheme.Small;
        this.lblErrorPassword.AutoSize = false;
        this.lblErrorPassword.AutoEllipsis = true;
        this.lblErrorPassword.TextAlign = ContentAlignment.MiddleRight;
        this.lblErrorPassword.Size = new Size(220, 18);
        this.lblErrorPassword.Location = new Point(baseX + fieldW - 220, 398);

        this.btnLogin.Text = "Iniciar Sesión";
        this.btnLogin.Size = new Size(fieldW, 50);
        this.btnLogin.Location = new Point(baseX, 438);
        this.btnLogin.Click += BtnLogin_Click;

        this.lblCargando.Text = "";
        this.lblCargando.Font = UITheme.Small;
        this.lblCargando.ForeColor = UITheme.Primary;
        this.lblCargando.AutoSize = false;
        this.lblCargando.AutoEllipsis = true;
        this.lblCargando.TextAlign = ContentAlignment.MiddleCenter;
        this.lblCargando.Size = new Size(fieldW, 22);
        this.lblCargando.Location = new Point(baseX, 498);

        this.lblError.Text = "";
        this.lblError.ForeColor = UITheme.Danger;
        this.lblError.Font = UITheme.BodyBold;
        this.lblError.AutoSize = false;
        this.lblError.AutoEllipsis = true;
        this.lblError.TextAlign = ContentAlignment.MiddleCenter;
        this.lblError.Size = new Size(fieldW, 24);
        this.lblError.Location = new Point(baseX, 522);

        // Botón cerrar (X)
        var btnCerrar = new Label
        {
            Text = "✕",
            Font = new Font(UITheme.FontFamily, 13f, FontStyle.Regular),
            ForeColor = UITheme.TextMuted,
            AutoSize = false,
            Size = new Size(38, 34),
            Location = new Point(W - 46, 10),
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand
        };
        btnCerrar.Click += (s, e) => Application.Exit();
        btnCerrar.MouseEnter += (s, e) => btnCerrar.ForeColor = UITheme.Danger;
        btnCerrar.MouseLeave += (s, e) => btnCerrar.ForeColor = UITheme.TextMuted;

        this.Controls.AddRange(new Control[]
        {
            lblBienvenida, lblSubtitulo,
            lblUsuario, contUsuario, this.lblErrorUsuario,
            lblPassword, contPassword, this.chkMostrar, this.lblErrorPassword,
            this.btnLogin, this.lblCargando, this.lblError, btnCerrar
        });

        this.MouseDown += Login_MouseDown;
        this.AcceptButton = this.btnLogin;
        this.ResumeLayout(false);
        this.PerformLayout();

        this.Paint += (s, e) =>
        {
            using var pen = new Pen(UITheme.Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        };
    }

    private void Login_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            NativeMethods.ReleaseCapture();
            NativeMethods.SendMessage(this.Handle, 0xA1, 0x2, 0);
        }
    }
}

internal static class NativeMethods
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool ReleaseCapture();
}
