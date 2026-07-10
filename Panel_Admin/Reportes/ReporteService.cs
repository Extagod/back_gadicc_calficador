using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Panel_Admin.Reportes;

/// <summary>Construye el modelo del reporte y lo exporta a PDF/Excel usando datos reales.</summary>
public class ReporteService
{
    static ReporteService()
    {
        // Licencia comunitaria de QuestPDF (gratuita)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>Construye el resultado del reporte a partir de las calificaciones y funcionarios reales.</summary>
    public ReporteResultado Construir(ReporteFiltro filtro, List<Calificacion> calificaciones, List<Empleado> funcionarios)
    {
        string Nombre(string cedula)
        {
            var f = funcionarios.FirstOrDefault(x => x.CedulaRucPersona == cedula);
            return f?.Persona != null ? $"{f.Persona.PrimerNombre} {f.Persona.PrimerApellido}".Trim() : cedula;
        }
        string Cargo(string cedula) => funcionarios.FirstOrDefault(x => x.CedulaRucPersona == cedula)?.Cargo ?? "—";

        var datos = calificaciones.Where(c =>
            c.FechaHora.Date >= filtro.Desde.Date && c.FechaHora.Date <= filtro.Hasta.Date);

        if (!string.IsNullOrEmpty(filtro.CedulaFuncionario))
            datos = datos.Where(c => c.CedulaRucPersona == filtro.CedulaFuncionario);
        if (!string.IsNullOrEmpty(filtro.Cargo))
            datos = datos.Where(c => Cargo(c.CedulaRucPersona) == filtro.Cargo);
        if (filtro.Tipo.HasValue)
            datos = datos.Where(c => c.Valor == filtro.Tipo.Value);

        var lista = datos.OrderByDescending(c => c.FechaHora).ToList();

        var r = new ReporteResultado
        {
            Filtro = filtro,
            GeneradoEn = DateTime.Now,
            Total = lista.Count,
            Excelente = lista.Count(c => c.Valor == ValorCalificacion.Excelente),
            Buena = lista.Count(c => c.Valor == ValorCalificacion.Buena),
            Regular = lista.Count(c => c.Valor == ValorCalificacion.Regular),
            Mala = lista.Count(c => c.Valor == ValorCalificacion.Mala),
            FuncionarioUnico = !string.IsNullOrEmpty(filtro.CedulaFuncionario),
            NombreFuncionario = string.IsNullOrEmpty(filtro.CedulaFuncionario) ? null : Nombre(filtro.CedulaFuncionario)
        };

        r.Filas = lista.Select(c => new ReporteFila
        {
            Fecha = c.FechaHora,
            Cedula = c.CedulaRucPersona,
            Funcionario = Nombre(c.CedulaRucPersona),
            Cargo = Cargo(c.CedulaRucPersona),
            Valor = c.Valor.ToString(),
            ValorNum = (int)c.Valor,
            Comentario = c.Comentarios ?? ""
        }).ToList();

        r.PorFuncionario = lista.GroupBy(c => c.CedulaRucPersona)
            .Select(g => new ReporteFuncionario
            {
                Cedula = g.Key,
                Nombre = Nombre(g.Key),
                Cargo = Cargo(g.Key),
                Total = g.Count(),
                Excelente = g.Count(c => c.Valor == ValorCalificacion.Excelente),
                Buena = g.Count(c => c.Valor == ValorCalificacion.Buena),
                Regular = g.Count(c => c.Valor == ValorCalificacion.Regular),
                Mala = g.Count(c => c.Valor == ValorCalificacion.Mala)
            })
            .OrderByDescending(x => x.Total).ToList();

        return r;
    }

    public string NombreArchivo(ReporteResultado r, string extension)
        => $"Reporte_Calificaciones_{r.Filtro.Desde:yyyy-MM-dd}_{r.Filtro.Hasta:yyyy-MM-dd}.{extension}";

    // =====================================================================
    // EXPORTACIÓN PDF (QuestPDF)
    // =====================================================================
    public void ExportarPdf(ReporteResultado r, string ruta)
    {
        Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(35);
                page.DefaultTextStyle(t => t.FontSize(10).FontColor("#212B36"));

                // Encabezado
                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("GADICC Calificador").FontSize(18).Bold().FontColor("#1E5AA8");
                            c.Item().Text("Reporte de Calificaciones de Funcionarios").FontSize(11).FontColor("#6D7A8A");
                        });
                        row.ConstantItem(150).AlignRight().Column(c =>
                        {
                            c.Item().Text($"Generado: {r.GeneradoEn:dd/MM/yyyy HH:mm}").FontSize(8).FontColor("#6D7A8A");
                        });
                    });
                    col.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor("#1E5AA8");
                });

                page.Content().PaddingVertical(12).Column(col =>
                {
                    col.Spacing(12);

                    // Periodo y filtros
                    col.Item().Background("#EAF2FF").Padding(10).Column(c =>
                    {
                        c.Item().Text($"Período: {r.Filtro.Desde:dd/MM/yyyy} — {r.Filtro.Hasta:dd/MM/yyyy}").Bold();
                        if (r.FuncionarioUnico)
                            c.Item().Text($"Funcionario: {r.NombreFuncionario} (Cédula: {r.Filtro.CedulaFuncionario})");
                        if (!string.IsNullOrEmpty(r.Filtro.Cargo))
                            c.Item().Text($"Cargo: {r.Filtro.Cargo}");
                        if (r.Filtro.Tipo.HasValue)
                            c.Item().Text($"Tipo de calificación: {r.Filtro.Tipo.Value}");
                    });

                    // Resumen
                    col.Item().Text("Resumen general").FontSize(13).Bold().FontColor("#1E5AA8");
                    col.Item().Row(row =>
                    {
                        void Tarjeta(RowDescriptor rd, string titulo, int valor, double pct, string color)
                        {
                            rd.RelativeItem().Border(1).BorderColor("#D9E1EA").Padding(8).Column(c =>
                            {
                                c.Item().Text(titulo).FontSize(9).FontColor("#6D7A8A");
                                c.Item().Text(valor.ToString()).FontSize(18).Bold().FontColor(color);
                                c.Item().Text($"{pct:0.0}%").FontSize(8).FontColor("#6D7A8A");
                            });
                        }
                        Tarjeta(row, "Total", r.Total, 100, "#1E5AA8");
                        row.ConstantItem(6);
                        Tarjeta(row, "Excelente", r.Excelente, r.PctExcelente, "#2E7D32");
                        row.ConstantItem(6);
                        Tarjeta(row, "Buena", r.Buena, r.PctBuena, "#66BB6A");
                        row.ConstantItem(6);
                        Tarjeta(row, "Regular", r.Regular, r.PctRegular, "#F57C00");
                        row.ConstantItem(6);
                        Tarjeta(row, "Mala", r.Mala, r.PctMala, "#D32F2F");
                    });

                    // Datos por funcionario o ranking
                    if (r.FuncionarioUnico && r.PorFuncionario.Count > 0)
                    {
                        var f = r.PorFuncionario[0];
                        col.Item().Text("Detalle del funcionario").FontSize(13).Bold().FontColor("#1E5AA8");
                        col.Item().Column(c =>
                        {
                            c.Item().Text($"Nombre: {f.Nombre}");
                            c.Item().Text($"Cédula: {f.Cedula}    Cargo: {f.Cargo}");
                            c.Item().Text($"Total de calificaciones: {f.Total}");
                            c.Item().Text($"Índice de satisfacción: {f.IndiceSatisfaccion:0.0} / 100");
                        });
                    }
                    else if (r.PorFuncionario.Count > 0)
                    {
                        col.Item().Text("Resumen por funcionario").FontSize(13).Bold().FontColor("#1E5AA8");
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cd =>
                            {
                                cd.RelativeColumn(3); cd.RelativeColumn(1); cd.RelativeColumn(1);
                                cd.RelativeColumn(1); cd.RelativeColumn(1); cd.RelativeColumn(1); cd.RelativeColumn(1.3f);
                            });
                            void H(string t) => table.Cell().Background("#1E5AA8").Padding(4).Text(t).FontColor("#FFFFFF").FontSize(9).Bold();
                            H("Funcionario"); H("Total"); H("Exc."); H("Buena"); H("Reg."); H("Mala"); H("% Exc.");
                            foreach (var pf in r.PorFuncionario)
                            {
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(pf.Nombre).FontSize(9);
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(pf.Total.ToString()).FontSize(9);
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(pf.Excelente.ToString()).FontSize(9);
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(pf.Buena.ToString()).FontSize(9);
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(pf.Regular.ToString()).FontSize(9);
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(pf.Mala.ToString()).FontSize(9);
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text($"{pf.PorcentajeExcelente:0.0}%").FontSize(9);
                            }
                        });
                    }

                    // Detalle de calificaciones
                    if (r.Filtro.IncluirDetalle && r.Filas.Count > 0)
                    {
                        col.Item().Text("Detalle de calificaciones").FontSize(13).Bold().FontColor("#1E5AA8");
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cd =>
                            {
                                cd.RelativeColumn(1.6f); cd.RelativeColumn(2.2f); cd.RelativeColumn(2f);
                                cd.RelativeColumn(1.3f);
                                if (r.Filtro.IncluirComentarios) cd.RelativeColumn(3f);
                            });
                            void H(string t) => table.Cell().Background("#1E5AA8").Padding(4).Text(t).FontColor("#FFFFFF").FontSize(9).Bold();
                            H("Fecha"); H("Funcionario"); H("Cargo"); H("Calificación");
                            if (r.Filtro.IncluirComentarios) H("Comentario");
                            foreach (var fila in r.Filas)
                            {
                                string color = fila.ValorNum switch { 1 => "#2E7D32", 2 => "#66BB6A", 3 => "#F57C00", 4 => "#D32F2F", _ => "#212B36" };
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(fila.Fecha.ToString("dd/MM/yyyy HH:mm")).FontSize(8);
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(fila.Funcionario).FontSize(8);
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(fila.Cargo).FontSize(8);
                                table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(fila.Valor).FontSize(8).Bold().FontColor(color);
                                if (r.Filtro.IncluirComentarios)
                                    table.Cell().BorderBottom(0.5f).BorderColor("#D9E1EA").Padding(4).Text(string.IsNullOrWhiteSpace(fila.Comentario) ? "—" : fila.Comentario).FontSize(8);
                            }
                        });
                    }
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("GADICC Calificador — ").FontSize(8).FontColor("#6D7A8A");
                    t.Span("Página ").FontSize(8).FontColor("#6D7A8A");
                    t.CurrentPageNumber().FontSize(8).FontColor("#6D7A8A");
                    t.Span(" de ").FontSize(8).FontColor("#6D7A8A");
                    t.TotalPages().FontSize(8).FontColor("#6D7A8A");
                });
            });
        }).GeneratePdf(ruta);
    }

    // =====================================================================
    // EXPORTACIÓN EXCEL (ClosedXML)
    // =====================================================================
    public void ExportarExcel(ReporteResultado r, string ruta)
    {
        using var wb = new XLWorkbook();

        // Hoja resumen
        var ws = wb.Worksheets.Add("Resumen");
        ws.Cell("A1").Value = "GADICC Calificador — Reporte de Calificaciones";
        ws.Range("A1:E1").Merge();
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 14;
        ws.Cell("A1").Style.Font.FontColor = XLColor.FromHtml("#1E5AA8");

        ws.Cell("A3").Value = "Período:";
        ws.Cell("B3").Value = $"{r.Filtro.Desde:dd/MM/yyyy} - {r.Filtro.Hasta:dd/MM/yyyy}";
        ws.Cell("A4").Value = "Generado:";
        ws.Cell("B4").Value = r.GeneradoEn.ToString("dd/MM/yyyy HH:mm");
        if (r.FuncionarioUnico)
        {
            ws.Cell("A5").Value = "Funcionario:";
            ws.Cell("B5").Value = $"{r.NombreFuncionario} ({r.Filtro.CedulaFuncionario})";
        }

        ws.Cell("A7").Value = "Categoría";
        ws.Cell("B7").Value = "Cantidad";
        ws.Cell("C7").Value = "Porcentaje";
        ws.Range("A7:C7").Style.Font.Bold = true;
        ws.Range("A7:C7").Style.Fill.BackgroundColor = XLColor.FromHtml("#1E5AA8");
        ws.Range("A7:C7").Style.Font.FontColor = XLColor.White;

        var filas = new (string, int, double)[]
        {
            ("Total", r.Total, 100),
            ("Excelente", r.Excelente, r.PctExcelente),
            ("Buena", r.Buena, r.PctBuena),
            ("Regular", r.Regular, r.PctRegular),
            ("Mala", r.Mala, r.PctMala),
        };
        int row = 8;
        foreach (var (cat, val, pct) in filas)
        {
            ws.Cell(row, 1).Value = cat;
            ws.Cell(row, 2).Value = val;
            ws.Cell(row, 3).Value = Math.Round(pct, 1) / 100.0;
            ws.Cell(row, 3).Style.NumberFormat.Format = "0.0%";
            row++;
        }
        ws.Columns().AdjustToContents();

        // Hoja por funcionario
        if (r.PorFuncionario.Count > 0)
        {
            var wf = wb.Worksheets.Add("Por funcionario");
            var head = new[] { "Cédula", "Funcionario", "Cargo", "Total", "Excelente", "Buena", "Regular", "Mala", "% Excelente", "Índice satisfacción" };
            for (int i = 0; i < head.Length; i++)
            {
                wf.Cell(1, i + 1).Value = head[i];
                wf.Cell(1, i + 1).Style.Font.Bold = true;
                wf.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1E5AA8");
                wf.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
            }
            int rr = 2;
            foreach (var f in r.PorFuncionario)
            {
                wf.Cell(rr, 1).Value = f.Cedula;
                wf.Cell(rr, 2).Value = f.Nombre;
                wf.Cell(rr, 3).Value = f.Cargo;
                wf.Cell(rr, 4).Value = f.Total;
                wf.Cell(rr, 5).Value = f.Excelente;
                wf.Cell(rr, 6).Value = f.Buena;
                wf.Cell(rr, 7).Value = f.Regular;
                wf.Cell(rr, 8).Value = f.Mala;
                wf.Cell(rr, 9).Value = Math.Round(f.PorcentajeExcelente, 1) / 100.0;
                wf.Cell(rr, 9).Style.NumberFormat.Format = "0.0%";
                wf.Cell(rr, 10).Value = Math.Round(f.IndiceSatisfaccion, 1);
                rr++;
            }
            wf.Columns().AdjustToContents();
        }

        // Hoja detalle
        if (r.Filtro.IncluirDetalle && r.Filas.Count > 0)
        {
            var wd = wb.Worksheets.Add("Detalle");
            var head = r.Filtro.IncluirComentarios
                ? new[] { "Fecha", "Cédula", "Funcionario", "Cargo", "Calificación", "Comentario" }
                : new[] { "Fecha", "Cédula", "Funcionario", "Cargo", "Calificación" };
            for (int i = 0; i < head.Length; i++)
            {
                wd.Cell(1, i + 1).Value = head[i];
                wd.Cell(1, i + 1).Style.Font.Bold = true;
                wd.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1E5AA8");
                wd.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
            }
            int rr = 2;
            foreach (var f in r.Filas)
            {
                wd.Cell(rr, 1).Value = f.Fecha;
                wd.Cell(rr, 1).Style.NumberFormat.Format = "dd/MM/yyyy HH:mm";
                wd.Cell(rr, 2).Value = f.Cedula;
                wd.Cell(rr, 3).Value = f.Funcionario;
                wd.Cell(rr, 4).Value = f.Cargo;
                wd.Cell(rr, 5).Value = f.Valor;
                if (r.Filtro.IncluirComentarios)
                    wd.Cell(rr, 6).Value = string.IsNullOrWhiteSpace(f.Comentario) ? "—" : f.Comentario;
                rr++;
            }
            wd.Columns().AdjustToContents();
        }

        wb.SaveAs(ruta);
    }
}
