namespace Panel_Admin;

partial class FuncionarioForm
{
    private System.ComponentModel.IContainer components = null;
    private TextBox txtCedula;
    private TextBox txtNombre;
    private TextBox txtApellido;
    private TextBox txtCargo;
    private TextBox txtDireccion;
    private Label lblErrorCedula;
    private Label lblErrorNombre;
    private Label lblErrorApellido;
    private Button btnGuardar;
    private Button btnCancelar;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.txtCedula = new TextBox();
        this.txtNombre = new TextBox();
        this.txtApellido = new TextBox();
        this.txtCargo = new TextBox();
        this.txtDireccion = new TextBox();
        this.lblErrorCedula = new Label();
        this.lblErrorNombre = new Label();
        this.lblErrorApellido = new Label();
        this.btnGuardar = new Button();
        this.btnCancelar = new Button();

        var lblCedula = new Label();
        var lblNombre = new Label();
        var lblApellido = new Label();
        var lblCargo = new Label();
        var lblDireccion = new Label();

        this.SuspendLayout();

        int x = 30, fieldW = 300;

        // Cédula
        lblCedula.Text = "Cédula/RUC *:";
        lblCedula.Location = new Point(x, 20);
        lblCedula.AutoSize = true;

        this.txtCedula.Location = new Point(x, 43);
        this.txtCedula.Size = new Size(fieldW, 23);
        this.txtCedula.MaxLength = 14;

        this.lblErrorCedula.Location = new Point(x, 68);
        this.lblErrorCedula.Size = new Size(fieldW, 18);
        this.lblErrorCedula.ForeColor = Color.Red;
        this.lblErrorCedula.Font = new Font("Segoe UI", 8F);
        this.lblErrorCedula.Text = "";

        // Nombre
        lblNombre.Text = "Nombre *:";
        lblNombre.Location = new Point(x, 92);
        lblNombre.AutoSize = true;

        this.txtNombre.Location = new Point(x, 115);
        this.txtNombre.Size = new Size(fieldW, 23);
        this.txtNombre.MaxLength = 100;

        this.lblErrorNombre.Location = new Point(x, 140);
        this.lblErrorNombre.Size = new Size(fieldW, 18);
        this.lblErrorNombre.ForeColor = Color.Red;
        this.lblErrorNombre.Font = new Font("Segoe UI", 8F);
        this.lblErrorNombre.Text = "";

        // Apellido
        lblApellido.Text = "Apellido *:";
        lblApellido.Location = new Point(x, 164);
        lblApellido.AutoSize = true;

        this.txtApellido.Location = new Point(x, 187);
        this.txtApellido.Size = new Size(fieldW, 23);
        this.txtApellido.MaxLength = 100;

        this.lblErrorApellido.Location = new Point(x, 212);
        this.lblErrorApellido.Size = new Size(fieldW, 18);
        this.lblErrorApellido.ForeColor = Color.Red;
        this.lblErrorApellido.Font = new Font("Segoe UI", 8F);
        this.lblErrorApellido.Text = "";

        // Cargo
        lblCargo.Text = "Cargo:";
        lblCargo.Location = new Point(x, 236);
        lblCargo.AutoSize = true;

        this.txtCargo.Location = new Point(x, 259);
        this.txtCargo.Size = new Size(fieldW, 23);
        this.txtCargo.MaxLength = 100;

        // Dirección
        lblDireccion.Text = "Dirección:";
        lblDireccion.Location = new Point(x, 292);
        lblDireccion.AutoSize = true;

        this.txtDireccion.Location = new Point(x, 315);
        this.txtDireccion.Size = new Size(fieldW, 23);
        this.txtDireccion.MaxLength = 300;

        // Botones
        this.btnGuardar.Text = "Guardar";
        this.btnGuardar.Location = new Point(x, 360);
        this.btnGuardar.Size = new Size(120, 35);
        this.btnGuardar.Click += BtnGuardar_Click;

        this.btnCancelar.Text = "Cancelar";
        this.btnCancelar.Location = new Point(x + 140, 360);
        this.btnCancelar.Size = new Size(120, 35);
        this.btnCancelar.Click += BtnCancelar_Click;

        // Form
        this.ClientSize = new Size(370, 420);
        this.Controls.AddRange(new Control[]
        {
            lblCedula, this.txtCedula, this.lblErrorCedula,
            lblNombre, this.txtNombre, this.lblErrorNombre,
            lblApellido, this.txtApellido, this.lblErrorApellido,
            lblCargo, this.txtCargo,
            lblDireccion, this.txtDireccion,
            this.btnGuardar, this.btnCancelar
        });
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
