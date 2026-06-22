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

        // === Estadísticas generales ===
        lblTotalCalificaciones.Text = $"Total de calificaciones: {calificaciones.Count}";
        lblTotalFuncionarios.Text = $"Total de funcionarios: {funcionarios.Count}";

        var excelente = calificaciones.Count(c => c.Valor == ValorCalificacion.Excelente);
        var buena = calificaciones.Count(c => c.Valor == ValorCalificacion.Buena);
        var regular = calificaciones.Count(c => c.Valor == ValorCalificacion.Regular);
        var mala = calificaciones.Count(c => c.Valor == ValorCalificacion.Mala);

        // === Gráfica de Pastel - Distribución de calificaciones ===
        pieChart.Series = new ISeries[]
        {
            new PieSeries<int>
            {
                Values = new[] { excelente },
                Name = "Excelente",
                Fill = new SolidColorPaint(new SKColor(46, 125, 50))
            },
            new PieSeries<int>
            {
                Values = new[] { buena },
                Name = "Buena",
                Fill = new SolidColorPaint(new SKColor(76, 175, 80))
            },
            new PieSeries<int>
            {
                Values = new[] { regular },
                Name = "Regular",
                Fill = new SolidColorPaint(new SKColor(255, 152, 0))
            },
            new PieSeries<int>
            {
                Values = new[] { mala },
                Name = "Mala",
                Fill = new SolidColorPaint(new SKColor(211, 47, 47))
            }
        };

        // === Gráfica de Barras - Calificaciones por funcionario ===
        var porFuncionario = calificaciones
            .GroupBy(c => c.IdEncargado)
            .Select(g =>
            {
                var func = funcionarios.FirstOrDefault(f => f.IdEncargado == g.Key);
                return new
                {
                    Nombre = func is not null ? $"{func.Nombre} {func.Apellido}" : $"ID:{g.Key}",
                    Excelente = g.Count(c => c.Valor == ValorCalificacion.Excelente),
                    Buena = g.Count(c => c.Valor == ValorCalificacion.Buena),
                    Regular = g.Count(c => c.Valor == ValorCalificacion.Regular),
                    Mala = g.Count(c => c.Valor == ValorCalificacion.Mala),
                    Total = g.Count()
                };
            })
            .OrderByDescending(x => x.Total)
            .Take(10)
            .ToList();

        var nombres = porFuncionario.Select(x => x.Nombre).ToArray();

        barChart.XAxes = new[]
        {
            new Axis
            {
                Labels = nombres,
                LabelsRotation = 15
            }
        };

        barChart.Series = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = porFuncionario.Select(x => x.Excelente).ToArray(),
                Name = "Excelente",
                Fill = new SolidColorPaint(new SKColor(46, 125, 50))
            },
            new ColumnSeries<int>
            {
                Values = porFuncionario.Select(x => x.Buena).ToArray(),
                Name = "Buena",
                Fill = new SolidColorPaint(new SKColor(76, 175, 80))
            },
            new ColumnSeries<int>
            {
                Values = porFuncionario.Select(x => x.Regular).ToArray(),
                Name = "Regular",
                Fill = new SolidColorPaint(new SKColor(255, 152, 0))
            },
            new ColumnSeries<int>
            {
                Values = porFuncionario.Select(x => x.Mala).ToArray(),
                Name = "Mala",
                Fill = new SolidColorPaint(new SKColor(211, 47, 47))
            }
        };
    }
}
