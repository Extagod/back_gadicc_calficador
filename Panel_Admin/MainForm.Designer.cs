using Panel_Admin.UI;

namespace Panel_Admin;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private Panel sidebar;
    private Panel header;
    private Panel content;
    private Label lblLogo;
    private Label lblLogoSub;
    private SidebarButton navDashboard;
    private SidebarButton navFuncionarios;
    private SidebarButton navCalificaciones;
    private SidebarButton navReportes;
    private SidebarButton navCerrarSesion;
    private Label lblSeccion;
    private Label lblSeccionDesc;
    private Label lblFecha;
    private Label lblUsuario;
    private Label lblAvatar;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.sidebar = new Panel();
        this.header = new Panel();
        this.content = new Panel();
        this.lblLogo = new Label();
        this.lblLogoSub = new Label();
        this.navDashboard = new SidebarButton();
        this.navFuncionarios = new SidebarButton();
        this.navCalificaciones = new SidebarButton();
        this.navReportes = new SidebarButton();
        this.navCerrarSesion = new SidebarButton();
        this.lblSeccion = new Label();
        this.lblSeccionDesc = new Label();
        this.lblFecha = new Label();
        this.lblUsuario = new Label();
        this.lblAvatar = new Label();

        this.SuspendLayout();

        // === Form ===
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1280, 760);
        this.MinimumSize = new Size(1120, 680);
        this.BackColor = UITheme.Background;
        this.Text = "GADICC Calificador — Panel Administrativo";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.WindowState = FormWindowState.Maximized;
        this.Font = UITheme.Body;

        // === Sidebar ===
        this.sidebar.Dock = DockStyle.Left;
        this.sidebar.Width = 250;
        this.sidebar.BackColor = UITheme.Sidebar;

        // Cabecera del sidebar (logo)
        var brand = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = UITheme.Sidebar };
        var logoImg = LogoHelper.Logo;
        if (logoImg != null)
        {
            var pic = new PictureBox
            {
                Image = logoImg,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Size = new Size(210, 78),
                Location = new Point(20, 16)
            };
            brand.Controls.Add(pic);
        }
        else
        {
            this.lblLogo.Text = "CAÑAR";
            this.lblLogo.Font = new Font(UITheme.FontFamilySemibold, 20f, FontStyle.Bold);
            this.lblLogo.ForeColor = Color.White;
            this.lblLogo.AutoSize = false;
            this.lblLogo.AutoEllipsis = true;
            this.lblLogo.Location = new Point(20, 28);
            this.lblLogo.Size = new Size(210, 32);
            this.lblLogoSub.Text = "MUNICIPIO INTERCULTURAL";
            this.lblLogoSub.Font = new Font(UITheme.FontFamily, 8f, FontStyle.Regular);
            this.lblLogoSub.ForeColor = Color.FromArgb(150, 175, 210);
            this.lblLogoSub.AutoSize = false;
            this.lblLogoSub.AutoEllipsis = true;
            this.lblLogoSub.Location = new Point(22, 62);
            this.lblLogoSub.Size = new Size(210, 18);
            brand.Controls.Add(this.lblLogoSub);
            brand.Controls.Add(this.lblLogo);
        }

        // Botones de navegación (se agregan en orden inverso por Dock=Top)
        this.navReportes.Text = "Reportes";
        this.navReportes.Icono = "📄";
        this.navReportes.Click += (s, e) => Navegar("reportes");

        this.navCalificaciones.Text = "Calificaciones";
        this.navCalificaciones.Icono = "⭐";
        this.navCalificaciones.Click += (s, e) => Navegar("calificaciones");

        this.navFuncionarios.Text = "Funcionarios";
        this.navFuncionarios.Icono = "👥";
        this.navFuncionarios.Click += (s, e) => Navegar("funcionarios");

        this.navDashboard.Text = "Dashboard";
        this.navDashboard.Icono = "📊";
        this.navDashboard.Click += (s, e) => Navegar("dashboard");

        // Separador superior de la lista
        var navTop = new Panel { Dock = DockStyle.Top, Height = 12, BackColor = UITheme.Sidebar };

        // Cerrar sesión (abajo)
        this.navCerrarSesion.Text = "Cerrar sesión";
        this.navCerrarSesion.Icono = "⏻";
        this.navCerrarSesion.Dock = DockStyle.Bottom;
        this.navCerrarSesion.Click += (s, e) => CerrarSesion();

        // Orden de adición: primero los Top en orden inverso al visual
        this.sidebar.Controls.Add(this.navReportes);
        this.sidebar.Controls.Add(this.navCalificaciones);
        this.sidebar.Controls.Add(this.navFuncionarios);
        this.sidebar.Controls.Add(this.navDashboard);
        this.sidebar.Controls.Add(navTop);
        this.sidebar.Controls.Add(brand);
        this.sidebar.Controls.Add(this.navCerrarSesion);

        // === Header ===
        this.header.Dock = DockStyle.Top;
        this.header.Height = 78;
        this.header.BackColor = UITheme.HeaderBar;
        this.header.Paint += (s, e) =>
        {
            using var pen = new Pen(UITheme.Border, 1);
            e.Graphics.DrawLine(pen, 0, this.header.Height - 1, this.header.Width, this.header.Height - 1);
        };

        this.lblSeccion.Text = "Dashboard";
        this.lblSeccion.Font = new Font(UITheme.FontFamilySemibold, 16f, FontStyle.Bold);
        this.lblSeccion.ForeColor = UITheme.TextPrimary;
        this.lblSeccion.AutoSize = true;
        this.lblSeccion.Location = new Point(28, 16);

        this.lblSeccionDesc.Text = "";
        this.lblSeccionDesc.Font = UITheme.Small;
        this.lblSeccionDesc.ForeColor = UITheme.TextSecondary;
        this.lblSeccionDesc.AutoSize = true;
        this.lblSeccionDesc.Location = new Point(30, 48);

        this.lblFecha.Text = "";
        this.lblFecha.Font = UITheme.Body;
        this.lblFecha.ForeColor = UITheme.TextSecondary;
        this.lblFecha.AutoSize = false;
        this.lblFecha.AutoEllipsis = true;
        this.lblFecha.TextAlign = ContentAlignment.MiddleRight;
        this.lblFecha.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.lblFecha.Size = new Size(300, 20);
        this.lblFecha.Location = new Point(this.ClientSize.Width - 440, 20);

        this.lblUsuario.Text = "";
        this.lblUsuario.Font = UITheme.BodyBold;
        this.lblUsuario.ForeColor = UITheme.TextPrimary;
        this.lblUsuario.AutoSize = false;
        this.lblUsuario.AutoEllipsis = true;
        this.lblUsuario.TextAlign = ContentAlignment.MiddleRight;
        this.lblUsuario.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.lblUsuario.Size = new Size(240, 20);
        this.lblUsuario.Location = new Point(this.ClientSize.Width - 440, 42);

        this.lblAvatar.Text = "A";
        this.lblAvatar.Font = new Font(UITheme.FontFamilySemibold, 14f, FontStyle.Bold);
        this.lblAvatar.ForeColor = Color.White;
        this.lblAvatar.BackColor = UITheme.Primary;
        this.lblAvatar.AutoSize = false;
        this.lblAvatar.TextAlign = ContentAlignment.MiddleCenter;
        this.lblAvatar.Size = new Size(44, 44);
        this.lblAvatar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.lblAvatar.Location = new Point(this.ClientSize.Width - 130, 18);

        this.header.Controls.AddRange(new Control[]
        {
            this.lblSeccion, this.lblSeccionDesc, this.lblFecha, this.lblUsuario, this.lblAvatar
        });

        // === Content ===
        this.content.Dock = DockStyle.Fill;
        this.content.BackColor = UITheme.Background;
        this.content.Padding = new Padding(24);

        // Orden de adición al form: content (fill) primero, luego header (top), luego sidebar (left)
        this.Controls.Add(this.content);
        this.Controls.Add(this.header);
        this.Controls.Add(this.sidebar);

        this.ResumeLayout(false);
    }
}
