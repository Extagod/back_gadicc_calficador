using System.Diagnostics;
using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;
using Capa_Abstracciones.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Panel_Admin.Reportes;
using Panel_Admin.UI;

namespace Panel_Admin.Views;

public class ReportesView : UserControl
{
    private readonly IServiceProvider _sp;
    private readonly ReporteService _reporteService = new();

    private List<Calificacion> _calificaciones = new();
    private List<Empleado> _funcionarios = new();
    private ReporteResultado? _ultimo;

    // Filtros
    private DateTimePicker _desde = null!, _hasta = null!;
    private ComboBox _cboFuncionario = null!, _cboCargo = null!, _cboTipo = null!;
    private CheckBox _chkTodos = null!, _chkComentarios = null!, _chkDetalle = null!;
    private UIButton _btnGenerar = null!, _btnPdf = null!, _btnExcel = null!, _btnImprimir = null!, _btnLimpiar = null!;

    // Vista previa
    private Panel _preview = null!;
    private MetricCard _cTotal = null!, _cExc = null!, _cBuena = null!, _cReg = null!, _cMala = null!;
    private Label _lblTituloPrev = null!, _lblPeriodo = null!, _lblVacio = null!;
    private DataGridView _gridPrev = null!;
    private LoadingOverlay _overlay = null!;

    public ReportesView(IServiceProvider serviceProvider)
    {
        _sp = serviceProvider;
        AutoScaleMode = AutoScaleMode.None;
        Dock = DockStyle.Fill;
        BackColor = UITheme.Background;
        ConstruirUI();
    }

    private void ConstruirUI()
    {
        // === Panel de filtros (izquierda) ===
        var panelFiltros = new RoundedPanel { Dock = DockStyle.Left, Width = 320, Radius = 12, Padding = new Padding(20, 18, 20, 18) };

        var lblF = new Label { Text = "Configuración del reporte", Font = UITheme.SectionTitle, ForeColor = UITheme.TextPrimary, AutoSize = false, Size = new Size(280, 26), Location = new Point(20, 18) };
        panelFiltros.Controls.Add(lblF);

        int x = 20, y = 58, w = 280;
        panelFiltros.Controls.Add(Mini("FECHA DESDE", x, y));
        _desde = new DateTimePicker { Location = new Point(x, y + 20), Size = new Size(w, 30), Format = DateTimePickerFormat.Short, Font = UITheme.Body };
        panelFiltros.Controls.Add(_desde);

        y += 62;
        panelFiltros.Controls.Add(Mini("FECHA HASTA", x, y));
        _hasta = new DateTimePicker { Location = new Point(x, y + 20), Size = new Size(w, 30), Format = DateTimePickerFormat.Short, Font = UITheme.Body };
        panelFiltros.Controls.Add(_hasta);

        y += 62;
        _chkTodos = new CheckBox { Text = "Todos los funcionarios", Checked = true, Font = UITheme.Body, ForeColor = UITheme.TextPrimary, AutoSize = true, Location = new Point(x, y) };
        _chkTodos.CheckedChanged += (_, _) => _cboFuncionario.Enabled = !_chkTodos.Checked;
        panelFiltros.Controls.Add(_chkTodos);

        y += 28;
        panelFiltros.Controls.Add(Mini("FUNCIONARIO", x, y));
        _cboFuncionario = Combo(x, y + 20, w); _cboFuncionario.Enabled = false;
        panelFiltros.Controls.Add(_cboFuncionario);

        y += 62;
        panelFiltros.Controls.Add(Mini("CARGO", x, y));
        _cboCargo = Combo(x, y + 20, w);
        panelFiltros.Controls.Add(_cboCargo);

        y += 62;
        panelFiltros.Controls.Add(Mini("TIPO DE CALIFICACIÓN", x, y));
        _cboTipo = Combo(x, y + 20, w);
        panelFiltros.Controls.Add(_cboTipo);

        y += 60;
        _chkComentarios = new CheckBox { Text = "Incluir comentarios", Checked = true, Font = UITheme.Body, ForeColor = UITheme.TextPrimary, AutoSize = true, Location = new Point(x, y) };
        panelFiltros.Controls.Add(_chkComentarios);
        y += 26;
        _chkDetalle = new CheckBox { Text = "Incluir detalle de calificaciones", Checked = true, Font = UITheme.Body, ForeColor = UITheme.TextPrimary, AutoSize = true, Location = new Point(x, y) };
        panelFiltros.Controls.Add(_chkDetalle);

        y += 40;
        _btnGenerar = new UIButton { Text = "Generar vista previa", BaseColor = UITheme.Primary, Size = new Size(w, 44), Location = new Point(x, y) };
        _btnGenerar.Click += (_, _) => GenerarPreview();
        panelFiltros.Controls.Add(_btnGenerar);

        y += 52;
        _btnLimpiar = new UIButton { Text = "Limpiar filtros", Outline = true, BaseColor = UITheme.Neutral, Size = new Size(w, 40), Location = new Point(x, y) };
        _btnLimpiar.Click += (_, _) => LimpiarFiltros();
        panelFiltros.Controls.Add(_btnLimpiar);

        // === Área de vista previa (derecha) ===
        var contPreview = new RoundedPanel { Dock = DockStyle.Fill, Radius = 12, Padding = new Padding(20, 16, 20, 16), Margin = new Padding(12, 0, 0, 0) };

        // Barra de acciones de exportación
        var barraExport = new Panel { Dock = DockStyle.Top, Height = 54, BackColor = UITheme.Card };
        _lblTituloPrev = new Label { Text = "Vista previa del reporte", Font = UITheme.SectionTitle, ForeColor = UITheme.TextPrimary, AutoSize = false, Size = new Size(360, 26), Location = new Point(0, 6) };
        _lblPeriodo = new Label { Text = "Configure los filtros y genere la vista previa", Font = UITheme.Small, ForeColor = UITheme.TextSecondary, AutoSize = false, Size = new Size(360, 18), Location = new Point(0, 32) };

        _btnPdf = new UIButton { Text = "PDF", BaseColor = UITheme.Danger, Size = new Size(90, 40), Anchor = AnchorStyles.Top | AnchorStyles.Right, Enabled = false };
        _btnExcel = new UIButton { Text = "Excel", BaseColor = UITheme.Success, Size = new Size(90, 40), Anchor = AnchorStyles.Top | AnchorStyles.Right, Enabled = false };
        _btnImprimir = new UIButton { Text = "Imprimir", Outline = true, BaseColor = UITheme.Primary, Size = new Size(110, 40), Anchor = AnchorStyles.Top | AnchorStyles.Right, Enabled = false };
        _btnPdf.Click += async (_, _) => await ExportarPdf();
        _btnExcel.Click += async (_, _) => await ExportarExcel();
        _btnImprimir.Click += async (_, _) => await Imprimir();
        barraExport.Controls.AddRange(new Control[] { _lblTituloPrev, _lblPeriodo, _btnImprimir, _btnPdf, _btnExcel });
        barraExport.Resize += (_, _) =>
        {
            _btnExcel.Location = new Point(barraExport.Width - 90, 6);
            _btnPdf.Location = new Point(barraExport.Width - 188, 6);
            _btnImprimir.Location = new Point(barraExport.Width - 306, 6);
        };

        // Tarjetas resumen
        var cards = new TableLayoutPanel { Dock = DockStyle.Top, Height = 162, ColumnCount = 5, RowCount = 1, BackColor = UITheme.Card, Margin = new Padding(0, 8, 0, 0) };
        for (int i = 0; i < 5; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
        _cTotal = new MetricCard("Total", UITheme.Primary, "📊") { Dock = DockStyle.Fill, Margin = new Padding(0, 8, 6, 0) };
        _cExc = new MetricCard("Excelente", UITheme.Excelente, "★") { Dock = DockStyle.Fill, Margin = new Padding(6, 8, 6, 0) };
        _cBuena = new MetricCard("Buena", UITheme.Buena, "👍") { Dock = DockStyle.Fill, Margin = new Padding(6, 8, 6, 0) };
        _cReg = new MetricCard("Regular", UITheme.Regular, "≈") { Dock = DockStyle.Fill, Margin = new Padding(6, 8, 6, 0) };
        _cMala = new MetricCard("Mala", UITheme.Mala, "▼") { Dock = DockStyle.Fill, Margin = new Padding(6, 8, 0, 0) };
        cards.Controls.Add(_cTotal, 0, 0); cards.Controls.Add(_cExc, 1, 0); cards.Controls.Add(_cBuena, 2, 0);
        cards.Controls.Add(_cReg, 3, 0); cards.Controls.Add(_cMala, 4, 0);

        // Grid de detalle
        _gridPrev = new DataGridView { Dock = DockStyle.Fill };
        GridStyler.Apply(_gridPrev);

        _lblVacio = new Label
        {
            Text = "No hay datos para los filtros seleccionados.\nAjuste el rango de fechas o los filtros y vuelva a generar.",
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = UITheme.Body,
            ForeColor = UITheme.TextMuted, Visible = false
        };

        _preview = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.Card };
        _preview.Controls.Add(_gridPrev);
        _preview.Controls.Add(_lblVacio);

        var spCards = new Panel { Dock = DockStyle.Top, Height = 8, BackColor = UITheme.Card };

        contPreview.Controls.Add(_preview);
        contPreview.Controls.Add(spCards);
        contPreview.Controls.Add(cards);
        contPreview.Controls.Add(barraExport);

        _overlay = new LoadingOverlay("Generando reporte...");

        Controls.Add(contPreview);
        Controls.Add(panelFiltros);
        Controls.Add(_overlay);
    }

    private static Label Mini(string t, int x, int y) => new()
    {
        Text = t, Font = new Font(UITheme.FontFamilySemibold, 8f, FontStyle.Bold),
        ForeColor = UITheme.TextSecondary, AutoSize = true, Location = new Point(x, y)
    };
    private static ComboBox Combo(int x, int y, int w) => new()
    {
        Location = new Point(x, y), Size = new Size(w, 30), DropDownStyle = ComboBoxStyle.DropDownList,
        FlatStyle = FlatStyle.Flat, Font = UITheme.Body
    };

    public async void Inicializar()
    {
        // Siempre recarga datos frescos al entrar a la sección (evita datos en caché desactualizados)
        _overlay.Mostrar("Cargando datos...");
        try
        {
            using var scope = _sp.CreateScope();
            var calSvc = scope.ServiceProvider.GetRequiredService<ICalificacionService>();
            var encSvc = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
            var cal = await calSvc.ObtenerTodasAsync();
            var enc = await encSvc.ObtenerTodosAsync();
            _calificaciones = cal.IsSuccess ? cal.Value!.ToList() : new();
            _funcionarios = enc.IsSuccess ? enc.Value!.ToList() : new();
            CargarCombos();
        }
        catch (Exception)
        {
            Notificador.Error(FindForm(), "No se pudieron cargar los datos para reportes.");
        }
        finally { _overlay.Ocultar(); }
    }

    private void CargarCombos()
    {
        _cboFuncionario.Items.Clear();
        foreach (var f in _funcionarios)
            _cboFuncionario.Items.Add($"{f.Persona?.PrimerNombre} {f.Persona?.PrimerApellido}".Trim());
        if (_cboFuncionario.Items.Count > 0) _cboFuncionario.SelectedIndex = 0;

        _cboCargo.Items.Clear();
        _cboCargo.Items.Add("Todos los cargos");
        foreach (var c in _funcionarios.Select(f => f.Cargo).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().OrderBy(c => c))
            _cboCargo.Items.Add(c!);
        _cboCargo.SelectedIndex = 0;

        _cboTipo.Items.Clear();
        _cboTipo.Items.AddRange(new object[] { "Todas", "Excelente", "Buena", "Regular", "Mala" });
        _cboTipo.SelectedIndex = 0;

        if (_calificaciones.Count > 0)
        {
            _desde.Value = _calificaciones.Min(c => c.FechaHora).Date;
            _hasta.Value = _calificaciones.Max(c => c.FechaHora).Date;
        }
        else
        {
            _desde.Value = DateTime.Now.AddDays(-30);
            _hasta.Value = DateTime.Now;
        }
    }

    private void LimpiarFiltros()
    {
        _chkTodos.Checked = true;
        _chkComentarios.Checked = true;
        _chkDetalle.Checked = true;
        if (_cboCargo.Items.Count > 0) _cboCargo.SelectedIndex = 0;
        if (_cboTipo.Items.Count > 0) _cboTipo.SelectedIndex = 0;
        CargarCombos();
    }

    private ReporteFiltro? ConstruirFiltro()
    {
        if (_desde.Value.Date > _hasta.Value.Date)
        {
            Notificador.Advertencia(FindForm(), "La fecha 'desde' no puede ser mayor que la fecha 'hasta'.");
            return null;
        }
        string? cedula = null;
        if (!_chkTodos.Checked && _cboFuncionario.SelectedIndex >= 0)
            cedula = _funcionarios[_cboFuncionario.SelectedIndex].CedulaRucPersona;

        return new ReporteFiltro
        {
            Desde = _desde.Value.Date,
            Hasta = _hasta.Value.Date,
            CedulaFuncionario = cedula,
            Cargo = _cboCargo.SelectedIndex > 0 ? _cboCargo.SelectedItem?.ToString() : null,
            Tipo = _cboTipo.SelectedIndex > 0 ? (ValorCalificacion)_cboTipo.SelectedIndex : null,
            IncluirComentarios = _chkComentarios.Checked,
            IncluirDetalle = _chkDetalle.Checked
        };
    }

    private void GenerarPreview()
    {
        var filtro = ConstruirFiltro();
        if (filtro is null) return;

        _ultimo = _reporteService.Construir(filtro, _calificaciones, _funcionarios);

        _cTotal.Valor = _ultimo.Total.ToString();
        _cExc.Valor = _ultimo.Excelente.ToString();
        _cBuena.Valor = _ultimo.Buena.ToString();
        _cReg.Valor = _ultimo.Regular.ToString();
        _cMala.Valor = _ultimo.Mala.ToString();

        _lblTituloPrev.Text = _ultimo.FuncionarioUnico ? $"Reporte — {_ultimo.NombreFuncionario}" : "Reporte — Todos los funcionarios";
        _lblPeriodo.Text = $"Período {filtro.Desde:dd/MM/yyyy} a {filtro.Hasta:dd/MM/yyyy}  ·  Generado {_ultimo.GeneradoEn:dd/MM/yyyy HH:mm}";

        if (!_ultimo.TieneDatos)
        {
            _gridPrev.Visible = false;
            _lblVacio.Visible = true; _lblVacio.BringToFront();
            _btnPdf.Enabled = _btnExcel.Enabled = _btnImprimir.Enabled = false;
            Notificador.Advertencia(FindForm(), "No se encontraron calificaciones con los filtros seleccionados.");
            return;
        }

        _lblVacio.Visible = false;
        _gridPrev.Visible = true;
        RenderGrid();
        _btnPdf.Enabled = _btnExcel.Enabled = _btnImprimir.Enabled = true;
    }

    private void RenderGrid()
    {
        if (_ultimo is null) return;
        _gridPrev.Columns.Clear();
        _gridPrev.AutoGenerateColumns = false;

        if (!_ultimo.FuncionarioUnico && _cboFuncionario_TodosSeleccionado())
        {
            // Ranking por funcionario
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Funcionario", Name = "n", FillWeight = 28 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cargo", Name = "c", FillWeight = 20 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", Name = "t", FillWeight = 10 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Exc.", Name = "e", FillWeight = 9 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Buena", Name = "b", FillWeight = 9 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Reg.", Name = "r", FillWeight = 9 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mala", Name = "m", FillWeight = 9 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "% Exc.", Name = "p", FillWeight = 10 });
            _gridPrev.Rows.Clear();
            foreach (var f in _ultimo.PorFuncionario)
                _gridPrev.Rows.Add(f.Nombre, f.Cargo, f.Total, f.Excelente, f.Buena, f.Regular, f.Mala, $"{f.PorcentajeExcelente:0.0}%");
        }
        else
        {
            // Detalle
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha", Name = "f", FillWeight = 16 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Funcionario", Name = "n", FillWeight = 22 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cargo", Name = "c", FillWeight = 18 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Calificación", Name = "v", FillWeight = 14 });
            _gridPrev.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Comentario", Name = "co", FillWeight = 30 });
            _gridPrev.Columns["v"].DefaultCellStyle.Font = new Font(UITheme.FontFamilySemibold, 9f, FontStyle.Bold);
            _gridPrev.Rows.Clear();
            foreach (var fila in _ultimo.Filas)
            {
                int idx = _gridPrev.Rows.Add(fila.Fecha.ToString("dd/MM/yyyy HH:mm"), fila.Funcionario, fila.Cargo, fila.Valor,
                    string.IsNullOrWhiteSpace(fila.Comentario) ? "—" : fila.Comentario);
                var color = UITheme.ColorCalificacion(fila.ValorNum);
                _gridPrev.Rows[idx].Cells["v"].Style.ForeColor = color;
                _gridPrev.Rows[idx].Cells["v"].Style.SelectionForeColor = color;
            }
        }
    }

    private bool _cboFuncionario_TodosSeleccionado() => _chkTodos.Checked;

    private async Task ExportarPdf()
    {
        if (_ultimo is null || !_ultimo.TieneDatos) return;
        using var dlg = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = _reporteService.NombreArchivo(_ultimo, "pdf") };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        _overlay.Mostrar("Generando PDF...");
        try
        {
            await Task.Run(() => _reporteService.ExportarPdf(_ultimo, dlg.FileName));
            _overlay.Ocultar();
            Notificador.Exito(FindForm(), "Reporte PDF generado correctamente.");
        }
        catch (Exception ex)
        {
            _overlay.Ocultar();
            Notificador.Error(FindForm(), "No se pudo generar el PDF: " + ex.Message);
        }
    }

    private async Task ExportarExcel()
    {
        if (_ultimo is null || !_ultimo.TieneDatos) return;
        using var dlg = new SaveFileDialog { Filter = "Excel|*.xlsx", FileName = _reporteService.NombreArchivo(_ultimo, "xlsx") };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        _overlay.Mostrar("Generando Excel...");
        try
        {
            await Task.Run(() => _reporteService.ExportarExcel(_ultimo, dlg.FileName));
            _overlay.Ocultar();
            Notificador.Exito(FindForm(), "Reporte Excel generado correctamente.");
        }
        catch (Exception ex)
        {
            _overlay.Ocultar();
            Notificador.Error(FindForm(), "No se pudo generar el Excel: " + ex.Message);
        }
    }

    private async Task Imprimir()
    {
        if (_ultimo is null || !_ultimo.TieneDatos) return;
        _overlay.Mostrar("Preparando impresión...");
        try
        {
            var temp = Path.Combine(Path.GetTempPath(), _reporteService.NombreArchivo(_ultimo, "pdf"));
            await Task.Run(() => _reporteService.ExportarPdf(_ultimo, temp));
            _overlay.Ocultar();
            Process.Start(new ProcessStartInfo { FileName = temp, UseShellExecute = true });
            Notificador.Info(FindForm(), "Se abrió el reporte en el visor de PDF. Use la opción Imprimir del visor.");
        }
        catch (Exception ex)
        {
            _overlay.Ocultar();
            Notificador.Error(FindForm(), "No se pudo preparar la impresión: " + ex.Message);
        }
    }
}
