using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;
using Capa_Abstracciones.Interfaces;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;

namespace Panel_Admin;

public partial class DashboardForm : Form
{
    private readonly ICalificacionService _calificacionService;
    private readonly IEncargadoService _encargadoService;

    public DashboardForm(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        _calificacionService = scope.ServiceProvider.GetRequiredService<ICalificacionService>();
        _encargadoService = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
        InitializeComponent();
        this.Load += DashboardForm_Load;
    }

    private async void DashboardForm_Load(object? sender, EventArgs e)
    {
        await CargarDashboardAsync();
    }

    private async Task CargarDashboardAsync()
    {
        var calResult = await _calificacionService.ObtenerTodasAsync();
        var encResult = await _encargadoService.ObtenerTodosAsync();

        if (!calResult.IsSuccess || !encResult.IsSuccess) return;

        var calificaciones = calResult.Value!.ToList();
        var funcionarios = encResult.Value!.ToList();

        var excelente = calificaciones.Count(c => c.Valor == ValorCalificacion.Excelente);
        var buena = calificaciones.Count(c => c.Valor == ValorCalificacion.Buena);
        var regular = calificaciones.Count(c => c.Valor == ValorCalificacion.Regular);
        var mala = calificaciones.Count(c => c.Valor == ValorCalificacion.Mala);

        // === Tarjetas de resumen ===
        lblCardTotal.Text = calificaciones.Count.ToString();
        lblCardExcelente.Text = excelente.ToString();
        lblCardBuena.Text = buena.ToString();
        lblCardRegular.Text = regular.ToString();
        lblCardMala.Text = mala.ToString();
        lblCardFuncionarios.Text = funcionarios.Count.ToString();

        // === Gráfica de Barras por funcionario ===
        var porFuncionario = calificaciones
            .GroupBy(c => c.CedulaRucPersona)
            .Select(g =>
            {
                var func = funcionarios.FirstOrDefault(f => f.CedulaRucPersona == g.Key);
                var nombre = func?.Persona != null
                    ? $"{func.Persona.PrimerNombre} {func.Persona.PrimerApellido}"
                    : g.Key;
                return new
                {
                    Nombre = nombre,
                    Excelente = g.Count(c => c.Valor == ValorCalificacion.Excelente),
                    Buena = g.Count(c => c.Valor == ValorCalificacion.Buena),
                    Regular = g.Count(c => c.Valor == ValorCalificacion.Regular),
                    Mala = g.Count(c => c.Valor == ValorCalificacion.Mala),
                    Total = g.Count()
                };
            })
            .OrderByDescending(x => x.Total)
            .Take(8)
            .ToList();

        barChart.XAxes = new[] { new Axis { Labels = porFuncionario.Select(x => x.Nombre).ToArray(), LabelsRotation = 15 } };
        barChart.Series = new ISeries[]
        {
            new ColumnSeries<int> { Values = porFuncionario.Select(x => x.Excelente).ToArray(), Name = "Excelente", Fill = new SolidColorPaint(new SKColor(46, 125, 50)) },
            new ColumnSeries<int> { Values = porFuncionario.Select(x => x.Buena).ToArray(), Name = "Buena", Fill = new SolidColorPaint(new SKColor(76, 175, 80)) },
            new ColumnSeries<int> { Values = porFuncionario.Select(x => x.Regular).ToArray(), Name = "Regular", Fill = new SolidColorPaint(new SKColor(255, 152, 0)) },
            new ColumnSeries<int> { Values = porFuncionario.Select(x => x.Mala).ToArray(), Name = "Mala", Fill = new SolidColorPaint(new SKColor(211, 47, 47)) }
        };

        // === Gráfica de línea temporal ===
        var porDia = calificaciones
            .GroupBy(c => c.FechaHora.Date)
            .OrderBy(g => g.Key)
            .TakeLast(14)
            .ToList();

        lineChart.XAxes = new[] { new Axis { Labels = porDia.Select(g => g.Key.ToString("dd/MM")).ToArray() } };
        lineChart.Series = new ISeries[]
        {
            new LineSeries<int>
            {
                Values = porDia.Select(g => g.Count()).ToArray(),
                Name = "Calificaciones/día",
                Fill = new SolidColorPaint(new SKColor(33, 150, 243, 80)),
                Stroke = new SolidColorPaint(new SKColor(33, 150, 243)) { StrokeThickness = 3 },
                GeometrySize = 8
            }
        };

        // === DataGridView - Últimas calificaciones ===
        var ultimas = calificaciones
            .OrderByDescending(c => c.FechaHora)
            .Take(10)
            .Select(c =>
            {
                var func = funcionarios.FirstOrDefault(f => f.CedulaRucPersona == c.CedulaRucPersona);
                var nombreFunc = func?.Persona != null
                    ? $"{func.Persona.PrimerNombre} {func.Persona.PrimerApellido}"
                    : c.CedulaRucPersona;
                return new
                {
                    Fecha = c.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                    Funcionario = nombreFunc,
                    Valor = c.Valor.ToString(),
                    Comentarios = c.Comentarios ?? "—",
                    IP = c.IpCliente ?? "—",
                    Dispositivo = c.DeviceFingerprint ?? "—"
                };
            })
            .ToList();

        dgvRecientes.DataSource = ultimas;
    }
}
