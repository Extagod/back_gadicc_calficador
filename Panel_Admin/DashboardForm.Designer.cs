using LiveChartsCore.SkiaSharpView.WinForms;

namespace Panel_Admin;

partial class DashboardForm
{
    private System.ComponentModel.IContainer components = null;
    private CartesianChart barChart;
    private PieChart pieChart;
    private Label lblTitulo;
    private Label lblTotalCalificaciones;
    private Label lblTotalFuncionarios;
    private Label lblTituloPie;
    private Label lblTituloBar;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.barChart = new CartesianChart();
        this.pieChart = new PieChart();
        this.lblTitulo = new Label();
        this.lblTotalCalificaciones = new Label();
        this.lblTotalFuncionarios = new Label();
        this.lblTituloPie = new Label();
        this.lblTituloBar = new Label();

        this.SuspendLayout();

        // Título
        this.lblTitulo.Text = "📊 Dashboard de Calificaciones";
        this.lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        this.lblTitulo.Location = new Point(20, 10);
        this.lblTitulo.AutoSize = true;

        // Stats
        this.lblTotalCalificaciones.Text = "Total de calificaciones: 0";
        this.lblTotalCalificaciones.Font = new Font("Segoe UI", 11F);
        this.lblTotalCalificaciones.Location = new Point(20, 50);
        this.lblTotalCalificaciones.AutoSize = true;

        this.lblTotalFuncionarios.Text = "Total de funcionarios: 0";
        this.lblTotalFuncionarios.Font = new Font("Segoe UI", 11F);
        this.lblTotalFuncionarios.Location = new Point(300, 50);
        this.lblTotalFuncionarios.AutoSize = true;

        // Título Pie
        this.lblTituloPie.Text = "Distribución General";
        this.lblTituloPie.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.lblTituloPie.Location = new Point(20, 80);
        this.lblTituloPie.AutoSize = true;

        // Pie Chart
        this.pieChart.Location = new Point(20, 105);
        this.pieChart.Size = new Size(350, 300);

        // Título Bar
        this.lblTituloBar.Text = "Calificaciones por Funcionario (Top 10)";
        this.lblTituloBar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.lblTituloBar.Location = new Point(400, 80);
        this.lblTituloBar.AutoSize = true;

        // Bar Chart
        this.barChart.Location = new Point(400, 105);
        this.barChart.Size = new Size(550, 300);

        // Form
        this.ClientSize = new Size(980, 430);
        this.Controls.AddRange(new Control[]
        {
            this.lblTitulo,
            this.lblTotalCalificaciones,
            this.lblTotalFuncionarios,
            this.lblTituloPie,
            this.pieChart,
            this.lblTituloBar,
            this.barChart
        });
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Dashboard - Estadísticas de Calificaciones";
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
