using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;
using Capa_Abstracciones.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Panel_Admin.UI;

namespace Panel_Admin.Views;

public class CalificacionesView : UserControl
{
    private readonly IServiceProvider _sp;
    private List<Calificacion> _todas = new();
    private List<Empleado> _funcionarios = new();

    private MetricCard _cardTotal = null!, _cardExc = null!, _cardBuena = null!, _cardReg = null!, _cardMala = null!;
    private DateTimePicker _desde = null!, _hasta = null!;
    private ComboBox _cboFuncionario = null!, _cboTipo = null!;
    private UITextBox _txtBuscar = null!;
    private UIButton _btnAplicar = null!, _btnLimpiar = null!;
    private DataGridView _grid = null!;
    private LoadingOverlay _overlay = null!;

    public CalificacionesView(IServiceProvider serviceProvider)
    {
        _sp = serviceProvider;
        AutoScaleMode = AutoScaleMode.None;
        Dock = DockStyle.Fill;
        BackColor = UITheme.Background;
        ConstruirUI();
    }

    private void ConstruirUI()
    {
        // === Fila de tarjetas ===
        var cards = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 160,
            ColumnCount = 5,
            RowCount = 1,
            BackColor = UITheme.Background
        };
        for (int i = 0; i < 5; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));

        _cardTotal = new MetricCard("Total calificaciones", UITheme.Primary, "📊") { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 8, 0) };
        _cardExc = new MetricCard("Excelente", UITheme.Excelente, "★") { Dock = DockStyle.Fill, Margin = new Padding(8, 0, 8, 0) };
        _cardBuena = new MetricCard("Buena", UITheme.Buena, "👍") { Dock = DockStyle.Fill, Margin = new Padding(8, 0, 8, 0) };
        _cardReg = new MetricCard("Regular", UITheme.Regular, "≈") { Dock = DockStyle.Fill, Margin = new Padding(8, 0, 8, 0) };
        _cardMala = new MetricCard("Mala", UITheme.Mala, "▼") { Dock = DockStyle.Fill, Margin = new Padding(8, 0, 0, 0) };
        cards.Controls.Add(_cardTotal, 0, 0);
        cards.Controls.Add(_cardExc, 1, 0);
        cards.Controls.Add(_cardBuena, 2, 0);
        cards.Controls.Add(_cardReg, 3, 0);
        cards.Controls.Add(_cardMala, 4, 0);

        // === Barra de filtros ===
        var filtros = new RoundedPanel { Dock = DockStyle.Top, Height = 92, Radius = 12, Margin = new Padding(0, 12, 0, 0) };

        var lblDesde = MiniLabel("DESDE", 18, 12); filtros.Controls.Add(lblDesde);
        _desde = NuevoDate(18, 32); filtros.Controls.Add(_desde);

        var lblHasta = MiniLabel("HASTA", 170, 12); filtros.Controls.Add(lblHasta);
        _hasta = NuevoDate(170, 32); filtros.Controls.Add(_hasta);

        var lblFunc = MiniLabel("FUNCIONARIO", 322, 12); filtros.Controls.Add(lblFunc);
        _cboFuncionario = NuevoCombo(322, 32, 210); filtros.Controls.Add(_cboFuncionario);

        var lblTipo = MiniLabel("CALIFICACIÓN", 544, 12); filtros.Controls.Add(lblTipo);
        _cboTipo = NuevoCombo(544, 32, 150); filtros.Controls.Add(_cboTipo);

        var lblBuscar = MiniLabel("BUSCAR", 706, 12); filtros.Controls.Add(lblBuscar);
        _txtBuscar = new UITextBox { Location = new Point(706, 30), Size = new Size(220, 38), PlaceholderText = "Comentario o funcionario..." };
        filtros.Controls.Add(_txtBuscar);

        _btnAplicar = new UIButton { Text = "Aplicar", BaseColor = UITheme.Primary, Size = new Size(110, 38), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        _btnAplicar.Location = new Point(filtros.Width - 244, 30);
        _btnAplicar.Click += (_, _) => AplicarFiltros();
        filtros.Controls.Add(_btnAplicar);

        _btnLimpiar = new UIButton { Text = "Limpiar", Outline = true, BaseColor = UITheme.Neutral, Size = new Size(110, 38), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        _btnLimpiar.Location = new Point(filtros.Width - 126, 30);
        _btnLimpiar.Click += (_, _) => LimpiarFiltros();
        filtros.Controls.Add(_btnLimpiar);

        // === Tabla ===
        var cardGrid = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(2), Radius = 12, Margin = new Padding(0, 12, 0, 0) };
        _grid = new DataGridView { Dock = DockStyle.Fill };
        GridStyler.Apply(_grid);
        ConstruirColumnas();
        cardGrid.Controls.Add(_grid);

        _overlay = new LoadingOverlay("Cargando calificaciones...");

        // Layout raíz determinista con alturas fijas (evita que las tarjetas se estiren)
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = UITheme.Background
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));  // tarjetas
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));  // filtros
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // tabla

        cards.Dock = DockStyle.Fill;
        cards.Margin = new Padding(0, 0, 0, 0);
        filtros.Dock = DockStyle.Fill;
        filtros.Margin = new Padding(0, 12, 0, 0);
        cardGrid.Dock = DockStyle.Fill;
        cardGrid.Margin = new Padding(0, 12, 0, 0);

        root.Controls.Add(cards, 0, 0);
        root.Controls.Add(filtros, 0, 1);
        root.Controls.Add(cardGrid, 0, 2);

        Controls.Add(root);
        Controls.Add(_overlay);
        _overlay.BringToFront();
    }

    private static Label MiniLabel(string t, int x, int y) => new()
    {
        Text = t, Font = new Font(UITheme.FontFamilySemibold, 8f, FontStyle.Bold),
        ForeColor = UITheme.TextSecondary, AutoSize = true, Location = new Point(x, y)
    };
    private static DateTimePicker NuevoDate(int x, int y) => new()
    {
        Location = new Point(x, y), Size = new Size(140, 30), Format = DateTimePickerFormat.Short, Font = UITheme.Body
    };
    private static ComboBox NuevoCombo(int x, int y, int w) => new()
    {
        Location = new Point(x, y), Size = new Size(w, 30), DropDownStyle = ComboBoxStyle.DropDownList,
        FlatStyle = FlatStyle.Flat, Font = UITheme.Body
    };

    private void ConstruirColumnas()
    {
        _grid.AutoGenerateColumns = false;
        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Fecha y hora", FillWeight = 15 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFuncionario", HeaderText = "Funcionario", FillWeight = 18 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCargo", HeaderText = "Cargo", FillWeight = 16 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colValor", HeaderText = "Calificación", FillWeight = 12 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colComentario", HeaderText = "Comentario", FillWeight = 25 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colIP", HeaderText = "IP", FillWeight = 14 });
        _grid.Columns["colValor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        _grid.Columns["colValor"].DefaultCellStyle.Font = new Font(UITheme.FontFamilySemibold, 9f, FontStyle.Bold);
    }

    public async void CargarDatos()
    {
        _overlay.Mostrar("Cargando calificaciones...");
        try
        {
            using var scope = _sp.CreateScope();
            var calSvc = scope.ServiceProvider.GetRequiredService<ICalificacionService>();
            var encSvc = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
            var cal = await calSvc.ObtenerTodasAsync();
            var enc = await encSvc.ObtenerTodosAsync();
            _todas = cal.IsSuccess ? cal.Value!.ToList() : new();
            _funcionarios = enc.IsSuccess ? enc.Value!.ToList() : new();
            CargarCombos();
            AplicarFiltros();
        }
        catch (Exception)
        {
            Notificador.Error(FindForm(), "No se pudieron cargar las calificaciones. Verifique la conexión.");
        }
        finally { _overlay.Ocultar(); }
    }

    private void CargarCombos()
    {
        _cboFuncionario.Items.Clear();
        _cboFuncionario.Items.Add("Todos los funcionarios");
        foreach (var f in _funcionarios)
            _cboFuncionario.Items.Add($"{f.Persona?.PrimerNombre} {f.Persona?.PrimerApellido}".Trim());
        _cboFuncionario.SelectedIndex = 0;

        _cboTipo.Items.Clear();
        _cboTipo.Items.AddRange(new object[] { "Todas", "Excelente", "Buena", "Regular", "Mala" });
        _cboTipo.SelectedIndex = 0;

        if (_todas.Count > 0)
        {
            _desde.Value = _todas.Min(c => c.FechaHora).Date;
            _hasta.Value = _todas.Max(c => c.FechaHora).Date;
        }
    }

    private void LimpiarFiltros()
    {
        if (_cboFuncionario.Items.Count > 0) _cboFuncionario.SelectedIndex = 0;
        if (_cboTipo.Items.Count > 0) _cboTipo.SelectedIndex = 0;
        _txtBuscar.Text = "";
        if (_todas.Count > 0)
        {
            _desde.Value = _todas.Min(c => c.FechaHora).Date;
            _hasta.Value = _todas.Max(c => c.FechaHora).Date;
        }
        AplicarFiltros();
    }

    private string NombreFuncionario(Calificacion c)
    {
        var f = _funcionarios.FirstOrDefault(x => x.CedulaRucPersona == c.CedulaRucPersona);
        return f?.Persona != null ? $"{f.Persona.PrimerNombre} {f.Persona.PrimerApellido}".Trim() : c.CedulaRucPersona;
    }
    private string CargoFuncionario(Calificacion c)
        => _funcionarios.FirstOrDefault(x => x.CedulaRucPersona == c.CedulaRucPersona)?.Cargo ?? "—";

    private void AplicarFiltros()
    {
        if (_desde.Value.Date > _hasta.Value.Date)
        {
            Notificador.Advertencia(FindForm(), "La fecha 'desde' no puede ser posterior a la fecha 'hasta'.");
            return;
        }

        var datos = _todas.Where(c => c.FechaHora.Date >= _desde.Value.Date && c.FechaHora.Date <= _hasta.Value.Date);

        if (_cboFuncionario.SelectedIndex > 0)
        {
            var f = _funcionarios[_cboFuncionario.SelectedIndex - 1];
            datos = datos.Where(c => c.CedulaRucPersona == f.CedulaRucPersona);
        }
        if (_cboTipo.SelectedIndex > 0)
        {
            var val = (ValorCalificacion)_cboTipo.SelectedIndex;
            datos = datos.Where(c => c.Valor == val);
        }
        var buscar = _txtBuscar.Text.Trim().ToLower();
        if (buscar.Length > 0)
            datos = datos.Where(c => (c.Comentarios?.ToLower().Contains(buscar) ?? false)
                                  || NombreFuncionario(c).ToLower().Contains(buscar));

        var lista = datos.OrderByDescending(c => c.FechaHora).ToList();
        MostrarEnGrid(lista);
        ActualizarTarjetas(lista);
    }

    private void MostrarEnGrid(List<Calificacion> lista)
    {
        _grid.Rows.Clear();
        foreach (var c in lista)
        {
            int idx = _grid.Rows.Add(
                c.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                NombreFuncionario(c),
                CargoFuncionario(c),
                c.Valor.ToString(),
                string.IsNullOrWhiteSpace(c.Comentarios) ? "—" : c.Comentarios,
                c.IpCliente ?? "—");

            var cell = _grid.Rows[idx].Cells["colValor"];
            var color = UITheme.ColorCalificacion((int)c.Valor);
            cell.Style.ForeColor = color;
            cell.Style.SelectionForeColor = color;
        }
    }

    private void ActualizarTarjetas(List<Calificacion> lista)
    {
        _cardTotal.Valor = lista.Count.ToString();
        _cardExc.Valor = lista.Count(c => c.Valor == ValorCalificacion.Excelente).ToString();
        _cardBuena.Valor = lista.Count(c => c.Valor == ValorCalificacion.Buena).ToString();
        _cardReg.Valor = lista.Count(c => c.Valor == ValorCalificacion.Regular).ToString();
        _cardMala.Valor = lista.Count(c => c.Valor == ValorCalificacion.Mala).ToString();
    }
}
