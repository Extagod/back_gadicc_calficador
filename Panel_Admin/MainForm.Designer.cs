namespace Panel_Admin;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.tabControl = new TabControl();
        this.tabFuncionarios = new TabPage();
        this.tabQR = new TabPage();
        this.tabCalificaciones = new TabPage();

        // === Tab Funcionarios Controls ===
        this.dgvFuncionarios = new DataGridView();
        this.txtBusqueda = new TextBox();
        this.txtNombre = new TextBox();
        this.txtApellido = new TextBox();
        this.txtCargo = new TextBox();
        this.txtDireccion = new TextBox();
        this.lblValidacionNombre = new Label();
        this.lblValidacionApellido = new Label();
        this.btnNuevoFuncionario = new Button();
        this.btnEditarFuncionario = new Button();

        // === Tab QR Controls ===
        this.picQR = new PictureBox();
        this.btnGenerarQR = new Button();
        this.btnGuardarQR = new Button();
        this.btnImprimirQR = new Button();

        // === Tab Calificaciones Controls ===
        this.dgvCalificaciones = new DataGridView();
        this.lblNoCalificaciones = new Label();
        this.lblTotal = new Label();
        this.lblExcelente = new Label();
        this.lblBuena = new Label();
        this.lblRegular = new Label();
        this.lblMala = new Label();
        this.dtpFechaInicio = new DateTimePicker();
        this.dtpFechaFin = new DateTimePicker();
        this.btnFiltrarFechas = new Button();

        // Labels for form
        var lblBusqueda = new Label();
        var lblNombreLabel = new Label();
        var lblApellidoLabel = new Label();
        var lblCargoLabel = new Label();
        var lblDireccionLabel = new Label();
        var lblFechaInicioLabel = new Label();
        var lblFechaFinLabel = new Label();

        ((System.ComponentModel.ISupportInitialize)this.dgvFuncionarios).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.dgvCalificaciones).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.picQR).BeginInit();
        this.SuspendLayout();

        // =============================================
        // tabControl
        // =============================================
        this.tabControl.Dock = DockStyle.Fill;
        this.tabControl.Location = new Point(0, 0);
        this.tabControl.Name = "tabControl";
        this.tabControl.SelectedIndex = 0;
        this.tabControl.Size = new Size(1000, 650);
        this.tabControl.TabPages.AddRange(new TabPage[] { this.tabFuncionarios, this.tabQR, this.tabCalificaciones });
        this.tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

        // =============================================
        // tabFuncionarios
        // =============================================
        this.tabFuncionarios.Name = "tabFuncionarios";
        this.tabFuncionarios.Text = "Funcionarios";
        this.tabFuncionarios.Padding = new Padding(10);
        this.tabFuncionarios.Controls.AddRange(new Control[]
        {
            lblBusqueda, this.txtBusqueda, this.dgvFuncionarios,
            lblNombreLabel, this.txtNombre, this.lblValidacionNombre,
            lblApellidoLabel, this.txtApellido, this.lblValidacionApellido,
            lblCargoLabel, this.txtCargo,
            lblDireccionLabel, this.txtDireccion,
            this.btnNuevoFuncionario, this.btnEditarFuncionario
        });

        // lblBusqueda
        lblBusqueda.Text = "Buscar:";
        lblBusqueda.Location = new Point(15, 15);
        lblBusqueda.Size = new Size(50, 23);

        // txtBusqueda
        this.txtBusqueda.Name = "txtBusqueda";
        this.txtBusqueda.Location = new Point(70, 12);
        this.txtBusqueda.Size = new Size(250, 23);
        this.txtBusqueda.PlaceholderText = "Buscar por nombre o apellido...";
        this.txtBusqueda.TextChanged += TxtBusqueda_TextChanged;

        // dgvFuncionarios
        this.dgvFuncionarios.Name = "dgvFuncionarios";
        this.dgvFuncionarios.Location = new Point(15, 45);
        this.dgvFuncionarios.Size = new Size(550, 530);
        this.dgvFuncionarios.AllowUserToAddRows = false;
        this.dgvFuncionarios.AllowUserToDeleteRows = false;
        this.dgvFuncionarios.ReadOnly = true;
        this.dgvFuncionarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvFuncionarios.MultiSelect = false;
        this.dgvFuncionarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvFuncionarios.SelectionChanged += DgvFuncionarios_SelectionChanged;

        // Form fields - Right panel
        int rightX = 590;
        int fieldWidth = 370;

        // lblNombreLabel
        lblNombreLabel.Text = "Nombre *:";
        lblNombreLabel.Location = new Point(rightX, 50);
        lblNombreLabel.Size = new Size(80, 23);

        // txtNombre
        this.txtNombre.Name = "txtNombre";
        this.txtNombre.Location = new Point(rightX, 73);
        this.txtNombre.Size = new Size(fieldWidth, 23);

        // lblValidacionNombre
        this.lblValidacionNombre.Name = "lblValidacionNombre";
        this.lblValidacionNombre.Location = new Point(rightX, 98);
        this.lblValidacionNombre.Size = new Size(fieldWidth, 20);
        this.lblValidacionNombre.ForeColor = Color.Red;
        this.lblValidacionNombre.Font = new Font("Segoe UI", 8F);
        this.lblValidacionNombre.Text = "";

        // lblApellidoLabel
        lblApellidoLabel.Text = "Apellido *:";
        lblApellidoLabel.Location = new Point(rightX, 125);
        lblApellidoLabel.Size = new Size(80, 23);

        // txtApellido
        this.txtApellido.Name = "txtApellido";
        this.txtApellido.Location = new Point(rightX, 148);
        this.txtApellido.Size = new Size(fieldWidth, 23);

        // lblValidacionApellido
        this.lblValidacionApellido.Name = "lblValidacionApellido";
        this.lblValidacionApellido.Location = new Point(rightX, 173);
        this.lblValidacionApellido.Size = new Size(fieldWidth, 20);
        this.lblValidacionApellido.ForeColor = Color.Red;
        this.lblValidacionApellido.Font = new Font("Segoe UI", 8F);
        this.lblValidacionApellido.Text = "";

        // lblCargoLabel
        lblCargoLabel.Text = "Cargo:";
        lblCargoLabel.Location = new Point(rightX, 200);
        lblCargoLabel.Size = new Size(80, 23);

        // txtCargo
        this.txtCargo.Name = "txtCargo";
        this.txtCargo.Location = new Point(rightX, 223);
        this.txtCargo.Size = new Size(fieldWidth, 23);

        // lblDireccionLabel
        lblDireccionLabel.Text = "Dirección:";
        lblDireccionLabel.Location = new Point(rightX, 255);
        lblDireccionLabel.Size = new Size(80, 23);

        // txtDireccion
        this.txtDireccion.Name = "txtDireccion";
        this.txtDireccion.Location = new Point(rightX, 278);
        this.txtDireccion.Size = new Size(fieldWidth, 23);

        // btnNuevoFuncionario
        this.btnNuevoFuncionario.Name = "btnNuevoFuncionario";
        this.btnNuevoFuncionario.Text = "Nuevo";
        this.btnNuevoFuncionario.Location = new Point(rightX, 320);
        this.btnNuevoFuncionario.Size = new Size(120, 35);
        this.btnNuevoFuncionario.Click += BtnNuevoFuncionario_Click;

        // btnEditarFuncionario
        this.btnEditarFuncionario.Name = "btnEditarFuncionario";
        this.btnEditarFuncionario.Text = "Editar";
        this.btnEditarFuncionario.Location = new Point(rightX + 140, 320);
        this.btnEditarFuncionario.Size = new Size(120, 35);
        this.btnEditarFuncionario.Click += BtnEditarFuncionario_Click;

        // =============================================
        // tabQR
        // =============================================
        this.tabQR.Name = "tabQR";
        this.tabQR.Text = "Códigos QR";
        this.tabQR.Padding = new Padding(10);
        this.tabQR.Controls.AddRange(new Control[]
        {
            this.picQR, this.btnGenerarQR, this.btnGuardarQR, this.btnImprimirQR
        });

        // picQR
        this.picQR.Name = "picQR";
        this.picQR.Location = new Point(20, 20);
        this.picQR.Size = new Size(300, 300);
        this.picQR.BorderStyle = BorderStyle.FixedSingle;
        this.picQR.SizeMode = PictureBoxSizeMode.Zoom;

        // btnGenerarQR
        this.btnGenerarQR.Name = "btnGenerarQR";
        this.btnGenerarQR.Text = "Generar QR";
        this.btnGenerarQR.Location = new Point(350, 20);
        this.btnGenerarQR.Size = new Size(150, 35);
        this.btnGenerarQR.Click += BtnGenerarQR_Click;

        // btnGuardarQR
        this.btnGuardarQR.Name = "btnGuardarQR";
        this.btnGuardarQR.Text = "Guardar QR";
        this.btnGuardarQR.Location = new Point(350, 70);
        this.btnGuardarQR.Size = new Size(150, 35);
        this.btnGuardarQR.Click += BtnGuardarQR_Click;

        // btnImprimirQR
        this.btnImprimirQR.Name = "btnImprimirQR";
        this.btnImprimirQR.Text = "Imprimir QR";
        this.btnImprimirQR.Location = new Point(350, 120);
        this.btnImprimirQR.Size = new Size(150, 35);
        this.btnImprimirQR.Click += BtnImprimirQR_Click;

        // =============================================
        // tabCalificaciones
        // =============================================
        this.tabCalificaciones.Name = "tabCalificaciones";
        this.tabCalificaciones.Text = "Calificaciones";
        this.tabCalificaciones.Padding = new Padding(10);
        this.tabCalificaciones.Controls.AddRange(new Control[]
        {
            this.dgvCalificaciones, this.lblNoCalificaciones,
            this.lblTotal, this.lblExcelente, this.lblBuena, this.lblRegular, this.lblMala,
            lblFechaInicioLabel, this.dtpFechaInicio,
            lblFechaFinLabel, this.dtpFechaFin,
            this.btnFiltrarFechas
        });

        // dgvCalificaciones
        this.dgvCalificaciones.Name = "dgvCalificaciones";
        this.dgvCalificaciones.Location = new Point(15, 15);
        this.dgvCalificaciones.Size = new Size(600, 400);
        this.dgvCalificaciones.AllowUserToAddRows = false;
        this.dgvCalificaciones.AllowUserToDeleteRows = false;
        this.dgvCalificaciones.ReadOnly = true;
        this.dgvCalificaciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvCalificaciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        // lblNoCalificaciones
        this.lblNoCalificaciones.Name = "lblNoCalificaciones";
        this.lblNoCalificaciones.Text = "No hay calificaciones para este funcionario.";
        this.lblNoCalificaciones.Location = new Point(15, 200);
        this.lblNoCalificaciones.Size = new Size(400, 25);
        this.lblNoCalificaciones.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
        this.lblNoCalificaciones.Visible = false;

        // Statistics panel - right side
        int statsX = 640;

        // lblTotal
        this.lblTotal.Name = "lblTotal";
        this.lblTotal.Text = "Total: 0";
        this.lblTotal.Location = new Point(statsX, 20);
        this.lblTotal.Size = new Size(200, 25);
        this.lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

        // lblExcelente
        this.lblExcelente.Name = "lblExcelente";
        this.lblExcelente.Text = "Excelente: 0";
        this.lblExcelente.Location = new Point(statsX, 55);
        this.lblExcelente.Size = new Size(200, 22);

        // lblBuena
        this.lblBuena.Name = "lblBuena";
        this.lblBuena.Text = "Buena: 0";
        this.lblBuena.Location = new Point(statsX, 80);
        this.lblBuena.Size = new Size(200, 22);

        // lblRegular
        this.lblRegular.Name = "lblRegular";
        this.lblRegular.Text = "Regular: 0";
        this.lblRegular.Location = new Point(statsX, 105);
        this.lblRegular.Size = new Size(200, 22);

        // lblMala
        this.lblMala.Name = "lblMala";
        this.lblMala.Text = "Mala: 0";
        this.lblMala.Location = new Point(statsX, 130);
        this.lblMala.Size = new Size(200, 22);

        // Date filter section
        // lblFechaInicioLabel
        lblFechaInicioLabel.Text = "Desde:";
        lblFechaInicioLabel.Location = new Point(statsX, 180);
        lblFechaInicioLabel.Size = new Size(50, 23);

        // dtpFechaInicio
        this.dtpFechaInicio.Name = "dtpFechaInicio";
        this.dtpFechaInicio.Location = new Point(statsX, 203);
        this.dtpFechaInicio.Size = new Size(200, 23);
        this.dtpFechaInicio.Format = DateTimePickerFormat.Short;

        // lblFechaFinLabel
        lblFechaFinLabel.Text = "Hasta:";
        lblFechaFinLabel.Location = new Point(statsX, 235);
        lblFechaFinLabel.Size = new Size(50, 23);

        // dtpFechaFin
        this.dtpFechaFin.Name = "dtpFechaFin";
        this.dtpFechaFin.Location = new Point(statsX, 258);
        this.dtpFechaFin.Size = new Size(200, 23);
        this.dtpFechaFin.Format = DateTimePickerFormat.Short;

        // btnFiltrarFechas
        this.btnFiltrarFechas.Name = "btnFiltrarFechas";
        this.btnFiltrarFechas.Text = "Filtrar";
        this.btnFiltrarFechas.Location = new Point(statsX, 295);
        this.btnFiltrarFechas.Size = new Size(120, 35);
        this.btnFiltrarFechas.Click += BtnFiltrarFechas_Click;

        // =============================================
        // MainForm
        // =============================================
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1000, 650);
        this.Controls.Add(this.tabControl);
        this.Name = "MainForm";
        this.Text = "Panel Administrativo - Gestión de Funcionarios";
        this.StartPosition = FormStartPosition.CenterScreen;

        ((System.ComponentModel.ISupportInitialize)this.dgvFuncionarios).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.dgvCalificaciones).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.picQR).EndInit();
        this.ResumeLayout(false);
    }

    #endregion

    // Tab control
    private TabControl tabControl;
    private TabPage tabFuncionarios;
    private TabPage tabQR;
    private TabPage tabCalificaciones;

    // Tab Funcionarios
    private DataGridView dgvFuncionarios;
    private TextBox txtBusqueda;
    private TextBox txtNombre;
    private TextBox txtApellido;
    private TextBox txtCargo;
    private TextBox txtDireccion;
    private Label lblValidacionNombre;
    private Label lblValidacionApellido;
    private Button btnNuevoFuncionario;
    private Button btnEditarFuncionario;

    // Tab QR
    private PictureBox picQR;
    private Button btnGenerarQR;
    private Button btnGuardarQR;
    private Button btnImprimirQR;

    // Tab Calificaciones
    private DataGridView dgvCalificaciones;
    private Label lblNoCalificaciones;
    private Label lblTotal;
    private Label lblExcelente;
    private Label lblBuena;
    private Label lblRegular;
    private Label lblMala;
    private DateTimePicker dtpFechaInicio;
    private DateTimePicker dtpFechaFin;
    private Button btnFiltrarFechas;
}
