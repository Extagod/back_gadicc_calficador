using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing.Printing;

namespace Panel_Admin;

public partial class DetalleFuncionarioForm : Form
{
    private readonly IEncargadoService _encargadoService;
    private Encargado _funcionario;

    public DetalleFuncionarioForm(Encargado funcionario, IServiceProvider serviceProvider)
    {
        _funcionario = funcionario;
        var scope = serviceProvider.CreateScope();
        _encargadoService = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
        InitializeComponent();
        CargarDatos();
    }

    private void CargarDatos()
    {
        lblNombreValor.Text = _funcionario.Nombre;
        lblApellidoValor.Text = _funcionario.Apellido;
        lblCargoValor.Text = _funcionario.Cargo ?? "—";
        lblDireccionValor.Text = _funcionario.Direccion ?? "—";
        lblTokenValor.Text = _funcionario.TokenQR ?? "Sin generar";

        if (!string.IsNullOrEmpty(_funcionario.CodigoQR))
        {
            var bytes = Convert.FromBase64String(_funcionario.CodigoQR);
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
        var resultado = await _encargadoService.RegenerarQRAsync(_funcionario.IdEncargado);
        if (resultado.IsSuccess)
        {
            // Actualizar datos locales
            var encResult = await _encargadoService.ObtenerPorIdAsync(_funcionario.IdEncargado);
            if (encResult.IsSuccess)
                _funcionario = encResult.Value!;

            var bytes = Convert.FromBase64String(resultado.Value!);
            using var ms = new MemoryStream(bytes);
            picQR.Image = Image.FromStream(ms);
            btnRegenerarQR.Text = "Regenerar QR";
            lblTokenValor.Text = _funcionario.TokenQR ?? "";
        }
        else
        {
            MessageBox.Show(resultado.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnGuardarQR_Click(object? sender, EventArgs e)
    {
        if (picQR.Image is null) return;

        using var dialog = new SaveFileDialog
        {
            Filter = "PNG Image|*.png",
            FileName = $"{_funcionario.Apellido}_{_funcionario.Nombre}_QR.png"
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
