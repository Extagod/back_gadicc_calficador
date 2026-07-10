using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Panel_Admin.UI;

public enum ToastType { Success, Warning, Error, Info }

/// <summary>Notificaciones y diálogos consistentes que reemplazan los MessageBox genéricos.</summary>
public static class Notificador
{
    public static void Exito(IWin32Window? owner, string mensaje, string titulo = "Operación exitosa")
        => Mostrar(owner, mensaje, titulo, ToastType.Success);

    public static void Advertencia(IWin32Window? owner, string mensaje, string titulo = "Advertencia")
        => Mostrar(owner, mensaje, titulo, ToastType.Warning);

    public static void Error(IWin32Window? owner, string mensaje, string titulo = "Error")
        => Mostrar(owner, mensaje, titulo, ToastType.Error);

    public static void Info(IWin32Window? owner, string mensaje, string titulo = "Información")
        => Mostrar(owner, mensaje, titulo, ToastType.Info);

    private static void Mostrar(IWin32Window? owner, string mensaje, string titulo, ToastType tipo)
    {
        using var dlg = new MensajeDialog(titulo, mensaje, tipo);
        if (owner is Form f) dlg.StartPosition = FormStartPosition.CenterParent;
        dlg.ShowDialog(owner);
    }

    /// <summary>Diálogo de confirmación con acción destructiva resaltada. Devuelve true si se confirma.</summary>
    public static bool Confirmar(IWin32Window? owner, string mensaje, string titulo = "Confirmar",
        string textoConfirmar = "Confirmar", bool destructivo = false)
    {
        using var dlg = new ConfirmDialog(titulo, mensaje, textoConfirmar, destructivo);
        return dlg.ShowDialog(owner) == DialogResult.OK;
    }
}

/// <summary>Diálogo de mensaje moderno (éxito/advertencia/error/info).</summary>
public class MensajeDialog : Form
{
    public MensajeDialog(string titulo, string mensaje, ToastType tipo)
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = UITheme.Card;
        Size = new Size(420, 220);
        ShowInTaskbar = false;

        Color color = tipo switch
        {
            ToastType.Success => UITheme.Success,
            ToastType.Warning => UITheme.Warning,
            ToastType.Error => UITheme.Danger,
            _ => UITheme.Info
        };
        string icono = tipo switch
        {
            ToastType.Success => "✓",
            ToastType.Warning => "!",
            ToastType.Error => "✕",
            _ => "i"
        };

        var barra = new Panel { Dock = DockStyle.Top, Height = 6, BackColor = color };
        Controls.Add(barra);

        var circulo = new Label
        {
            Text = icono,
            Font = new Font(UITheme.FontFamilySemibold, 20f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = color,
            Size = new Size(54, 54),
            Location = new Point(30, 34),
            TextAlign = ContentAlignment.MiddleCenter
        };
        MakeCircle(circulo);
        Controls.Add(circulo);

        var lblTitulo = new Label
        {
            Text = titulo,
            Font = UITheme.Title,
            ForeColor = UITheme.TextPrimary,
            AutoSize = false,
            Location = new Point(100, 38),
            Size = new Size(290, 30)
        };
        Controls.Add(lblTitulo);

        var lblMensaje = new Label
        {
            Text = mensaje,
            Font = UITheme.Body,
            ForeColor = UITheme.TextSecondary,
            AutoSize = false,
            Location = new Point(100, 72),
            Size = new Size(295, 70)
        };
        Controls.Add(lblMensaje);

        var btnOk = new UIButton
        {
            Text = "Aceptar",
            BaseColor = color,
            HoverColor = ControlPaint.Dark(color, 0.1f),
            Size = new Size(110, 38),
            Location = new Point(280, 158)
        };
        btnOk.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
        Controls.Add(btnOk);

        AcceptButton = btnOk;
        Paint += (_, e) =>
        {
            using var pen = new Pen(UITheme.Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        };
    }

    private static void MakeCircle(Control c)
    {
        var path = new GraphicsPath();
        path.AddEllipse(0, 0, c.Width, c.Height);
        c.Region = new Region(path);
    }
}

/// <summary>Diálogo de confirmación con botones Cancelar / acción.</summary>
public class ConfirmDialog : Form
{
    public ConfirmDialog(string titulo, string mensaje, string textoConfirmar, bool destructivo)
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = UITheme.Card;
        Size = new Size(440, 230);
        ShowInTaskbar = false;

        Color color = destructivo ? UITheme.Danger : UITheme.Primary;

        var barra = new Panel { Dock = DockStyle.Top, Height = 6, BackColor = color };
        Controls.Add(barra);

        var lblTitulo = new Label
        {
            Text = titulo,
            Font = UITheme.Title,
            ForeColor = UITheme.TextPrimary,
            AutoSize = false,
            Location = new Point(30, 34),
            Size = new Size(380, 30)
        };
        Controls.Add(lblTitulo);

        var lblMensaje = new Label
        {
            Text = mensaje,
            Font = UITheme.Body,
            ForeColor = UITheme.TextSecondary,
            AutoSize = false,
            Location = new Point(30, 74),
            Size = new Size(380, 80)
        };
        Controls.Add(lblMensaje);

        var btnCancelar = new UIButton
        {
            Text = "Cancelar",
            Outline = true,
            BaseColor = UITheme.Neutral,
            Size = new Size(130, 40),
            Location = new Point(140, 168)
        };
        btnCancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.Add(btnCancelar);

        var btnConfirmar = new UIButton
        {
            Text = textoConfirmar,
            BaseColor = color,
            HoverColor = ControlPaint.Dark(color, 0.1f),
            Size = new Size(150, 40),
            Location = new Point(280, 168)
        };
        btnConfirmar.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
        Controls.Add(btnConfirmar);

        AcceptButton = btnConfirmar;
        CancelButton = btnCancelar;
        Paint += (_, e) =>
        {
            using var pen = new Pen(UITheme.Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        };
    }
}

/// <summary>Overlay de carga semitransparente con texto.</summary>
public class LoadingOverlay : Panel
{
    private readonly Label _lbl;
    public LoadingOverlay(string texto = "Cargando...")
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(180, 245, 247, 250);
        _lbl = new Label
        {
            Text = texto,
            Font = new Font(UITheme.FontFamilySemibold, 12f, FontStyle.Bold),
            ForeColor = UITheme.Primary,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(_lbl);
        Visible = false;
    }
    public void Mostrar(string? texto = null) { if (texto != null) _lbl.Text = texto; Visible = true; BringToFront(); }
    public void Ocultar() => Visible = false;
}
