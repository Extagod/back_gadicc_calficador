using Panel_Admin.UI;

namespace Panel_Admin;

partial class DetalleFuncionarioForm
{
    private System.ComponentModel.IContainer components = null;
    private PictureBox picQR;
    private Label lblCedulaValor;
    private Label lblNombreValor;
    private Label lblApellidoValor;
    private Label lblCargoValor;
    private Label lblDireccionValor;
    private Label lblTokenValor;
    private UIButton btnRegenerarQR;
    private UIButton btnGuardarQR;
    private UIButton btnImprimirQR;
    private UIButton btnCerrar;
    private Label lblTitulo;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private Label CampoTitulo(string t, int x, int y) => new()
    {
        Text = t,
        Font = new Font(UITheme.FontFamilySemibold, 8f, FontStyle.Bold),
        ForeColor = UITheme.TextSecondary,
        AutoSize = true,
        Location = new Point(x, y)
    };
    private Label CampoValor(int x, int y, int w) => new()
    {
        Text = "—",
        Font = UITheme.BodyBold,
        ForeColor = UITheme.TextPrimary,
        AutoSize = false,
        Size = new Size(w, 22),
        Location = new Point(x, y + 16)
    };

    private void InitializeComponent()
    {
        this.picQR = new PictureBox();
        this.lblTitulo = new Label();
        this.btnRegenerarQR = new UIButton();
        this.btnGuardarQR = new UIButton();
        this.btnImprimirQR = new UIButton();
        this.btnCerrar = new UIButton();

        this.SuspendLayout();

        // Form
        this.ClientSize = new Size(760, 560);
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = UITheme.Background;

        var barra = new Panel { Dock = DockStyle.Top, Height = 6, BackColor = UITheme.Primary };
        this.Controls.Add(barra);

        this.lblTitulo.Text = "Detalle del Funcionario";
        this.lblTitulo.Font = UITheme.Title;
        this.lblTitulo.ForeColor = UITheme.TextPrimary;
        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Location = new Point(28, 26);
        this.Controls.Add(this.lblTitulo);

        var btnX = new Label
        {
            Text = "✕", Font = new Font(UITheme.FontFamily, 12f), ForeColor = UITheme.TextMuted,
            AutoSize = false, Size = new Size(34, 30), Location = new Point(712, 12),
            TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand
        };
        btnX.Click += BtnCerrar_Click;
        this.Controls.Add(btnX);

        // === Tarjeta info (izquierda) ===
        var cardInfo = new RoundedPanel
        {
            Location = new Point(28, 70),
            Size = new Size(360, 400),
            Radius = 12
        };
        int cx = 24, cyTop = 24, gap = 62;
        var lblSecInfo = new Label
        {
            Text = "INFORMACIÓN PERSONAL",
            Font = new Font(UITheme.FontFamilySemibold, 9f, FontStyle.Bold),
            ForeColor = UITheme.Primary, AutoSize = true, Location = new Point(cx, cyTop)
        };
        cardInfo.Controls.Add(lblSecInfo);

        cardInfo.Controls.Add(CampoTitulo("CÉDULA / RUC", cx, cyTop + 34));
        this.lblCedulaValor = CampoValor(cx, cyTop + 34, 300); this.lblCedulaValor.Font = new Font("Consolas", 10f, FontStyle.Bold);
        cardInfo.Controls.Add(this.lblCedulaValor);

        cardInfo.Controls.Add(CampoTitulo("NOMBRE", cx, cyTop + 34 + gap));
        this.lblNombreValor = CampoValor(cx, cyTop + 34 + gap, 300);
        cardInfo.Controls.Add(this.lblNombreValor);

        cardInfo.Controls.Add(CampoTitulo("APELLIDO", cx, cyTop + 34 + gap * 2));
        this.lblApellidoValor = CampoValor(cx, cyTop + 34 + gap * 2, 300);
        cardInfo.Controls.Add(this.lblApellidoValor);

        cardInfo.Controls.Add(CampoTitulo("CARGO", cx, cyTop + 34 + gap * 3));
        this.lblCargoValor = CampoValor(cx, cyTop + 34 + gap * 3, 300);
        cardInfo.Controls.Add(this.lblCargoValor);

        cardInfo.Controls.Add(CampoTitulo("DIRECCIÓN", cx, cyTop + 34 + gap * 4));
        this.lblDireccionValor = CampoValor(cx, cyTop + 34 + gap * 4, 300);
        cardInfo.Controls.Add(this.lblDireccionValor);

        this.Controls.Add(cardInfo);

        // === Tarjeta QR (derecha) ===
        var cardQR = new RoundedPanel
        {
            Location = new Point(404, 70),
            Size = new Size(328, 400),
            Radius = 12
        };
        var lblSecQR = new Label
        {
            Text = "CÓDIGO QR",
            Font = new Font(UITheme.FontFamilySemibold, 9f, FontStyle.Bold),
            ForeColor = UITheme.Primary, AutoSize = true, Location = new Point(24, 20)
        };
        cardQR.Controls.Add(lblSecQR);

        var panelQR = new Panel
        {
            Location = new Point(64, 50),
            Size = new Size(200, 200),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        this.picQR = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.White };
        panelQR.Controls.Add(this.picQR);
        cardQR.Controls.Add(panelQR);

        this.lblTokenValor = new Label
        {
            Text = "Sin generar",
            Font = new Font("Consolas", 8f),
            ForeColor = UITheme.TextSecondary,
            AutoSize = false,
            Size = new Size(280, 18),
            Location = new Point(24, 258),
            TextAlign = ContentAlignment.MiddleCenter
        };
        cardQR.Controls.Add(this.lblTokenValor);

        this.btnRegenerarQR.Text = "Generar QR";
        this.btnRegenerarQR.BaseColor = UITheme.Primary;
        this.btnRegenerarQR.Size = new Size(130, 38);
        this.btnRegenerarQR.Location = new Point(24, 288);
        this.btnRegenerarQR.Click += BtnRegenerarQR_Click;
        cardQR.Controls.Add(this.btnRegenerarQR);

        this.btnGuardarQR.Text = "Guardar";
        this.btnGuardarQR.Outline = true;
        this.btnGuardarQR.BaseColor = UITheme.Primary;
        this.btnGuardarQR.Size = new Size(150, 38);
        this.btnGuardarQR.Location = new Point(160, 288);
        this.btnGuardarQR.Click += BtnGuardarQR_Click;
        cardQR.Controls.Add(this.btnGuardarQR);

        this.btnImprimirQR.Text = "Imprimir QR";
        this.btnImprimirQR.Outline = true;
        this.btnImprimirQR.BaseColor = UITheme.Neutral;
        this.btnImprimirQR.Size = new Size(286, 38);
        this.btnImprimirQR.Location = new Point(24, 336);
        this.btnImprimirQR.Click += BtnImprimirQR_Click;
        cardQR.Controls.Add(this.btnImprimirQR);

        this.Controls.Add(cardQR);

        // Cerrar (abajo)
        this.btnCerrar.Text = "Cerrar";
        this.btnCerrar.Outline = true;
        this.btnCerrar.BaseColor = UITheme.Neutral;
        this.btnCerrar.Size = new Size(160, 42);
        this.btnCerrar.Location = new Point(572, 494);
        this.btnCerrar.Click += BtnCerrar_Click;
        this.Controls.Add(this.btnCerrar);

        this.AcceptButton = this.btnCerrar;
        this.CancelButton = this.btnCerrar;

        this.Paint += (s, e) =>
        {
            using var pen = new Pen(UITheme.Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        };

        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
