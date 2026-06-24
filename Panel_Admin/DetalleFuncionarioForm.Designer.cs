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
    private Button btnRegenerarQR;
    private Button btnGuardarQR;
    private Button btnImprimirQR;
    private Button btnCerrar;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.picQR = new PictureBox();
        this.lblCedulaValor = new Label();
        this.lblNombreValor = new Label();
        this.lblApellidoValor = new Label();
        this.lblCargoValor = new Label();
        this.lblDireccionValor = new Label();
        this.lblTokenValor = new Label();
        this.btnRegenerarQR = new Button();
        this.btnGuardarQR = new Button();
        this.btnImprimirQR = new Button();
        this.btnCerrar = new Button();

        var lblCedula = new Label();
        var lblNombre = new Label();
        var lblApellido = new Label();
        var lblCargo = new Label();
        var lblDireccion = new Label();
        var lblToken = new Label();
        var lblTitulo = new Label();

        ((System.ComponentModel.ISupportInitialize)this.picQR).BeginInit();
        this.SuspendLayout();

        int infoX = 20, valX = 120;

        // Título
        lblTitulo.Text = "Detalle del Funcionario";
        lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblTitulo.Location = new Point(infoX, 15);
        lblTitulo.AutoSize = true;

        // Info labels
        lblCedula.Text = "Cédula:";
        lblCedula.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCedula.Location = new Point(infoX, 55);
        lblCedula.AutoSize = true;
        this.lblCedulaValor.Location = new Point(valX, 55);
        this.lblCedulaValor.AutoSize = true;
        this.lblCedulaValor.Font = new Font("Consolas", 9F);

        lblNombre.Text = "Nombre:";
        lblNombre.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNombre.Location = new Point(infoX, 80);
        lblNombre.AutoSize = true;
        this.lblNombreValor.Location = new Point(valX, 80);
        this.lblNombreValor.AutoSize = true;

        lblApellido.Text = "Apellido:";
        lblApellido.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblApellido.Location = new Point(infoX, 105);
        lblApellido.AutoSize = true;
        this.lblApellidoValor.Location = new Point(valX, 105);
        this.lblApellidoValor.AutoSize = true;

        lblCargo.Text = "Cargo:";
        lblCargo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCargo.Location = new Point(infoX, 130);
        lblCargo.AutoSize = true;
        this.lblCargoValor.Location = new Point(valX, 130);
        this.lblCargoValor.AutoSize = true;

        lblDireccion.Text = "Dirección:";
        lblDireccion.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDireccion.Location = new Point(infoX, 155);
        lblDireccion.AutoSize = true;
        this.lblDireccionValor.Location = new Point(valX, 155);
        this.lblDireccionValor.AutoSize = true;

        lblToken.Text = "Token QR:";
        lblToken.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblToken.Location = new Point(infoX, 180);
        lblToken.AutoSize = true;
        this.lblTokenValor.Location = new Point(valX, 180);
        this.lblTokenValor.AutoSize = true;
        this.lblTokenValor.Font = new Font("Consolas", 8.5F);

        // QR Image
        this.picQR.Location = new Point(infoX, 210);
        this.picQR.Size = new Size(250, 250);
        this.picQR.BorderStyle = BorderStyle.FixedSingle;
        this.picQR.SizeMode = PictureBoxSizeMode.Zoom;

        // Buttons - right of QR
        int btnX = 290;

        this.btnRegenerarQR.Text = "Generar QR";
        this.btnRegenerarQR.Location = new Point(btnX, 210);
        this.btnRegenerarQR.Size = new Size(150, 35);
        this.btnRegenerarQR.Click += BtnRegenerarQR_Click;

        this.btnGuardarQR.Text = "Guardar QR";
        this.btnGuardarQR.Location = new Point(btnX, 260);
        this.btnGuardarQR.Size = new Size(150, 35);
        this.btnGuardarQR.Click += BtnGuardarQR_Click;

        this.btnImprimirQR.Text = "Imprimir QR";
        this.btnImprimirQR.Location = new Point(btnX, 310);
        this.btnImprimirQR.Size = new Size(150, 35);
        this.btnImprimirQR.Click += BtnImprimirQR_Click;

        this.btnCerrar.Text = "Cerrar";
        this.btnCerrar.Location = new Point(btnX, 420);
        this.btnCerrar.Size = new Size(150, 35);
        this.btnCerrar.Click += BtnCerrar_Click;

        // Form
        this.ClientSize = new Size(470, 480);
        this.Controls.AddRange(new Control[]
        {
            lblTitulo,
            lblCedula, this.lblCedulaValor,
            lblNombre, this.lblNombreValor,
            lblApellido, this.lblApellidoValor,
            lblCargo, this.lblCargoValor,
            lblDireccion, this.lblDireccionValor,
            lblToken, this.lblTokenValor,
            this.picQR,
            this.btnRegenerarQR, this.btnGuardarQR, this.btnImprimirQR, this.btnCerrar
        });
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Detalle del Funcionario";

        ((System.ComponentModel.ISupportInitialize)this.picQR).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
