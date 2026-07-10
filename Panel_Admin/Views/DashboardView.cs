using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;
using Capa_Abstracciones.Interfaces;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using Microsoft.Extensions.DependencyInjection;
using Panel_Admin.UI;
using SkiaSharp;

namespace Panel_Admin.Views;

public class DashboardView : UserControl
{
    private readonly IServiceProvider _sp;

    private MetricCard _cardTotal = null!, _cardExc = null!, _cardBuena = null!, _cardReg = null!, _cardMala = null!, _cardFunc = null!;
    private PieChart _pieChart = null!;
    private CartesianChart _barChart = null!;
    private DataGridView _gridRecientes = null!;
    private FlowLayoutPanel _panelAlertas = null!;
    private LoadingOverlay _overlay = null!;
    private Panel _pieEmpty = null!, _barEmpty = null!;

    public DashboardView(IServiceProvider serviceProvider)
    {
        _sp = serviceProvider;
        AutoScaleMode = AutoScaleMode.None;
        Dock = DockStyle.Fill;
        BackColor = UITheme.Background;
        AutoScroll = true;
        ConstruirUI();
    }

    private void ConstruirUI()
    {
        // === Tarjetas de métricas ===
        var cards = new TableLayoutPanel
        {
            Dock = DockStyle.Top, Height = 160, ColumnCount = 6, RowCount = 1, BackColor = UITheme.Background
        };
        for (int i = 0; i < 6; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 6));

        _cardTotal = new MetricCard("Total", UITheme.Primary, "📊") { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 6, 0) };
        _cardExc = new MetricCard("Excelente", UITheme.Excelente, "★") { Dock = DockStyle.Fill, Margin = new Padding(6, 0, 6, 0) };
        _cardBuena = new MetricCard("Buena", UITheme.Buena, "👍") { Dock = DockStyle.Fill, Margin = new Padding(6, 0, 6, 0) };
        _cardReg = new MetricCard("Regular", UITheme.Regular, "≈") { Dock = DockStyle.Fill, Margin = new Padding(6, 0, 6, 0) };
        _cardMala = new MetricCard("Mala", UITheme.Mala, "▼") { Dock = DockStyle.Fill, Margin = new Padding(6, 0, 6, 0) };
        _cardFunc = new MetricCard("Funcionarios", UITheme.Neutral, "👥") { Dock = DockStyle.Fill, Margin = new Padding(6, 0, 0, 0) };
        cards.Controls.Add(_cardTotal, 0, 0);
        cards.Controls.Add(_cardExc, 1, 0);
        cards.Controls.Add(_cardBuena, 2, 0);
        cards.Controls.Add(_cardReg, 3, 0);
        cards.Controls.Add(_cardMala, 4, 0);
        cards.Controls.Add(_cardFunc, 5, 0);

        // === Fila de gráficas ===
        var graficas = new TableLayoutPanel
        {
            Dock = DockStyle.Top, Height = 300, ColumnCount = 2, RowCount = 1, BackColor = UITheme.Background,
            Margin = new Padding(0, 12, 0, 0)
        };
        graficas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
        graficas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));

        var cardPie = CrearCardPie("Distribución por tipo de calificación", out _pieChart, out _pieEmpty);
        cardPie.Margin = new Padding(0, 0, 6, 0);
        var cardBar = CrearCardGrafica("Calificaciones por funcionario", out _barChart, out _barEmpty);
        cardBar.Margin = new Padding(6, 0, 0, 0);
        graficas.Controls.Add(cardPie, 0, 0);
        graficas.Controls.Add(cardBar, 1, 0);

        // === Fila inferior: recientes + alertas ===
        var inferior = new TableLayoutPanel
        {
            Dock = DockStyle.Top, Height = 320, ColumnCount = 2, RowCount = 1, BackColor = UITheme.Background,
            Margin = new Padding(0, 12, 0, 0)
        };
        inferior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62f));
        inferior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38f));

        // Recientes
        var cardRecientes = new RoundedPanel { Dock = DockStyle.Fill, Radius = 12, Padding = new Padding(16, 14, 16, 16), Margin = new Padding(0, 0, 6, 0) };
        var lblRec = new Label { Text = "Últimas calificaciones", Font = UITheme.SectionTitle, ForeColor = UITheme.TextPrimary, Dock = DockStyle.Top, Height = 30 };
        _gridRecientes = new DataGridView { Dock = DockStyle.Fill };
        GridStyler.Apply(_gridRecientes);
        _gridRecientes.RowTemplate.Height = 34;
        _gridRecientes.ColumnHeadersHeight = 38;
        cardRecientes.Controls.Add(_gridRecientes);
        cardRecientes.Controls.Add(lblRec);

        // Alertas
        var cardAlertas = new RoundedPanel { Dock = DockStyle.Fill, Radius = 12, Padding = new Padding(16, 14, 16, 16), Margin = new Padding(6, 0, 0, 0) };
        var lblAlertas = new Label { Text = "Alertas y funcionarios destacados", Font = UITheme.SectionTitle, ForeColor = UITheme.TextPrimary, Dock = DockStyle.Top, Height = 30 };
        _panelAlertas = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
        cardAlertas.Controls.Add(_panelAlertas);
        cardAlertas.Controls.Add(lblAlertas);

        inferior.Controls.Add(cardRecientes, 0, 0);
        inferior.Controls.Add(cardAlertas, 1, 0);

        _overlay = new LoadingOverlay("Cargando dashboard...");

        var sp1 = new Panel { Dock = DockStyle.Top, Height = 12, BackColor = UITheme.Background };
        var sp2 = new Panel { Dock = DockStyle.Top, Height = 12, BackColor = UITheme.Background };

        // Orden inverso para Dock=Top
        Controls.Add(inferior);
        Controls.Add(sp2);
        Controls.Add(graficas);
        Controls.Add(sp1);
        Controls.Add(cards);
        Controls.Add(_overlay);
    }

    private RoundedPanel CrearCardGrafica(string titulo, out CartesianChart chart, out Panel emptyState)
    {
        var card = new RoundedPanel { Dock = DockStyle.Fill, Radius = 12, Padding = new Padding(16, 14, 16, 14) };
        var lbl = new Label { Text = titulo, Font = UITheme.SectionTitle, ForeColor = UITheme.TextPrimary, Dock = DockStyle.Top, Height = 30 };
        chart = new CartesianChart { Dock = DockStyle.Fill, BackColor = UITheme.Card, LegendPosition = LegendPosition.Bottom };
        emptyState = new Panel { Dock = DockStyle.Fill, Visible = false, BackColor = UITheme.Card };
        var lblEmpty = new Label
        {
            Text = "Sin datos para mostrar", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
            Font = UITheme.Body, ForeColor = UITheme.TextMuted
        };
        emptyState.Controls.Add(lblEmpty);
        card.Controls.Add(chart);
        card.Controls.Add(emptyState);
        card.Controls.Add(lbl);
        return card;
    }

    private RoundedPanel CrearCardPie(string titulo, out PieChart chart, out Panel emptyState)
    {
        var card = new RoundedPanel { Dock = DockStyle.Fill, Radius = 12, Padding = new Padding(16, 14, 16, 14) };
        var lbl = new Label { Text = titulo, Font = UITheme.SectionTitle, ForeColor = UITheme.TextPrimary, Dock = DockStyle.Top, Height = 30 };
        chart = new PieChart { Dock = DockStyle.Fill, BackColor = UITheme.Card, LegendPosition = LegendPosition.Bottom };
        emptyState = new Panel { Dock = DockStyle.Fill, Visible = false, BackColor = UITheme.Card };
        var lblEmpty = new Label
        {
            Text = "Sin datos para mostrar", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
            Font = UITheme.Body, ForeColor = UITheme.TextMuted
        };
        emptyState.Controls.Add(lblEmpty);
        card.Controls.Add(chart);
        card.Controls.Add(emptyState);
        card.Controls.Add(lbl);
        return card;
    }

    public async void CargarDatos()
    {
        _overlay.Mostrar("Cargando dashboard...");
        try
        {
            using var scope = _sp.CreateScope();
            var calSvc = scope.ServiceProvider.GetRequiredService<ICalificacionService>();
            var encSvc = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
            var calRes = await calSvc.ObtenerTodasAsync();
            var encRes = await encSvc.ObtenerTodosAsync();
            var cal = calRes.IsSuccess ? calRes.Value!.ToList() : new();
            var func = encRes.IsSuccess ? encRes.Value!.ToList() : new();
            Render(cal, func);
        }
        catch (Exception)
        {
            Notificador.Error(FindForm(), "No se pudo cargar el dashboard. Verifique la conexión.");
        }
        finally { _overlay.Ocultar(); }
    }

    private string NombreFunc(List<Empleado> func, string cedula)
    {
        var f = func.FirstOrDefault(x => x.CedulaRucPersona == cedula);
        return f?.Persona != null ? $"{f.Persona.PrimerNombre} {f.Persona.PrimerApellido}".Trim() : cedula;
    }

    private void Render(List<Calificacion> cal, List<Empleado> func)
    {
        _cardTotal.Valor = cal.Count.ToString();
        _cardExc.Valor = cal.Count(c => c.Valor == ValorCalificacion.Excelente).ToString();
        _cardBuena.Valor = cal.Count(c => c.Valor == ValorCalificacion.Buena).ToString();
        _cardReg.Valor = cal.Count(c => c.Valor == ValorCalificacion.Regular).ToString();
        _cardMala.Valor = cal.Count(c => c.Valor == ValorCalificacion.Mala).ToString();
        _cardFunc.Valor = func.Count.ToString();

        // === Dona (distribución por tipo de calificación) ===
        var distrib = new (string Nombre, int Valor, SKColor Color)[]
        {
            ("Excelente", cal.Count(c => c.Valor == ValorCalificacion.Excelente), new SKColor(46, 125, 50)),
            ("Buena",     cal.Count(c => c.Valor == ValorCalificacion.Buena),     new SKColor(102, 187, 106)),
            ("Regular",   cal.Count(c => c.Valor == ValorCalificacion.Regular),   new SKColor(245, 124, 0)),
            ("Mala",      cal.Count(c => c.Valor == ValorCalificacion.Mala),      new SKColor(211, 47, 47)),
        };
        if (cal.Count == 0)
        {
            _pieChart.Visible = false; _pieEmpty.Visible = true; _pieEmpty.BringToFront();
        }
        else
        {
            _pieEmpty.Visible = false; _pieChart.Visible = true;
            _pieChart.Series = distrib.Where(d => d.Valor > 0).Select(d =>
                new PieSeries<int>
                {
                    Values = new[] { d.Valor },
                    Name = d.Nombre,
                    Fill = new SolidColorPaint(d.Color),
                    InnerRadius = 62,
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 15,
                    DataLabelsPosition = PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => point.Coordinate.PrimaryValue.ToString("0")
                }).ToArray();
        }

        // === Barras (por funcionario, top 8) ===
        var porFunc = cal.GroupBy(c => c.CedulaRucPersona)
            .Select(g => new
            {
                Nombre = NombreFunc(func, g.Key),
                Exc = g.Count(c => c.Valor == ValorCalificacion.Excelente),
                Bue = g.Count(c => c.Valor == ValorCalificacion.Buena),
                Reg = g.Count(c => c.Valor == ValorCalificacion.Regular),
                Mal = g.Count(c => c.Valor == ValorCalificacion.Mala),
                Total = g.Count()
            })
            .OrderByDescending(x => x.Total).Take(8).ToList();

        if (porFunc.Count == 0)
        {
            _barChart.Visible = false; _barEmpty.Visible = true; _barEmpty.BringToFront();
        }
        else
        {
            _barEmpty.Visible = false; _barChart.Visible = true;
            _barChart.XAxes = new[] { new Axis { Labels = porFunc.Select(x => x.Nombre).ToArray(), LabelsRotation = 20, TextSize = 10 } };
            _barChart.YAxes = new[] { new Axis { MinLimit = 0, TextSize = 11, MinStep = 1 } };
            _barChart.Series = new ISeries[]
            {
                new ColumnSeries<int> { Values = porFunc.Select(x => x.Exc).ToArray(), Name = "Excelente", Fill = new SolidColorPaint(new SKColor(46, 125, 50)), Rx = 5, Ry = 5, MaxBarWidth = 26 },
                new ColumnSeries<int> { Values = porFunc.Select(x => x.Bue).ToArray(), Name = "Buena", Fill = new SolidColorPaint(new SKColor(102, 187, 106)), Rx = 5, Ry = 5, MaxBarWidth = 26 },
                new ColumnSeries<int> { Values = porFunc.Select(x => x.Reg).ToArray(), Name = "Regular", Fill = new SolidColorPaint(new SKColor(245, 124, 0)), Rx = 5, Ry = 5, MaxBarWidth = 26 },
                new ColumnSeries<int> { Values = porFunc.Select(x => x.Mal).ToArray(), Name = "Mala", Fill = new SolidColorPaint(new SKColor(211, 47, 47)), Rx = 5, Ry = 5, MaxBarWidth = 26 }
            };
        }

        // === Grid recientes ===
        _gridRecientes.Columns.Clear();
        _gridRecientes.AutoGenerateColumns = false;
        _gridRecientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha", Name = "f", FillWeight = 24 });
        _gridRecientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Funcionario", Name = "n", FillWeight = 32 });
        _gridRecientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Calificación", Name = "v", FillWeight = 22 });
        _gridRecientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Comentario", Name = "c", FillWeight = 22 });
        _gridRecientes.Columns["v"].DefaultCellStyle.Font = new Font(UITheme.FontFamilySemibold, 9f, FontStyle.Bold);
        _gridRecientes.Rows.Clear();
        foreach (var c in cal.OrderByDescending(c => c.FechaHora).Take(10))
        {
            int idx = _gridRecientes.Rows.Add(
                c.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                NombreFunc(func, c.CedulaRucPersona),
                c.Valor.ToString(),
                string.IsNullOrWhiteSpace(c.Comentarios) ? "—" : c.Comentarios);
            var color = UITheme.ColorCalificacion((int)c.Valor);
            _gridRecientes.Rows[idx].Cells["v"].Style.ForeColor = color;
            _gridRecientes.Rows[idx].Cells["v"].Style.SelectionForeColor = color;
        }

        // === Alertas ===
        RenderAlertas(cal, func);
    }

    private void RenderAlertas(List<Calificacion> cal, List<Empleado> func)
    {
        _panelAlertas.Controls.Clear();

        // Mejor valorado (más excelentes)
        var mejor = cal.Where(c => c.Valor == ValorCalificacion.Excelente)
            .GroupBy(c => c.CedulaRucPersona)
            .OrderByDescending(g => g.Count()).FirstOrDefault();
        if (mejor != null)
            _panelAlertas.Controls.Add(CrearAlerta("★ Mejor valorado",
                $"{NombreFunc(func, mejor.Key)} — {mejor.Count()} calificaciones excelentes", UITheme.Excelente));

        // Funcionarios con regulares/malas
        var conNegativas = cal.Where(c => c.Valor == ValorCalificacion.Regular || c.Valor == ValorCalificacion.Mala)
            .GroupBy(c => c.CedulaRucPersona)
            .OrderByDescending(g => g.Count()).Take(3).ToList();
        foreach (var g in conNegativas)
            _panelAlertas.Controls.Add(CrearAlerta("⚠ Requiere atención",
                $"{NombreFunc(func, g.Key)} — {g.Count()} calificaciones regulares/malas", UITheme.Warning));

        // Días sin calificaciones (en últimos 7 días)
        var hoy = DateTime.Now.Date;
        int diasSin = 0;
        for (int i = 0; i < 7; i++)
        {
            var dia = hoy.AddDays(-i);
            if (!cal.Any(c => c.FechaHora.Date == dia)) diasSin++;
        }
        if (diasSin > 0)
            _panelAlertas.Controls.Add(CrearAlerta("📅 Actividad reciente",
                $"{diasSin} de los últimos 7 días sin calificaciones registradas", UITheme.Info));

        // Registro más reciente
        var reciente = cal.OrderByDescending(c => c.FechaHora).FirstOrDefault();
        if (reciente != null)
            _panelAlertas.Controls.Add(CrearAlerta("🕓 Último registro",
                $"{reciente.FechaHora:dd/MM/yyyy HH:mm} — {NombreFunc(func, reciente.CedulaRucPersona)}", UITheme.Neutral));

        if (_panelAlertas.Controls.Count == 0)
            _panelAlertas.Controls.Add(CrearAlerta("Sin datos", "Aún no hay calificaciones registradas.", UITheme.TextMuted));
    }

    private Control CrearAlerta(string titulo, string detalle, Color color)
    {
        int ancho = Math.Max(_panelAlertas.ClientSize.Width - 24, 300);
        var p = new RoundedPanel
        {
            Width = ancho,
            Height = 64,
            Radius = 8,
            BorderColor = UITheme.Border,
            Margin = new Padding(0, 0, 0, 8),
            BackColor = UITheme.Card
        };
        var barra = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = color };
        var lblT = new Label { Text = titulo, Font = UITheme.BodyBold, ForeColor = color, AutoSize = false, Location = new Point(16, 10), Size = new Size(p.Width - 24, 20) };
        var lblD = new Label { Text = detalle, Font = UITheme.Small, ForeColor = UITheme.TextSecondary, AutoSize = false, Location = new Point(16, 32), Size = new Size(p.Width - 24, 24) };
        p.Controls.Add(lblD);
        p.Controls.Add(lblT);
        p.Controls.Add(barra);
        return p;
    }
}
