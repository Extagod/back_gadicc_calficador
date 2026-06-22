using Microsoft.Extensions.DependencyInjection;
using Capa_Abstracciones.Interfaces;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;

namespace Panel_Admin;

public partial class MainForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEncargadoService _encargadoService;
    private readonly ICalificacionService _calificacionService;
    private List<Encargado> _funcionarios = new();
    private Encargado? _selectedFuncionario;

    public MainForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var scope = serviceProvider.CreateScope();
        _encargadoService = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
        _calificacionService = scope.ServiceProvider.GetRequiredService<ICalificacionService>();
        InitializeComponent();
        this.Load += MainForm_Load;
    }

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        await CargarFuncionariosAsync();
    }

    // === Tab Funcionarios ===
    private async Task CargarFuncionariosAsync()
    {
        var resultado = await _encargadoService.ObtenerTodosAsync();
        if (resultado.IsSuccess)
        {
            _funcionarios = resultado.Value!.ToList();
            RefrescarGrid();
        }
    }

    private void RefrescarGrid()
    {
        var filtro = txtBusqueda.Text.Trim().ToLower();
        var datos = _funcionarios.AsEnumerable();

        if (filtro.Length >= 1)
        {
            datos = datos.Where(f => f.Nombre.ToLower().Contains(filtro)
                                  || f.Apellido.ToLower().Contains(filtro));
        }

        dgvFuncionarios.DataSource = datos.Select(f => new
        {
            f.IdEncargado,
            f.Nombre,
            f.Apellido,
            f.Cargo,
            f.Direccion,
            QR = string.IsNullOrEmpty(f.CodigoQR) ? "❌" : "✅"
        }).ToList();
    }

    private void TxtBusqueda_TextChanged(object? sender, EventArgs e)
    {
        RefrescarGrid();
    }

    private async void BtnNuevo_Click(object? sender, EventArgs e)
    {
        using var form = new FuncionarioForm();
        if (form.ShowDialog(this) == DialogResult.OK && form.Resultado is not null)
        {
            try
            {
                var resultado = await _encargadoService.CrearEncargadoAsync(form.Resultado);
                if (resultado.IsSuccess)
                {
                    await CargarFuncionariosAsync();
                }
                else
                {
                    MessageBox.Show(resultado.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo completar la operación. Verifique la conexión.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnEditar_Click(object? sender, EventArgs e)
    {
        if (_selectedFuncionario is null)
        {
            MessageBox.Show("Seleccione un funcionario para editar.", "Validación",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var datosActuales = new CrearEncargadoDto
        {
            Nombre = _selectedFuncionario.Nombre,
            Apellido = _selectedFuncionario.Apellido,
            Cargo = _selectedFuncionario.Cargo,
            Direccion = _selectedFuncionario.Direccion
        };

        using var form = new FuncionarioForm(datosActuales);
        if (form.ShowDialog(this) == DialogResult.OK && form.Resultado is not null)
        {
            try
            {
                var resultado = await _encargadoService.ActualizarEncargadoAsync(
                    _selectedFuncionario.IdEncargado, form.Resultado);
                if (resultado.IsSuccess)
                {
                    await CargarFuncionariosAsync();
                }
                else
                {
                    MessageBox.Show(resultado.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo completar la operación. Verifique la conexión.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_selectedFuncionario is null)
        {
            MessageBox.Show("Seleccione un funcionario para eliminar.", "Validación",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"¿Está seguro de eliminar a {_selectedFuncionario.Nombre} {_selectedFuncionario.Apellido}?\n\nEsta acción eliminará también todas sus calificaciones.",
            "Confirmar Eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm == DialogResult.Yes)
        {
            try
            {
                // Eliminar via SQL directamente ya que no tenemos un método Delete en el servicio
                // Agreguemos soporte para eliminar
                var resultado = await _encargadoService.EliminarEncargadoAsync(_selectedFuncionario.IdEncargado);
                if (resultado.IsSuccess)
                {
                    _selectedFuncionario = null;
                    await CargarFuncionariosAsync();
                }
                else
                {
                    MessageBox.Show(resultado.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo completar la operación. Verifique la conexión.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void DgvFuncionarios_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvFuncionarios.SelectedRows.Count > 0)
        {
            var id = (int)dgvFuncionarios.SelectedRows[0].Cells["IdEncargado"].Value;
            _selectedFuncionario = _funcionarios.FirstOrDefault(f => f.IdEncargado == id);
        }
    }

    private void DgvFuncionarios_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        var id = (int)dgvFuncionarios.Rows[e.RowIndex].Cells["IdEncargado"].Value;
        var funcionario = _funcionarios.FirstOrDefault(f => f.IdEncargado == id);
        if (funcionario is null) return;

        using var form = new DetalleFuncionarioForm(funcionario, _serviceProvider);
        form.ShowDialog(this);

        // Refrescar por si se regeneró el QR
        _ = CargarFuncionariosAsync();
    }

    // === Tab Calificaciones ===
    private async void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (tabControl.SelectedTab == tabCalificaciones && _selectedFuncionario is not null)
        {
            await CargarCalificacionesAsync(_selectedFuncionario.IdEncargado);
        }
    }

    private async Task CargarCalificacionesAsync(int idEncargado)
    {
        var resultado = await _calificacionService.ObtenerPorEncargadoAsync(idEncargado);
        if (resultado.IsSuccess)
        {
            var calificaciones = resultado.Value!.ToList();
            MostrarCalificaciones(calificaciones);
        }
    }

    private void MostrarCalificaciones(List<Calificacion> calificaciones)
    {
        if (calificaciones.Count == 0)
        {
            lblNoCalificaciones.Visible = true;
            dgvCalificaciones.DataSource = null;
        }
        else
        {
            lblNoCalificaciones.Visible = false;
            dgvCalificaciones.DataSource = calificaciones
                .OrderByDescending(c => c.FechaHora)
                .Select(c => new
                {
                    c.FechaHora,
                    Valor = c.Valor.ToString(),
                    c.Comentarios
                }).ToList();
        }

        lblTotal.Text = $"Total: {calificaciones.Count}";
        lblExcelente.Text = $"Excelente: {calificaciones.Count(c => c.Valor == ValorCalificacion.Excelente)}";
        lblBuena.Text = $"Buena: {calificaciones.Count(c => c.Valor == ValorCalificacion.Buena)}";
        lblRegular.Text = $"Regular: {calificaciones.Count(c => c.Valor == ValorCalificacion.Regular)}";
        lblMala.Text = $"Mala: {calificaciones.Count(c => c.Valor == ValorCalificacion.Mala)}";
    }

    private async void BtnFiltrarFechas_Click(object? sender, EventArgs e)
    {
        if (_selectedFuncionario is null)
        {
            MessageBox.Show("Seleccione un funcionario de la pestaña Funcionarios primero.",
                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (dtpFechaInicio.Value > dtpFechaFin.Value)
        {
            MessageBox.Show("La fecha de inicio no puede ser posterior a la fecha de fin.",
                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var resultado = await _calificacionService.ObtenerPorEncargadoYRangoAsync(
            _selectedFuncionario.IdEncargado, dtpFechaInicio.Value, dtpFechaFin.Value);

        if (resultado.IsSuccess)
        {
            MostrarCalificaciones(resultado.Value!.ToList());
        }
    }
}
