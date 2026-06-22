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

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.tabControl = new TabControl();
        this.tabFuncionarios = new TabPage();
        this.tabCalificaciones = new TabPage();

        // Tab Funcionarios controls
        this.dgvFuncionarios = new DataGridView();
        this.txtBusqueda = new TextBox();
        this.btnNuevo = new Button();
        this.btnEditar = new Button();
        this.btnEliminar = new Button();

        // Tab Calificaciones controls
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

        var lblBusqueda = new Label();
        var lblInfo = new Label();
        var lblFechaInicioLabel = new Label();
        var lblFechaFinLabel = new Label();

        ((System.ComponentModel.ISupportInitialize)this.dgvFuncionarios).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.dgvCalificaciones).BeginInit();
        this.SuspendLayout();

        // === TabControl ===
        this.tabControl.Dock = DockStyle.Fill;
        this.tabControl.TabPages.AddRange(new TabPage[] { this.tabFuncionarios, this.tabCalificaciones });
        this.tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

        // === Tab Funcionarios ===
        this.tabFuncionarios.Text = "Funcionarios";
        this.tabFuncionarios.Padding = new Padding(10);
        this.btnDashboard = new Button();
        this.tabFuncionarios.Controls.AddRange(new Control[]
        {
            lblBusqueda, this.txtBusqueda, this.dgvFuncionarios,
            this.btnNuevo, this.btnEditar, this.btnEliminar, this.btnDashboard, lblInfo
        });

        lblBusqueda.Text = "Buscar:";
        lblBusqueda.Location = new Point(15, 15);
        lblBusqueda.AutoSize = true;

        this.txtBusqueda.Location = new Point(70, 12);
        this.txtBusqueda.Size = new Size(300, 23);
        this.txtBusqueda.PlaceholderText = "Buscar por nombre o apellido...";
        this.txtBusqueda.TextChanged += TxtBusqueda_TextChanged;

        // Buttons
        this.btnNuevo.Text = "➕ Nuevo";
        this.btnNuevo.Location = new Point(400, 8);
        this.btnNuevo.Size = new Size(110, 30);
        this.btnNuevo.Click += BtnNuevo_Click;

        this.btnEditar.Text = "✏️ Editar";
        this.btnEditar.Location = new Point(520, 8);
        this.btnEditar.Size = new Size(110, 30);
        this.btnEditar.Click += BtnEditar_Click;

        this.btnEliminar.Text = "🗑️ Eliminar";
        this.btnEliminar.Location = new Point(640, 8);
        this.btnEliminar.Size = new Size(110, 30);
        this.btnEliminar.ForeColor = Color.DarkRed;
        this.btnEliminar.Click += BtnEliminar_Click;

        this.btnDashboard.Text = "📊 Dashboard";
        this.btnDashboard.Location = new Point(780, 8);
        this.btnDashboard.Size = new Size(120, 30);
        this.btnDashboard.BackColor = Color.FromArgb(33, 150, 243);
        this.btnDashboard.ForeColor = Color.White;
        this.btnDashboard.FlatStyle = FlatStyle.Flat;
        this.btnDashboard.Click += BtnDashboard_Click;

        // DataGridView
        this.dgvFuncionarios.Location = new Point(15, 45);
        this.dgvFuncionarios.Size = new Size(940, 500);
        this.dgvFuncionarios.AllowUserToAddRows = false;
        this.dgvFuncionarios.AllowUserToDeleteRows = false;
        this.dgvFuncionarios.ReadOnly = true;
        this.dgvFuncionarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvFuncionarios.MultiSelect = false;
        this.dgvFuncionarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvFuncionarios.SelectionChanged += DgvFuncionarios_SelectionChanged;
        this.dgvFuncionarios.CellDoubleClick += DgvFuncionarios_CellDoubleClick;

        // Info label
        lblInfo.Text = "💡 Doble clic en un funcionario para ver su detalle y código QR";
        lblInfo.Location = new Point(15, 552);
        lblInfo.AutoSize = true;
        lblInfo.ForeColor = Color.Gray;
        lblInfo.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);

        // === Tab Calificaciones ===
        this.tabCalificaciones.Text = "Calificaciones";
        this.tabCalificaciones.Padding = new Padding(10);
        this.btnVerTodas = new Button();
        this.tabCalificaciones.Controls.AddRange(new Control[]
        {
            this.dgvCalificaciones, this.lblNoCalificaciones,
            this.lblTotal, this.lblExcelente, this.lblBuena, this.lblRegular, this.lblMala,
            lblFechaInicioLabel, this.dtpFechaInicio,
            lblFechaFinLabel, this.dtpFechaFin,
            this.btnFiltrarFechas, this.btnVerTodas
        });

        this.dgvCalificaciones.Location = new Point(15, 15);
        this.dgvCalificaciones.Size = new Size(700, 530);
        this.dgvCalificaciones.AllowUserToAddRows = false;
        this.dgvCalificaciones.AllowUserToDeleteRows = false;
        this.dgvCalificaciones.ReadOnly = true;
        this.dgvCalificaciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvCalificaciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        this.lblNoCalificaciones.Text = "Cargando calificaciones...";
        this.lblNoCalificaciones.Location = new Point(15, 250);
        this.lblNoCalificaciones.Size = new Size(500, 25);
        this.lblNoCalificaciones.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
        this.lblNoCalificaciones.Visible = false;

        int statsX = 730;

        this.lblTotal.Text = "Total: 0";
        this.lblTotal.Location = new Point(statsX, 20);
        this.lblTotal.Size = new Size(200, 25);
        this.lblTotal.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

        this.lblExcelente.Text = "Excelente: 0";
        this.lblExcelente.Location = new Point(statsX, 55);
        this.lblExcelente.Size = new Size(200, 22);
        this.lblExcelente.ForeColor = Color.FromArgb(46, 125, 50);

        this.lblBuena.Text = "Buena: 0";
        this.lblBuena.Location = new Point(statsX, 80);
        this.lblBuena.Size = new Size(200, 22);
        this.lblBuena.ForeColor = Color.FromArgb(56, 142, 60);

        this.lblRegular.Text = "Regular: 0";
        this.lblRegular.Location = new Point(statsX, 105);
        this.lblRegular.Size = new Size(200, 22);
        this.lblRegular.ForeColor = Color.FromArgb(245, 124, 0);

        this.lblMala.Text = "Mala: 0";
        this.lblMala.Location = new Point(statsX, 130);
        this.lblMala.Size = new Size(200, 22);
        this.lblMala.ForeColor = Color.FromArgb(198, 40, 40);

        lblFechaInicioLabel.Text = "Desde:";
        lblFechaInicioLabel.Location = new Point(statsX, 180);
        lblFechaInicioLabel.AutoSize = true;

        this.dtpFechaInicio.Location = new Point(statsX, 203);
        this.dtpFechaInicio.Size = new Size(220, 23);
        this.dtpFechaInicio.Format = DateTimePickerFormat.Short;

        lblFechaFinLabel.Text = "Hasta:";
        lblFechaFinLabel.Location = new Point(statsX, 235);
        lblFechaFinLabel.AutoSize = true;

        this.dtpFechaFin.Location = new Point(statsX, 258);
        this.dtpFechaFin.Size = new Size(220, 23);
        this.dtpFechaFin.Format = DateTimePickerFormat.Short;

        this.btnFiltrarFechas.Text = "🔍 Filtrar";
        this.btnFiltrarFechas.Location = new Point(statsX, 295);
        this.btnFiltrarFechas.Size = new Size(100, 35);
        this.btnFiltrarFechas.Click += BtnFiltrarFechas_Click;

        this.btnVerTodas.Text = "📋 Ver Todas";
        this.btnVerTodas.Location = new Point(statsX + 110, 295);
        this.btnVerTodas.Size = new Size(110, 35);
        this.btnVerTodas.Click += BtnVerTodas_Click;

        // === MainForm ===
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1000, 620);
        this.Controls.Add(this.tabControl);
        this.Text = "Panel Administrativo - Gestión de Funcionarios";
        this.StartPosition = FormStartPosition.CenterScreen;

        ((System.ComponentModel.ISupportInitialize)this.dgvFuncionarios).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.dgvCalificaciones).EndInit();
        this.ResumeLayout(false);
    }

    private TabControl tabControl;
    private TabPage tabFuncionarios;
    private TabPage tabCalificaciones;

    // Tab Funcionarios
    private DataGridView dgvFuncionarios;
    private TextBox txtBusqueda;
    private Button btnNuevo;
    private Button btnEditar;
    private Button btnEliminar;
    private Button btnDashboard;

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
    private Button btnVerTodas;
}
