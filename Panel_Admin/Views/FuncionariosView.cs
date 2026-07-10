using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Panel_Admin.UI;

namespace Panel_Admin.Views;

public class FuncionariosView : UserControl
{
    private readonly IServiceProvider _sp;
    private List<Empleado> _funcionarios = new();

    private UITextBox _busqueda = null!;
    private ComboBox _filtroCargo = null!;
    private UIButton _btnLimpiar = null!;
    private UIButton _btnNuevo = null!;
    private Label _lblContador = null!;
    private DataGridView _grid = null!;
    private LoadingOverlay _overlay = null!;

    public FuncionariosView(IServiceProvider serviceProvider)
    {
        _sp = serviceProvider;
        AutoScaleMode = AutoScaleMode.None;
        Dock = DockStyle.Fill;
        BackColor = UITheme.Background;
        ConstruirUI();
    }

    private void ConstruirUI()
    {
        // Tarjeta de filtros (arriba)
        var toolbar = new RoundedPanel
        {
            Dock = DockStyle.Top,
            Height = 88,
            Padding = new Padding(18, 16, 18, 16),
            Radius = 12
        };

        var lblBuscar = new Label
        {
            Text = "BUSCAR",
            Font = new Font(UITheme.FontFamilySemibold, 8f, FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            AutoSize = true,
            Location = new Point(20, 12)
        };
        _busqueda = new UITextBox
        {
            Location = new Point(20, 32),
            Size = new Size(320, 40),
            PlaceholderText = "Cédula, nombre o apellido..."
        };
        _busqueda.Inner.TextChanged += (_, _) => RefrescarGrid();

        var lblCargo = new Label
        {
            Text = "CARGO",
            Font = new Font(UITheme.FontFamilySemibold, 8f, FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            AutoSize = true,
            Location = new Point(360, 12)
        };
        _filtroCargo = new ComboBox
        {
            Location = new Point(360, 34),
            Size = new Size(230, 30),
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            Font = UITheme.Body
        };
        _filtroCargo.SelectedIndexChanged += (_, _) => RefrescarGrid();

        _btnLimpiar = new UIButton
        {
            Text = "Limpiar",
            Outline = true,
            BaseColor = UITheme.Neutral,
            Size = new Size(100, 40),
            Location = new Point(610, 32)
        };
        _btnLimpiar.Click += (_, _) =>
        {
            _busqueda.Text = "";
            if (_filtroCargo.Items.Count > 0) _filtroCargo.SelectedIndex = 0;
            RefrescarGrid();
        };

        _lblContador = new Label
        {
            Text = "0 funcionarios",
            Font = UITheme.Small,
            ForeColor = UITheme.TextSecondary,
            AutoSize = true,
            Location = new Point(730, 44)
        };

        _btnNuevo = new UIButton
        {
            Text = "＋  Nuevo funcionario",
            BaseColor = UITheme.Primary,
            Size = new Size(200, 44),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(toolbar.Width - 220, 22)
        };
        _btnNuevo.Click += async (_, _) => await NuevoFuncionario();

        toolbar.Controls.AddRange(new Control[]
        {
            lblBuscar, _busqueda, lblCargo, _filtroCargo, _btnLimpiar, _lblContador, _btnNuevo
        });

        // Tarjeta contenedora de la tabla
        var cardGrid = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(2),
            Radius = 12
        };

        _grid = new DataGridView { Dock = DockStyle.Fill };
        GridStyler.Apply(_grid);
        ConstruirColumnas();
        _grid.CellContentClick += Grid_CellClick;
        _grid.CellDoubleClick += async (s, e) => { if (e.RowIndex >= 0) await VerDetalle(e.RowIndex); };
        cardGrid.Controls.Add(_grid);

        _overlay = new LoadingOverlay("Cargando funcionarios...");

        // Espaciador entre toolbar y grid
        var spacer = new Panel { Dock = DockStyle.Top, Height = 16, BackColor = UITheme.Background };

        Controls.Add(cardGrid);
        Controls.Add(spacer);
        Controls.Add(toolbar);
        Controls.Add(_overlay);
    }

    private void ConstruirColumnas()
    {
        _grid.AutoGenerateColumns = false;
        _grid.Columns.Clear();

        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCedula", HeaderText = "Cédula", FillWeight = 14 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "Nombre completo", FillWeight = 22 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCargo", HeaderText = "Cargo", FillWeight = 20 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDireccion", HeaderText = "Dirección", FillWeight = 22 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colQR", HeaderText = "Estado QR", FillWeight = 10 });

        var estilos = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font(UITheme.FontFamily, 11f) };
        foreach (var (name, header) in new[] { ("colVer", "👁"), ("colEditar", "✏"), ("colGenQR", "QR"), ("colEliminar", "🗑") })
        {
            var col = new DataGridViewButtonColumn
            {
                Name = name,
                HeaderText = "",
                Text = header,
                UseColumnTextForButtonValue = true,
                FillWeight = 4,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = estilos
            };
            _grid.Columns.Add(col);
        }
        _grid.Columns["colQR"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
    }

    public async void CargarDatos()
    {
        _overlay.Mostrar("Cargando funcionarios...");
        try
        {
            using var scope = _sp.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
            var res = await svc.ObtenerTodosAsync();
            _funcionarios = res.IsSuccess ? res.Value!.ToList() : new();
            CargarFiltroCargos();
            RefrescarGrid();
        }
        catch (Exception)
        {
            Notificador.Error(FindForm(), "No se pudieron cargar los funcionarios. Verifique la conexión.");
        }
        finally { _overlay.Ocultar(); }
    }

    private void CargarFiltroCargos()
    {
        var cargos = _funcionarios.Select(f => f.Cargo).Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct().OrderBy(c => c).ToList();
        _filtroCargo.Items.Clear();
        _filtroCargo.Items.Add("Todos los cargos");
        foreach (var c in cargos) _filtroCargo.Items.Add(c!);
        _filtroCargo.SelectedIndex = 0;
    }

    private void RefrescarGrid()
    {
        var filtro = _busqueda.Text.Trim().ToLower();
        var cargoSel = _filtroCargo.SelectedIndex > 0 ? _filtroCargo.SelectedItem?.ToString() : null;

        var datos = _funcionarios.AsEnumerable();
        if (filtro.Length >= 1)
        {
            datos = datos.Where(f =>
                (f.Persona?.PrimerNombre?.ToLower().Contains(filtro) ?? false) ||
                (f.Persona?.PrimerApellido?.ToLower().Contains(filtro) ?? false) ||
                f.CedulaRucPersona.ToLower().Contains(filtro));
        }
        if (cargoSel != null)
            datos = datos.Where(f => f.Cargo == cargoSel);

        var lista = datos.ToList();
        _grid.Rows.Clear();
        foreach (var f in lista)
        {
            int idx = _grid.Rows.Add(
                f.CedulaRucPersona,
                $"{f.Persona?.PrimerNombre} {f.Persona?.PrimerApellido}".Trim(),
                f.Cargo ?? "—",
                f.Persona?.Direccion ?? "—",
                string.IsNullOrEmpty(f.EmpleadoQR?.CodigoQR) ? "Sin QR" : "✓ Activo",
                "👁", "✏", "QR", "🗑");

            var cellQR = _grid.Rows[idx].Cells["colQR"];
            bool tieneQR = !string.IsNullOrEmpty(f.EmpleadoQR?.CodigoQR);
            cellQR.Style.ForeColor = tieneQR ? UITheme.Success : UITheme.TextMuted;
            cellQR.Style.Font = new Font(UITheme.FontFamilySemibold, 9f, FontStyle.Bold);
        }

        _lblContador.Text = lista.Count == 1 ? "1 funcionario encontrado" : $"{lista.Count} funcionarios encontrados";
    }

    private Empleado? FuncionarioDeFila(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= _grid.Rows.Count) return null;
        var cedula = _grid.Rows[rowIndex].Cells["colCedula"].Value?.ToString();
        return _funcionarios.FirstOrDefault(f => f.CedulaRucPersona == cedula);
    }

    private async void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var colName = _grid.Columns[e.ColumnIndex].Name;
        switch (colName)
        {
            case "colVer":
            case "colGenQR":
                await VerDetalle(e.RowIndex);
                break;
            case "colEditar":
                await EditarFuncionario(e.RowIndex);
                break;
            case "colEliminar":
                await EliminarFuncionario(e.RowIndex);
                break;
        }
    }

    private async Task VerDetalle(int rowIndex)
    {
        var f = FuncionarioDeFila(rowIndex);
        if (f is null) return;
        using var form = new DetalleFuncionarioForm(f, _sp);
        form.ShowDialog(FindForm());
        CargarDatos();
        await Task.CompletedTask;
    }

    private async Task NuevoFuncionario()
    {
        using var form = new FuncionarioForm();
        if (form.ShowDialog(FindForm()) == DialogResult.OK && form.Resultado is not null)
        {
            _overlay.Mostrar("Guardando funcionario...");
            try
            {
                using var scope = _sp.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
                var res = await svc.CrearEncargadoAsync(form.Resultado);
                _overlay.Ocultar();
                if (res.IsSuccess)
                {
                    Notificador.Exito(FindForm(), "Funcionario creado correctamente. Se generó su código QR.");
                    CargarDatos();
                }
                else Notificador.Error(FindForm(), res.ErrorMessage ?? "No se pudo crear el funcionario.");
            }
            catch (Exception)
            {
                _overlay.Ocultar();
                Notificador.Error(FindForm(), "No se pudo completar la operación. Verifique la conexión.");
            }
        }
    }

    private async Task EditarFuncionario(int rowIndex)
    {
        var f = FuncionarioDeFila(rowIndex);
        if (f is null) return;
        var dto = new CrearEncargadoDto
        {
            CedulaRucPersona = f.CedulaRucPersona,
            Nombre = f.Persona?.PrimerNombre ?? "",
            Apellido = f.Persona?.PrimerApellido ?? "",
            Cargo = f.Cargo,
            Direccion = f.Persona?.Direccion
        };
        using var form = new FuncionarioForm(dto);
        if (form.ShowDialog(FindForm()) == DialogResult.OK && form.Resultado is not null)
        {
            _overlay.Mostrar("Actualizando...");
            try
            {
                using var scope = _sp.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
                var res = await svc.ActualizarEncargadoAsync(f.CedulaRucPersona, form.Resultado);
                _overlay.Ocultar();
                if (res.IsSuccess) { Notificador.Exito(FindForm(), "Funcionario actualizado correctamente."); CargarDatos(); }
                else Notificador.Error(FindForm(), res.ErrorMessage ?? "No se pudo actualizar.");
            }
            catch (Exception)
            {
                _overlay.Ocultar();
                Notificador.Error(FindForm(), "No se pudo completar la operación. Verifique la conexión.");
            }
        }
    }

    private async Task EliminarFuncionario(int rowIndex)
    {
        var f = FuncionarioDeFila(rowIndex);
        if (f is null) return;
        var nombre = $"{f.Persona?.PrimerNombre} {f.Persona?.PrimerApellido}".Trim();
        bool ok = Notificador.Confirmar(FindForm(),
            $"¿Está seguro de eliminar a {nombre} (cédula {f.CedulaRucPersona})?\n\n" +
            "Esta acción eliminará también su código QR y todas sus calificaciones asociadas. No se puede deshacer.",
            "Eliminar funcionario", "Eliminar funcionario", destructivo: true);
        if (!ok) return;

        _overlay.Mostrar("Eliminando...");
        try
        {
            using var scope = _sp.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
            var res = await svc.EliminarEncargadoAsync(f.CedulaRucPersona);
            _overlay.Ocultar();
            if (res.IsSuccess) { Notificador.Exito(FindForm(), "Funcionario eliminado correctamente."); CargarDatos(); }
            else Notificador.Error(FindForm(), res.ErrorMessage ?? "No se pudo eliminar.");
        }
        catch (Exception)
        {
            _overlay.Ocultar();
            Notificador.Error(FindForm(), "No se pudo completar la operación. Verifique la conexión.");
        }
    }
}
