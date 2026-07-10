using Panel_Admin.UI;

namespace Panel_Admin;

partial class FuncionarioForm
{
    private System.ComponentModel.IContainer components = null;
    private UITextBox txtCedula;
    private UITextBox txtNombre;
    private UITextBox txtApellido;
    private UITextBox txtCargo;
    private UITextBox txtDireccion;
    private Label lblErrorCedula;
    private Label lblErrorNombre;
    private Label lblErrorApellido;
    private UIButton btnGuardar;
    private UIButton btnCancelar;
    private Label lblTitulo;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private Label Etiqueta(string texto, int x, int y) => new()
    {
        Text = texto,
        Font = new Font(UITheme.FontFamilySemibold, 8.5f, FontStyle.Bold),
        ForeColor = UITheme.TextSecondary,
        AutoSize = true,
        Location = new Point(x, y)
    };

    private Label Error(int x, int y) => new()
    {
        Text = "",
        Font = UITheme.Small,
        ForeColor = UITheme.Danger,
        AutoSize = true,
        Location = new Point(x, y)
    };

    private void InitializeComponent()
    {
        this.lblTitulo = new Label();
        this.lblErrorCedula = Error(40, 118);
        this.lblErrorNombre = Error(40, 190);
        this.lblErrorApellido = Error(40, 262);
        this.btnGuardar = new UIButton();
        this.btnCancelar = new UIButton();

        this.SuspendLayout();

        // Form
        this.ClientSize = new Size(480, 560);
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = UITheme.Card;

        // Barra superior de color
        var barra = new Panel { Dock = DockStyle.Top, Height = 6, BackColor = UITheme.Primary };
        this.Controls.Add(barra);

        // Título
        this.lblTitulo.Text = "Nuevo Funcionario";
        this.lblTitulo.Font = UITheme.Title;
        this.lblTitulo.ForeColor = UITheme.TextPrimary;
        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Location = new Point(40, 30);
        this.Controls.Add(this.lblTitulo);

        // Botón cerrar
        var btnX = new Label
        {
            Text = "✕",
            Font = new Font(UITheme.FontFamily, 12f),
            ForeColor = UITheme.TextMuted,
            AutoSize = false,
            Size = new Size(34, 30),
            Location = new Point(432, 12),
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand
        };
        btnX.Click += BtnCancelar_Click;
        this.Controls.Add(btnX);

        int x = 40, w = 400;

        // Cédula
        this.Controls.Add(Etiqueta("CÉDULA / RUC *", x, 74));
        this.txtCedula = new UITextBox { Location = new Point(x, 92), Size = new Size(w, 42), PlaceholderText = "Ej: 0102030405" };
        this.txtCedula.Inner.MaxLength = 14;
        this.Controls.Add(this.txtCedula);
        this.Controls.Add(this.lblErrorCedula);

        // Nombre
        this.Controls.Add(Etiqueta("NOMBRE *", x, 146));
        this.txtNombre = new UITextBox { Location = new Point(x, 164), Size = new Size(w, 42), PlaceholderText = "Primer nombre" };
        this.txtNombre.Inner.MaxLength = 100;
        this.Controls.Add(this.txtNombre);
        this.Controls.Add(this.lblErrorNombre);

        // Apellido
        this.Controls.Add(Etiqueta("APELLIDO *", x, 218));
        this.txtApellido = new UITextBox { Location = new Point(x, 236), Size = new Size(w, 42), PlaceholderText = "Primer apellido" };
        this.txtApellido.Inner.MaxLength = 100;
        this.Controls.Add(this.txtApellido);
        this.Controls.Add(this.lblErrorApellido);

        // Cargo
        this.Controls.Add(Etiqueta("CARGO", x, 290));
        this.txtCargo = new UITextBox { Location = new Point(x, 308), Size = new Size(w, 42), PlaceholderText = "Cargo que desempeña" };
        this.txtCargo.Inner.MaxLength = 100;
        this.Controls.Add(this.txtCargo);

        // Dirección
        this.Controls.Add(Etiqueta("DIRECCIÓN", x, 362));
        this.txtDireccion = new UITextBox { Location = new Point(x, 380), Size = new Size(w, 42), PlaceholderText = "Dirección de domicilio" };
        this.txtDireccion.Inner.MaxLength = 300;
        this.Controls.Add(this.txtDireccion);

        // Botones
        this.btnCancelar.Text = "Cancelar";
        this.btnCancelar.Outline = true;
        this.btnCancelar.BaseColor = UITheme.Neutral;
        this.btnCancelar.Size = new Size(150, 44);
        this.btnCancelar.Location = new Point(x, 470);
        this.btnCancelar.Click += BtnCancelar_Click;
        this.Controls.Add(this.btnCancelar);

        this.btnGuardar.Text = "Guardar";
        this.btnGuardar.BaseColor = UITheme.Primary;
        this.btnGuardar.Size = new Size(200, 44);
        this.btnGuardar.Location = new Point(x + 160, 470);
        this.btnGuardar.Click += BtnGuardar_Click;
        this.Controls.Add(this.btnGuardar);

        this.AcceptButton = this.btnGuardar;
        this.CancelButton = this.btnCancelar;

        this.Paint += (s, e) =>
        {
            using var pen = new Pen(UITheme.Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        };

        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
