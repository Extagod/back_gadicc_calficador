using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing.Printing;

namespace Panel_Admin;

public partial class DetalleFuncionarioForm : Form
{
    private readonly IEncargadoService _encargadoService;
    private Empleado _funcionario;

    public DetalleFuncionarioForm(Empleado funcionario, IServiceProvider serviceProvider)
    {
        _funcionario = funcionario;
        var scope = serviceProvider.CreateScope();
        _encargadoService = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
        InitializeComponent();
        CargarDatos();
    }

    private void CargarDatos()
    {
        lblCedulaValor.Text = _funcionario.CedulaRucPersona;
        lblNombreValor.Text = _funcionario.Persona?.PrimerNombre ?? "—";
        lblApellidoValor.Text = _funcionario.Persona?.PrimerApellido ?? "—";
        lblCargoValor.Text = _funcionario.Cargo ?? "—";
        lblDireccionValor.Text = _funcionario.Persona?.Direccion ?? "—";
        lblTokenValor.Text = _funcionario.EmpleadoQR?.TokenQR ?? "Sin generar";

        var codigoQR = _funcionario.EmpleadoQR?.CodigoQR;
        if (!string.IsNullOrEmpty(codigoQR))
        {
            var bytes = Convert.FromBase64String(codigoQR);
            using var ms = new MemoryStream(bytes);
            picQR.Image = Image.FromStream(ms);
            btnRegenerarQR.Text = "Regenerar QR";
        }
        else
        {
            picQR.Image = null;
            btnRegenerarQR.Text = "Generar QR";
        }
    }

    private async void BtnRegenerarQR_Click(object? sender, EventArgs e)
    {
        var resultado = await _encargadoService.RegenerarQRAsync(_funcionario.CedulaRucPersona);
        if (resultado.IsSuccess)
        {
            // Actualizar datos locales
            var encResult = await _encargadoService.ObtenerPorCedulaAsync(_funcionario.CedulaRucPersona);
            if (encResult.IsSuccess)
                _funcionario = encResult.Value!;

            var bytes = Convert.FromBase64String(resultado.Value!);
            using var ms = new MemoryStream(bytes);
            picQR.Image = Image.FromStream(ms);
            btnRegenerarQR.Text = "Regenerar QR";
            lblTokenValor.Text = _funcionario.EmpleadoQR?.TokenQR ?? "";
        }
        else
        {
            MessageBox.Show(resultado.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnGuardarQR_Click(object? sender, EventArgs e)
    {
        if (picQR.Image is null) return;

        var nombre = _funcionario.Persona?.PrimerNombre ?? "funcionario";
        var apellido = _funcionario.Persona?.PrimerApellido ?? "";
        using var dialog = new SaveFileDialog
        {
            Filter = "PNG Image|*.png",
            FileName = $"{apellido}_{nombre}_QR.png"
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
            MessageBox.Show("La impresión no pudo completarse. Verifique la impresora.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnCerrar_Click(object? sender, EventArgs e)
    {
        this.Close();
    }
}
