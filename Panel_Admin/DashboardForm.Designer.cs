using LiveChartsCore.SkiaSharpView.WinForms;

namespace Panel_Admin;

partial class DashboardForm
{
    private System.ComponentModel.IContainer components = null;
    private CartesianChart barChart;
    private CartesianChart lineChart;
    private PieChart pieChart;
    private DataGridView dgvRecientes;

    // Cards
    private Panel cardTotal, cardExcelente, cardBuena, cardRegular, cardMala, cardFuncionarios;
    private Label lblCardTotal, lblCardExcelente, lblCardBuena, lblCardRegular, lblCardMala, lblCardFuncionarios;
    private Label lblCardTotalTitle, lblCardExcelenteTitle, lblCardBuenaTitle, lblCardRegularTitle, lblCardMalaTitle, lblCardFuncionariosTitle;

    // Section labels
    private Label lblTitulo, lblSeccionBarras, lblSeccionLinea, lblSeccionRecientes, lblSeccionDonut;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.barChart = new CartesianChart();
        this.lineChart = new CartesianChart();
        this.pieChart = new PieChart();
        this.dgvRecientes = new DataGridView();

        this.lblTitulo = new Label();
        this.lblSeccionBarras = new Label();
        this.lblSeccionLinea = new Label();
        this.lblSeccionRecientes = new Label();
        this.lblSeccionDonut = new Label();

        ((System.ComponentModel.ISupportInitialize)this.dgvRecientes).BeginInit();
        this.SuspendLayout();

        // === TÍTULO ===
        this.lblTitulo.Text = "DASHBOARD";
        this.lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        this.lblTitulo.ForeColor = Color.FromArgb(50, 50, 50);
        this.lblTitulo.Location = new Point(520, 12);
        this.lblTitulo.AutoSize = true;

        // === TARJETAS DE RESUMEN (fila superior) ===
        int cardY = 55, cardH = 85, cardW = 185, gap = 12;
        int startX = 20;

        // Card Total
        cardTotal = CrearCard(startX, cardY, cardW, cardH, Color.FromArgb(63, 81, 181));
        lblCardTotalTitle = CrearCardTitle(cardTotal, "Total Calificaciones");
        lblCardTotal = CrearCardValue(cardTotal, "0");

        // Card Excelente
        cardExcelente = CrearCard(startX + (cardW + gap), cardY, cardW, cardH, Color.FromArgb(46, 125, 50));
        lblCardExcelenteTitle = CrearCardTitle(cardExcelente, "Excelente");
        lblCardExcelente = CrearCardValue(cardExcelente, "0");

        // Card Buena
        cardBuena = CrearCard(startX + 2 * (cardW + gap), cardY, cardW, cardH, Color.FromArgb(76, 175, 80));
        lblCardBuenaTitle = CrearCardTitle(cardBuena, "Buena");
        lblCardBuena = CrearCardValue(cardBuena, "0");

        // Card Regular
        cardRegular = CrearCard(startX + 3 * (cardW + gap), cardY, cardW, cardH, Color.FromArgb(255, 152, 0));
        lblCardRegularTitle = CrearCardTitle(cardRegular, "Regular");
        lblCardRegular = CrearCardValue(cardRegular, "0");

        // Card Mala
        cardMala = CrearCard(startX + 4 * (cardW + gap), cardY, cardW, cardH, Color.FromArgb(211, 47, 47));
        lblCardMalaTitle = CrearCardTitle(cardMala, "Mala");
        lblCardMala = CrearCardValue(cardMala, "0");

        // Card Funcionarios
        cardFuncionarios = CrearCard(startX + 5 * (cardW + gap), cardY, cardW, cardH, Color.FromArgb(96, 96, 96));
        lblCardFuncionariosTitle = CrearCardTitle(cardFuncionarios, "Funcionarios");
        lblCardFuncionarios = CrearCardValue(cardFuncionarios, "0");

        // === GRÁFICA DE LÍNEA (tendencia temporal) ===
        this.lblSeccionLinea.Text = "Tendencia de Calificaciones (Últimos 14 días)";
        this.lblSeccionLinea.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.lblSeccionLinea.ForeColor = Color.FromArgb(50, 50, 50);
        this.lblSeccionLinea.Location = new Point(20, 155);
        this.lblSeccionLinea.AutoSize = true;

        this.lineChart.Location = new Point(20, 178);
        this.lineChart.Size = new Size(720, 220);

        // === GRÁFICA DE BARRAS ===
        this.lblSeccionBarras.Text = "Calificaciones por Funcionario";
        this.lblSeccionBarras.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.lblSeccionBarras.ForeColor = Color.FromArgb(50, 50, 50);
        this.lblSeccionBarras.Location = new Point(760, 155);
        this.lblSeccionBarras.AutoSize = true;

        this.barChart.Location = new Point(760, 178);
        this.barChart.Size = new Size(430, 220);
        this.lblSeccionRecientes.Text = "Últimas 10 Calificaciones";
        this.lblSeccionRecientes.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.lblSeccionRecientes.ForeColor = Color.FromArgb(50, 50, 50);
        this.lblSeccionRecientes.Location = new Point(20, 410);
        this.lblSeccionRecientes.AutoSize = true;

        this.dgvRecientes.Location = new Point(20, 435);
        this.dgvRecientes.Size = new Size(1170, 270);
        this.dgvRecientes.AllowUserToAddRows = false;
        this.dgvRecientes.AllowUserToDeleteRows = false;
        this.dgvRecientes.ReadOnly = true;
        this.dgvRecientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvRecientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvRecientes.BackgroundColor = Color.White;
        this.dgvRecientes.BorderStyle = BorderStyle.FixedSingle;
        this.dgvRecientes.RowHeadersVisible = false;
        this.dgvRecientes.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
        this.dgvRecientes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        this.dgvRecientes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(63, 81, 181);
        this.dgvRecientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        this.dgvRecientes.EnableHeadersVisualStyles = false;
        this.dgvRecientes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 250);

        // === FORM ===
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1210, 720);
        this.BackColor = Color.FromArgb(240, 242, 245);

        this.Controls.Add(this.lblTitulo);
        this.Controls.Add(this.cardTotal);
        this.Controls.Add(this.cardExcelente);
        this.Controls.Add(this.cardBuena);
        this.Controls.Add(this.cardRegular);
        this.Controls.Add(this.cardMala);
        this.Controls.Add(this.cardFuncionarios);
        this.Controls.Add(this.lblSeccionLinea);
        this.Controls.Add(this.lineChart);
        this.Controls.Add(this.lblSeccionBarras);
        this.Controls.Add(this.barChart);
        this.Controls.Add(this.lblSeccionRecientes);
        this.Controls.Add(this.dgvRecientes);

        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.MaximizeBox = true;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Dashboard - Estadísticas de Calificaciones";
        this.WindowState = FormWindowState.Maximized;

        ((System.ComponentModel.ISupportInitialize)this.dgvRecientes).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private Panel CrearCard(int x, int y, int w, int h, Color bgColor)
    {
        var panel = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(w, h),
            BackColor = bgColor
        };
        panel.Paint += (s, e) =>
        {
            // Rounded corners effect
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        };
        return panel;
    }

    private Label CrearCardTitle(Panel parent, string title)
    {
        var lbl = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(240, 240, 240),
            Location = new Point(12, 8),
            AutoSize = true
        };
        parent.Controls.Add(lbl);
        return lbl;
    }

    private Label CrearCardValue(Panel parent, string value)
    {
        var lbl = new Label
        {
            Text = value,
            Font = new Font("Segoe UI", 24F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(12, 35),
            AutoSize = true
        };
        parent.Controls.Add(lbl);
        return lbl;
    }
}
