using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Panel_Admin.UI;
using System.Drawing.Printing;

namespace Panel_Admin;

public partial class DetalleFuncionarioForm : Form
{
    private readonly IServiceProvider _sp;
    private Empleado _funcionario;

    public DetalleFuncionarioForm(Empleado funcionario, IServiceProvider serviceProvider)
    {
        _funcionario = funcionario;
        _sp = serviceProvider;
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
            MostrarQR(codigoQR);
            btnRegenerarQR.Text = "Regenerar QR";
            btnGuardarQR.Enabled = true;
            btnImprimirQR.Enabled = true;
        }
        else
        {
            picQR.Image = null;
            btnRegenerarQR.Text = "Generar QR";
            btnGuardarQR.Enabled = false;
            btnImprimirQR.Enabled = false;
        }
    }

    private void MostrarQR(string base64)
    {
        try
        {
            var bytes = Convert.FromBase64String(base64);
            using var ms = new MemoryStream(bytes);
            picQR.Image = Image.FromStream(ms);
        }
        catch { picQR.Image = null; }
    }

    private async void BtnRegenerarQR_Click(object? sender, EventArgs e)
    {
        btnRegenerarQR.Enabled = false;
        btnRegenerarQR.Text = "Generando...";
        try
        {
            using var scope = _sp.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<IEncargadoService>();
            var res = await svc.RegenerarQRAsync(_funcionario.CedulaRucPersona);
            if (res.IsSuccess)
            {
                var enc = await svc.ObtenerPorCedulaAsync(_funcionario.CedulaRucPersona);
                if (enc.IsSuccess) _funcionario = enc.Value!;
                MostrarQR(res.Value!);
                lblTokenValor.Text = _funcionario.EmpleadoQR?.TokenQR ?? "";
                btnGuardarQR.Enabled = true;
                btnImprimirQR.Enabled = true;
                Notificador.Exito(this, "Código QR generado correctamente.");
            }
            else Notificador.Error(this, res.ErrorMessage ?? "No se pudo generar el QR.");
        }
        catch (Exception)
        {
            Notificador.Error(this, "No se pudo completar la operación. Verifique la conexión.");
        }
        finally
        {
            btnRegenerarQR.Enabled = true;
            btnRegenerarQR.Text = string.IsNullOrEmpty(_funcionario.EmpleadoQR?.CodigoQR) ? "Generar QR" : "Regenerar QR";
        }
    }

    private void BtnGuardarQR_Click(object? sender, EventArgs e)
    {
        if (picQR.Image is null) return;
        var nombre = _funcionario.Persona?.PrimerNombre ?? "funcionario";
        var apellido = _funcionario.Persona?.PrimerApellido ?? "";
        using var dialog = new SaveFileDialog
        {
            Filter = "Imagen PNG|*.png",
            FileName = $"QR_{apellido}_{nombre}.png"
        };
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            picQR.Image.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            Notificador.Exito(this, "Código QR guardado correctamente.");
        }
    }

    private void BtnImprimirQR_Click(object? sender, EventArgs e)
    {
        if (picQR.Image is null) return;
        try
        {
            var printDoc = new PrintDocument();
            printDoc.PrintPage += (s, ev) => ev.Graphics!.DrawImage(picQR.Image, ev.MarginBounds);
            printDoc.Print();
        }
        catch (Exception)
        {
            Notificador.Error(this, "La impresión no pudo completarse. Verifique la impresora.");
        }
    }

    private void BtnCerrar_Click(object? sender, EventArgs e) => Close();
}
