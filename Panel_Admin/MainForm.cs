using Microsoft.Extensions.DependencyInjection;
using Capa_Abstracciones.Interfaces;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;
using System.Drawing.Printing;

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
            dgvFuncionarios.DataSource = null;
            dgvFuncionarios.DataSource = _funcionarios.Select(f => new
            {
                f.IdEncargado,
                f.Nombre,
                f.Apellido,
                f.Cargo,
                f.Direccion
            }).ToList();
        }
    }

    private void TxtBusqueda_TextChanged(object? sender, EventArgs e)
    {
        var filtro = txtBusqueda.Text.Trim().ToLower();
        if (filtro.Length >= 1)
        {
            var filtered = _funcionarios
                .Where(f => f.Nombre.ToLower().Contains(filtro)
                         || f.Apellido.ToLower().Contains(filtro))
                .Select(f => new { f.IdEncargado, f.Nombre, f.Apellido, f.Cargo, f.Direccion })
                .ToList();
            dgvFuncionarios.DataSource = filtered;
        }
        else
        {
            dgvFuncionarios.DataSource = _funcionarios.Select(f => new
            {
                f.IdEncargado,
                f.Nombre,
                f.Apellido,
                f.Cargo,
                f.Direccion
            }).ToList();
        }
    }

    private async void BtnNuevoFuncionario_Click(object? sender, EventArgs e)
    {
        lblValidacionNombre.Text = "";
        lblValidacionApellido.Text = "";

        if (string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            lblValidacionNombre.Text = "El nombre es obligatorio.";
            return;
        }
        if (string.IsNullOrWhiteSpace(txtApellido.Text))
        {
            lblValidacionApellido.Text = "El apellido es obligatorio.";
            return;
        }

        var dto = new CrearEncargadoDto
        {
            Nombre = txtNombre.Text.Trim(),
            Apellido = txtApellido.Text.Trim(),
            Cargo = txtCargo.Text.Trim(),
            Direccion = txtDireccion.Text.Trim()
        };

        try
        {
            var resultado = await _encargadoService.CrearEncargadoAsync(dto);
            if (resultado.IsSuccess)
            {
                await CargarFuncionariosAsync();
                LimpiarFormularioFuncionario();
            }
            else
            {
                MessageBox.Show(resultado.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception)
        {
            MessageBox.Show("No se pudo completar la operación. Verifique la conexión.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnEditarFuncionario_Click(object? sender, EventArgs e)
    {
        if (_selectedFuncionario is null) return;
        lblValidacionNombre.Text = "";
        lblValidacionApellido.Text = "";

        if (string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            lblValidacionNombre.Text = "El nombre es obligatorio.";
            return;
        }
        if (string.IsNullOrWhiteSpace(txtApellido.Text))
        {
            lblValidacionApellido.Text = "El apellido es obligatorio.";
            return;
        }

        var dto = new CrearEncargadoDto
        {
            Nombre = txtNombre.Text.Trim(),
            Apellido = txtApellido.Text.Trim(),
            Cargo = txtCargo.Text.Trim(),
            Direccion = txtDireccion.Text.Trim()
        };

        try
        {
            var resultado = await _encargadoService.ActualizarEncargadoAsync(_selectedFuncionario.IdEncargado, dto);
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
            MessageBox.Show("No se pudo completar la operación. Verifique la conexión.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DgvFuncionarios_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvFuncionarios.SelectedRows.Count > 0)
        {
            var id = (int)dgvFuncionarios.SelectedRows[0].Cells["IdEncargado"].Value;
            _selectedFuncionario = _funcionarios.FirstOrDefault(f => f.IdEncargado == id);
            if (_selectedFuncionario is not null)
            {
                txtNombre.Text = _selectedFuncionario.Nombre;
                txtApellido.Text = _selectedFuncionario.Apellido;
                txtCargo.Text = _selectedFuncionario.Cargo ?? "";
                txtDireccion.Text = _selectedFuncionario.Direccion ?? "";

                // QR tab: show existing QR
                if (!string.IsNullOrEmpty(_selectedFuncionario.CodigoQR))
                {
                    var bytes = Convert.FromBase64String(_selectedFuncionario.CodigoQR);
                    using var ms = new MemoryStream(bytes);
                    picQR.Image = Image.FromStream(ms);
                    btnGenerarQR.Text = "Regenerar QR";
                }
                else
                {
                    picQR.Image = null;
                    btnGenerarQR.Text = "Generar QR";
                }
            }
        }
    }

    private void LimpiarFormularioFuncionario()
    {
        txtNombre.Text = "";
        txtApellido.Text = "";
        txtCargo.Text = "";
        txtDireccion.Text = "";
        _selectedFuncionario = null;
    }

    // === Tab QR ===
    private async void BtnGenerarQR_Click(object? sender, EventArgs e)
    {
        if (_selectedFuncionario is null)
        {
            MessageBox.Show("Debe seleccionar un funcionario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var resultado = await _encargadoService.RegenerarQRAsync(_selectedFuncionario.IdEncargado);
        if (resultado.IsSuccess)
        {
            var bytes = Convert.FromBase64String(resultado.Value!);
            using var ms = new MemoryStream(bytes);
            picQR.Image = Image.FromStream(ms);
            btnGenerarQR.Text = "Regenerar QR";
            await CargarFuncionariosAsync();
        }
        else
        {
            MessageBox.Show(resultado.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnGuardarQR_Click(object? sender, EventArgs e)
    {
        if (picQR.Image is null || _selectedFuncionario is null) return;

        using var dialog = new SaveFileDialog
        {
            Filter = "PNG Image|*.png",
            FileName = $"{_selectedFuncionario.Apellido}_{_selectedFuncionario.Nombre}_QR.png"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            picQR.Image.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    private void BtnImprimirQR_Click(object? sender, EventArgs e)
    {
        if (picQR.Image is null) return;

        try
        {
            var printDoc = new PrintDocument();
            printDoc.PrintPage += (s, ev) =>
            {
                ev.Graphics!.DrawImage(picQR.Image, ev.MarginBounds);
            };
            printDoc.Print();
        }
        catch (Exception)
        {
            MessageBox.Show("La impresión no pudo completarse. Verifique la impresora.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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

        // Statistics
        lblTotal.Text = $"Total: {calificaciones.Count}";
        lblExcelente.Text = $"Excelente: {calificaciones.Count(c => c.Valor == ValorCalificacion.Excelente)}";
        lblBuena.Text = $"Buena: {calificaciones.Count(c => c.Valor == ValorCalificacion.Buena)}";
        lblRegular.Text = $"Regular: {calificaciones.Count(c => c.Valor == ValorCalificacion.Regular)}";
        lblMala.Text = $"Mala: {calificaciones.Count(c => c.Valor == ValorCalificacion.Mala)}";
    }

    private async void BtnFiltrarFechas_Click(object? sender, EventArgs e)
    {
        if (_selectedFuncionario is null) return;

        if (dtpFechaInicio.Value > dtpFechaFin.Value)
        {
            MessageBox.Show("La fecha de inicio no puede ser posterior a la fecha de fin.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
