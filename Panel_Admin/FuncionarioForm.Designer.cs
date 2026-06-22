namespace Panel_Admin;

partial class FuncionarioForm
{
    private System.ComponentModel.IContainer components = null;
    private TextBox txtNombre;
    private TextBox txtApellido;
    private TextBox txtCargo;
    private TextBox txtDireccion;
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
        this.txtNombre = new TextBox();
        this.txtApellido = new TextBox();
        this.txtCargo = new TextBox();
        this.txtDireccion = new TextBox();
        this.lblErrorNombre = new Label();
        this.lblErrorApellido = new Label();
        this.btnGuardar = new Button();
        this.btnCancelar = new Button();

        var lblNombre = new Label();
        var lblApellido = new Label();
        var lblCargo = new Label();
        var lblDireccion = new Label();

        this.SuspendLayout();

        int x = 30, fieldW = 300;

        // Nombre
        lblNombre.Text = "Nombre *:";
        lblNombre.Location = new Point(x, 20);
        lblNombre.AutoSize = true;

        this.txtNombre.Location = new Point(x, 43);
        this.txtNombre.Size = new Size(fieldW, 23);
        this.txtNombre.MaxLength = 100;

        this.lblErrorNombre.Location = new Point(x, 68);
        this.lblErrorNombre.Size = new Size(fieldW, 18);
        this.lblErrorNombre.ForeColor = Color.Red;
        this.lblErrorNombre.Font = new Font("Segoe UI", 8F);
        this.lblErrorNombre.Text = "";

        // Apellido
        lblApellido.Text = "Apellido *:";
        lblApellido.Location = new Point(x, 92);
        lblApellido.AutoSize = true;

        this.txtApellido.Location = new Point(x, 115);
        this.txtApellido.Size = new Size(fieldW, 23);
        this.txtApellido.MaxLength = 100;

        this.lblErrorApellido.Location = new Point(x, 140);
        this.lblErrorApellido.Size = new Size(fieldW, 18);
        this.lblErrorApellido.ForeColor = Color.Red;
        this.lblErrorApellido.Font = new Font("Segoe UI", 8F);
        this.lblErrorApellido.Text = "";

        // Cargo
        lblCargo.Text = "Cargo:";
        lblCargo.Location = new Point(x, 164);
        lblCargo.AutoSize = true;

        this.txtCargo.Location = new Point(x, 187);
        this.txtCargo.Size = new Size(fieldW, 23);
        this.txtCargo.MaxLength = 100;

        // Dirección
        lblDireccion.Text = "Dirección:";
        lblDireccion.Location = new Point(x, 220);
        lblDireccion.AutoSize = true;

        this.txtDireccion.Location = new Point(x, 243);
        this.txtDireccion.Size = new Size(fieldW, 23);
        this.txtDireccion.MaxLength = 200;

        // Botones
        this.btnGuardar.Text = "Guardar";
        this.btnGuardar.Location = new Point(x, 290);
        this.btnGuardar.Size = new Size(120, 35);
        this.btnGuardar.Click += BtnGuardar_Click;

        this.btnCancelar.Text = "Cancelar";
        this.btnCancelar.Location = new Point(x + 140, 290);
        this.btnCancelar.Size = new Size(120, 35);
        this.btnCancelar.Click += BtnCancelar_Click;

        // Form
        this.ClientSize = new Size(370, 350);
        this.Controls.AddRange(new Control[]
        {
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
