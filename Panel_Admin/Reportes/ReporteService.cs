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
    private const string Azul = "#1C4A8A";
    private const string Gris = "#6D7A8A";
    private const string Borde = "#D9E1EA";

    private static string ColorValor(int v) => v switch
    { 1 => "#2E7D32", 2 => "#66BB6A", 3 => "#F57C00", 4 => "#D32F2F", _ => "#212B36" };

    public void ExportarPdf(ReporteResultado r, string ruta)
    {
        Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(42);
                page.DefaultTextStyle(t => t.FontSize(10).FontColor("#212B36").FontFamily("Segoe UI"));

                // ===== Encabezado =====
                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("GADICC Calificador").FontSize(20).Bold().FontColor(Azul);
                            c.Item().PaddingTop(2).Text("Reporte de Calificaciones de Funcionarios").FontSize(11).FontColor(Gris);
                        });
                        row.ConstantItem(170).AlignRight().AlignBottom().Text($"Generado: {r.GeneradoEn:dd/MM/yyyy HH:mm}")
                            .FontSize(8.5f).FontColor(Gris);
                    });
                    col.Item().PaddingTop(10).LineHorizontal(2).LineColor(Azul);
                });

                // ===== Contenido =====
                page.Content().PaddingTop(20).Column(col =>
                {
                    col.Spacing(22);

                    // --- Período y filtros ---
                    col.Item().Background("#EAF2FF").Padding(14).Column(c =>
                    {
                        c.Spacing(3);
                        c.Item().Text(t =>
                        {
                            t.Span("Período:  ").SemiBold().FontColor(Azul);
                            t.Span($"{r.Filtro.Desde:dd/MM/yyyy} — {r.Filtro.Hasta:dd/MM/yyyy}");
                        });
                        c.Item().Text(t =>
                        {
                            t.Span("Alcance:  ").SemiBold().FontColor(Azul);
                            t.Span(r.FuncionarioUnico ? $"{r.NombreFuncionario} (Cédula: {r.Filtro.CedulaFuncionario})" : "Todos los funcionarios");
                        });
                        if (!string.IsNullOrEmpty(r.Filtro.Cargo))
                            c.Item().Text(t => { t.Span("Cargo:  ").SemiBold().FontColor(Azul); t.Span(r.Filtro.Cargo!); });
                        if (r.Filtro.Tipo.HasValue)
                            c.Item().Text(t => { t.Span("Tipo:  ").SemiBold().FontColor(Azul); t.Span(r.Filtro.Tipo.Value.ToString()); });
                    });

                    // --- Resumen general ---
                    col.Item().Column(sec =>
                    {
                        Titulo(sec, "Resumen general");
                        sec.Item().PaddingTop(10).Row(row =>
                        {
                            void Tarjeta(string titulo, int valor, double pct, string color, bool ultima = false)
                            {
                                var item = row.RelativeItem();
                                item.Border(1).BorderColor(Borde).Background("#FFFFFF").Padding(12).Column(c =>
                                {
                                    c.Spacing(4);
                                    c.Item().Text(titulo.ToUpper()).FontSize(8).FontColor(Gris).LetterSpacing(0.05f);
                                    c.Item().Text(valor.ToString()).FontSize(26).Bold().FontColor(color);
                                    c.Item().Text($"{pct:0.0}%").FontSize(9).FontColor(Gris);
                                });
                                if (!ultima) row.ConstantItem(10);
                            }
                            Tarjeta("Total", r.Total, 100, Azul);
                            Tarjeta("Excelente", r.Excelente, r.PctExcelente, "#2E7D32");
                            Tarjeta("Buena", r.Buena, r.PctBuena, "#66BB6A");
                            Tarjeta("Regular", r.Regular, r.PctRegular, "#F57C00");
                            Tarjeta("Mala", r.Mala, r.PctMala, "#D32F2F", ultima: true);
                        });
                    });

                    // --- Detalle del funcionario o ranking ---
                    if (r.FuncionarioUnico && r.PorFuncionario.Count > 0)
                    {
                        var f = r.PorFuncionario[0];
                        col.Item().Column(sec =>
                        {
                            Titulo(sec, "Detalle del funcionario");
                            sec.Item().PaddingTop(10).Border(1).BorderColor(Borde).Padding(14).Column(c =>
                            {
                                c.Spacing(6);
                                c.Item().Text(t => { t.Span("Nombre:  ").SemiBold(); t.Span(f.Nombre); });
                                c.Item().Text(t => { t.Span("Cédula:  ").SemiBold(); t.Span(f.Cedula); t.Span("      Cargo:  ").SemiBold(); t.Span(f.Cargo); });
                                c.Item().Text(t => { t.Span("Total de calificaciones:  ").SemiBold(); t.Span(f.Total.ToString()); });
                                c.Item().Text(t => { t.Span("Índice de satisfacción:  ").SemiBold(); t.Span($"{f.IndiceSatisfaccion:0.0} / 100").FontColor(Azul).Bold(); });
                            });
                        });
                    }
                    else if (r.PorFuncionario.Count > 0)
                    {
                        col.Item().Column(sec =>
                        {
                            Titulo(sec, "Resumen por funcionario");
                            sec.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3); cd.RelativeColumn(2.2f); cd.RelativeColumn(1);
                                    cd.RelativeColumn(1); cd.RelativeColumn(1); cd.RelativeColumn(1); cd.RelativeColumn(1.3f);
                                });
                                foreach (var h in new[] { "Funcionario", "Cargo", "Total", "Exc.", "Buena", "Reg.", "Mala" })
                                    table.Cell().Background(Azul).PaddingVertical(7).PaddingHorizontal(6).Text(h).FontColor("#FFFFFF").FontSize(9).Bold();
                                table.Cell().Background(Azul).PaddingVertical(7).PaddingHorizontal(6).Text("% Exc.").FontColor("#FFFFFF").FontSize(9).Bold();
                                bool alt = false;
                                foreach (var pf in r.PorFuncionario)
                                {
                                    string bg = alt ? "#F5F8FC" : "#FFFFFF"; alt = !alt;
                                    void C(string txt) => table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(Borde).PaddingVertical(6).PaddingHorizontal(6).Text(txt).FontSize(9);
                                    C(pf.Nombre); C(pf.Cargo); C(pf.Total.ToString()); C(pf.Excelente.ToString());
                                    C(pf.Buena.ToString()); C(pf.Regular.ToString()); C(pf.Mala.ToString());
                                }
                            });
                        });
                    }

                    // --- Detalle de calificaciones ---
                    if (r.Filtro.IncluirDetalle && r.Filas.Count > 0)
                    {
                        col.Item().Column(sec =>
                        {
                            Titulo(sec, "Detalle de calificaciones");
                            sec.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(1.7f); cd.RelativeColumn(2.2f); cd.RelativeColumn(2f);
                                    cd.RelativeColumn(1.4f);
                                    if (r.Filtro.IncluirComentarios) cd.RelativeColumn(3f);
                                });
                                var heads = r.Filtro.IncluirComentarios
                                    ? new[] { "Fecha", "Funcionario", "Cargo", "Calificación", "Comentario" }
                                    : new[] { "Fecha", "Funcionario", "Cargo", "Calificación" };
                                foreach (var h in heads)
                                    table.Cell().Background(Azul).PaddingVertical(7).PaddingHorizontal(6).Text(h).FontColor("#FFFFFF").FontSize(9).Bold();
                                bool alt = false;
                                foreach (var fila in r.Filas)
                                {
                                    string bg = alt ? "#F5F8FC" : "#FFFFFF"; alt = !alt;
                                    table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(Borde).PaddingVertical(6).PaddingHorizontal(6).Text(fila.Fecha.ToString("dd/MM/yyyy HH:mm")).FontSize(8.5f);
                                    table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(Borde).PaddingVertical(6).PaddingHorizontal(6).Text(fila.Funcionario).FontSize(8.5f);
                                    table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(Borde).PaddingVertical(6).PaddingHorizontal(6).Text(fila.Cargo).FontSize(8.5f);
                                    table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(Borde).PaddingVertical(6).PaddingHorizontal(6).Text(fila.Valor).FontSize(8.5f).Bold().FontColor(ColorValor(fila.ValorNum));
                                    if (r.Filtro.IncluirComentarios)
                                        table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(Borde).PaddingVertical(6).PaddingHorizontal(6).Text(string.IsNullOrWhiteSpace(fila.Comentario) ? "—" : fila.Comentario).FontSize(8.5f);
                                }
                            });
                        });
                    }
                });

                // ===== Pie de página =====
                page.Footer().PaddingTop(8).Column(f =>
                {
                    f.Item().LineHorizontal(0.5f).LineColor(Borde);
                    f.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Text("GADICC Calificador · Municipio Intercultural de Cañar").FontSize(8).FontColor(Gris);
                        row.RelativeItem().AlignRight().Text(t =>
                        {
                            t.Span("Página ").FontSize(8).FontColor(Gris);
                            t.CurrentPageNumber().FontSize(8).FontColor(Gris);
                            t.Span(" de ").FontSize(8).FontColor(Gris);
                            t.TotalPages().FontSize(8).FontColor(Gris);
                        });
                    });
                });
            });
        }).GeneratePdf(ruta);
    }

    // Título de sección con barra azul a la izquierda
    private static void Titulo(QuestPDF.Fluent.ColumnDescriptor col, string texto)
    {
        col.Item().Row(r =>
        {
            r.ConstantItem(4).Background(Azul);
            r.ConstantItem(9);
            r.RelativeItem().AlignMiddle().Text(texto).FontSize(14).Bold().FontColor(Azul);
        });
    }

    // =====================================================================
    // EXPORTACIÓN EXCEL (ClosedXML)
    // =====================================================================
    public void ExportarExcel(ReporteResultado r, string ruta)
    {
        using var wb = new XLWorkbook();
        var azul = XLColor.FromHtml("#1C4A8A");
        var azulClaro = XLColor.FromHtml("#EAF2FF");
        var gris = XLColor.FromHtml("#F5F8FC");
        var bordeCol = XLColor.FromHtml("#D9E1EA");

        // ============ HOJA RESUMEN ============
        var ws = wb.Worksheets.Add("Resumen");
        ws.ShowGridLines = false;

        // Título
        ws.Cell("A1").Value = "GADICC Calificador";
        ws.Range("A1:C1").Merge();
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 18;
        ws.Cell("A1").Style.Font.FontColor = azul;
        ws.Cell("A2").Value = "Reporte de Calificaciones de Funcionarios";
        ws.Range("A2:C2").Merge();
        ws.Cell("A2").Style.Font.FontSize = 11;
        ws.Cell("A2").Style.Font.FontColor = XLColor.FromHtml("#6D7A8A");

        // Datos del reporte
        ws.Cell("A4").Value = "Período:";  ws.Cell("A4").Style.Font.Bold = true;
        ws.Cell("B4").Value = $"{r.Filtro.Desde:dd/MM/yyyy} — {r.Filtro.Hasta:dd/MM/yyyy}";
        ws.Cell("A5").Value = "Alcance:";  ws.Cell("A5").Style.Font.Bold = true;
        ws.Cell("B5").Value = r.FuncionarioUnico ? $"{r.NombreFuncionario} ({r.Filtro.CedulaFuncionario})" : "Todos los funcionarios";
        ws.Cell("A6").Value = "Generado:"; ws.Cell("A6").Style.Font.Bold = true;
        ws.Cell("B6").Value = r.GeneradoEn.ToString("dd/MM/yyyy HH:mm");

        // Tabla resumen
        int hr = 8;
        var heads = new[] { "Categoría", "Cantidad", "Porcentaje" };
        for (int i = 0; i < heads.Length; i++)
        {
            var cell = ws.Cell(hr, i + 1);
            cell.Value = heads[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = azul;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
        var filas = new (string cat, int val, double pct, string color)[]
        {
            ("Total", r.Total, 100, "#1C4A8A"),
            ("Excelente", r.Excelente, r.PctExcelente, "#2E7D32"),
            ("Buena", r.Buena, r.PctBuena, "#66BB6A"),
            ("Regular", r.Regular, r.PctRegular, "#F57C00"),
            ("Mala", r.Mala, r.PctMala, "#D32F2F"),
        };
        int rw = hr + 1;
        foreach (var (cat, val, pct, color) in filas)
        {
            ws.Cell(rw, 1).Value = cat;
            ws.Cell(rw, 1).Style.Font.FontColor = XLColor.FromHtml(color);
            ws.Cell(rw, 1).Style.Font.Bold = true;
            ws.Cell(rw, 2).Value = val;
            ws.Cell(rw, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(rw, 3).Value = Math.Round(pct, 1) / 100.0;
            ws.Cell(rw, 3).Style.NumberFormat.Format = "0.0%";
            ws.Cell(rw, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            if ((rw - hr) % 2 == 0) ws.Range(rw, 1, rw, 3).Style.Fill.BackgroundColor = gris;
            rw++;
        }
        var rangoResumen = ws.Range(hr, 1, rw - 1, 3);
        rangoResumen.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        rangoResumen.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        rangoResumen.Style.Border.OutsideBorderColor = bordeCol;
        rangoResumen.Style.Border.InsideBorderColor = bordeCol;
        ws.Column(1).Width = 22; ws.Column(2).Width = 14; ws.Column(3).Width = 14;
        ws.Row(hr).Height = 20;

        // ============ HOJA POR FUNCIONARIO ============
        if (r.PorFuncionario.Count > 0)
        {
            var wf = wb.Worksheets.Add("Por funcionario");
            wf.ShowGridLines = false;
            var head = new[] { "Cédula", "Funcionario", "Cargo", "Total", "Excelente", "Buena", "Regular", "Mala", "% Excelente", "Índice satisfacción" };
            for (int i = 0; i < head.Length; i++)
            {
                var cell = wf.Cell(1, i + 1);
                cell.Value = head[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = azul;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
            wf.Row(1).Height = 22;
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
                for (int col = 4; col <= 10; col++) wf.Cell(rr, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                if (rr % 2 == 0) wf.Range(rr, 1, rr, head.Length).Style.Fill.BackgroundColor = gris;
                rr++;
            }
            var rango = wf.Range(1, 1, rr - 1, head.Length);
            rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rango.Style.Border.OutsideBorderColor = bordeCol;
            rango.Style.Border.InsideBorderColor = bordeCol;
            wf.Columns().AdjustToContents();
            wf.Column(2).Width = Math.Min(wf.Column(2).Width, 28);
            wf.Column(3).Width = Math.Min(wf.Column(3).Width, 26);
            wf.SheetView.FreezeRows(1);
            wf.RangeUsed()?.SetAutoFilter();
        }

        // ============ HOJA DETALLE ============
        if (r.Filtro.IncluirDetalle && r.Filas.Count > 0)
        {
            var wd = wb.Worksheets.Add("Detalle");
            wd.ShowGridLines = false;
            var head = r.Filtro.IncluirComentarios
                ? new[] { "Fecha", "Cédula", "Funcionario", "Cargo", "Calificación", "Comentario" }
                : new[] { "Fecha", "Cédula", "Funcionario", "Cargo", "Calificación" };
            for (int i = 0; i < head.Length; i++)
            {
                var cell = wd.Cell(1, i + 1);
                cell.Value = head[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = azul;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            wd.Row(1).Height = 22;
            int rr = 2;
            foreach (var f in r.Filas)
            {
                wd.Cell(rr, 1).Value = f.Fecha;
                wd.Cell(rr, 1).Style.NumberFormat.Format = "dd/MM/yyyy HH:mm";
                wd.Cell(rr, 2).Value = f.Cedula;
                wd.Cell(rr, 3).Value = f.Funcionario;
                wd.Cell(rr, 4).Value = f.Cargo;
                wd.Cell(rr, 5).Value = f.Valor;
                wd.Cell(rr, 5).Style.Font.FontColor = XLColor.FromHtml(ColorValor(f.ValorNum));
                wd.Cell(rr, 5).Style.Font.Bold = true;
                wd.Cell(rr, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                if (r.Filtro.IncluirComentarios)
                    wd.Cell(rr, 6).Value = string.IsNullOrWhiteSpace(f.Comentario) ? "—" : f.Comentario;
                if (rr % 2 == 0) wd.Range(rr, 1, rr, head.Length).Style.Fill.BackgroundColor = gris;
                rr++;
            }
            var rango = wd.Range(1, 1, rr - 1, head.Length);
            rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rango.Style.Border.OutsideBorderColor = bordeCol;
            rango.Style.Border.InsideBorderColor = bordeCol;
            wd.Columns().AdjustToContents();
            wd.Column(1).Width = 18;
            if (r.Filtro.IncluirComentarios) wd.Column(6).Width = Math.Min(Math.Max(wd.Column(6).Width, 30), 50);
            wd.SheetView.FreezeRows(1);
            wd.RangeUsed()?.SetAutoFilter();
        }

        wb.SaveAs(ruta);
    }
}
