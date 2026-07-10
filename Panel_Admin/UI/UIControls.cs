using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Panel_Admin.UI;

/// <summary>Panel con esquinas redondeadas y borde suave opcional.</summary>
public class RoundedPanel : Panel
{
    public int Radius { get; set; } = 10;
    public Color BorderColor { get; set; } = UITheme.Border;
    public int BorderThickness { get; set; } = 1;
    public bool ShadowEffect { get; set; } = false;

    public RoundedPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = UITheme.Card;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = RoundedRect(rect, Radius);

        using (var bg = new SolidBrush(BackColor))
            e.Graphics.FillPath(bg, path);

        if (BorderThickness > 0)
        {
            using var pen = new Pen(BorderColor, BorderThickness);
            e.Graphics.DrawPath(pen, path);
        }
    }

    public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();
        if (radius <= 0) { path.AddRectangle(bounds); path.CloseFigure(); return path; }
        path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}

/// <summary>Botón plano con esquinas redondeadas y estados hover/pressed.</summary>
public class UIButton : Button
{
    public int Radius { get; set; } = 8;
    public Color BaseColor { get; set; } = UITheme.Primary;
    public Color HoverColor { get; set; } = UITheme.PrimaryDark;
    public bool Outline { get; set; } = false;
    private bool _hover;

    public UIButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.MouseOverBackColor = Color.Transparent;
        FlatAppearance.MouseDownBackColor = Color.Transparent;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = Color.Transparent;
        ForeColor = Color.White;
        Font = UITheme.Button;
        Cursor = Cursors.Hand;
        Size = new Size(120, 38);
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = RoundedPanel.RoundedRect(rect, Radius);

        Color fill = Outline ? (_hover ? UITheme.PrimaryLight : UITheme.Card) : (_hover ? HoverColor : BaseColor);
        using (var b = new SolidBrush(Enabled ? fill : Color.FromArgb(210, 214, 220)))
            g.FillPath(b, path);

        if (Outline)
        {
            using var pen = new Pen(BaseColor, 1.5f);
            g.DrawPath(pen, path);
        }

        Color textColor = Outline ? BaseColor : ForeColor;
        TextRenderer.DrawText(g, Text, Font, rect,
            Enabled ? textColor : Color.FromArgb(140, 146, 154),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }
}

/// <summary>Etiqueta tipo "badge" con fondo de color y texto centrado.</summary>
public class Badge : Label
{
    public int Radius { get; set; } = 10;
    public Color FillColor { get; set; } = UITheme.Info;

    public Badge()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = Color.Transparent;
        ForeColor = Color.White;
        Font = new Font(UITheme.FontFamilySemibold, 8.5f, FontStyle.Bold);
        TextAlign = ContentAlignment.MiddleCenter;
        AutoSize = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = RoundedPanel.RoundedRect(rect, Radius);
        using (var b = new SolidBrush(FillColor))
            e.Graphics.FillPath(b, path);
        TextRenderer.DrawText(e.Graphics, Text, Font, rect, ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
}

/// <summary>Caja de texto con placeholder y borde redondeado (contenedor).</summary>
public class UITextBox : Panel
{
    public TextBox Inner { get; }
    public int Radius { get; set; } = 8;
    private bool _focused;

    public UITextBox()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = UITheme.Card;
        Padding = new Padding(12, 0, 12, 0);
        Height = 42;
        Inner = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = UITheme.Body,
            BackColor = UITheme.Card,
            ForeColor = UITheme.TextPrimary,
        };
        Inner.GotFocus += (_, _) => { _focused = true; Invalidate(); };
        Inner.LostFocus += (_, _) => { _focused = false; Invalidate(); };
        Controls.Add(Inner);
    }

    public string PlaceholderText { get => Inner.PlaceholderText; set => Inner.PlaceholderText = value; }
    public override string Text { get => Inner.Text; set => Inner.Text = value; }
    public bool UseSystemPasswordChar { get => Inner.UseSystemPasswordChar; set => Inner.UseSystemPasswordChar = value; }

    protected override void OnResize(EventArgs eventargs)
    {
        base.OnResize(eventargs);
        if (Inner != null)
        {
            Inner.Width = Width - Padding.Left - Padding.Right;
            Inner.Location = new Point(Padding.Left, (Height - Inner.Height) / 2);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = RoundedPanel.RoundedRect(rect, Radius);
        using (var b = new SolidBrush(BackColor)) e.Graphics.FillPath(b, path);
        using var pen = new Pen(_focused ? UITheme.Primary : UITheme.Border, _focused ? 1.8f : 1f);
        e.Graphics.DrawPath(pen, path);
    }
}


/// <summary>Botón de navegación de la barra lateral con estado activo e icono.</summary>
public class SidebarButton : Button
{
    private bool _active;
    private bool _hover;
    public string Icono { get; set; } = "";

    public bool Active
    {
        get => _active;
        set { _active = value; Invalidate(); }
    }

    public SidebarButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = Color.Transparent;
        ForeColor = UITheme.TextOnDark;
        Font = new Font(UITheme.FontFamily, 10.5f, FontStyle.Regular);
        Cursor = Cursors.Hand;
        Height = 48;
        Dock = DockStyle.Top;
        TextAlign = ContentAlignment.MiddleLeft;
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        Color bg = _active ? UITheme.SidebarActive : (_hover ? UITheme.SidebarHover : UITheme.Sidebar);
        using (var b = new SolidBrush(bg))
            g.FillRectangle(b, ClientRectangle);

        // Barra indicadora izquierda cuando está activo
        if (_active)
        {
            using var accent = new SolidBrush(Color.White);
            g.FillRectangle(accent, 0, 8, 4, Height - 16);
        }

        Color fg = _active ? Color.White : UITheme.TextOnDark;

        // Icono
        var iconRect = new Rectangle(20, 0, 30, Height);
        using (var fIcon = new Font("Segoe UI Emoji", 12.5f))
            TextRenderer.DrawText(g, Icono, fIcon, iconRect, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

        // Texto
        var textRect = new Rectangle(56, 0, Width - 60, Height);
        TextRenderer.DrawText(g, Text, Font, textRect, fg,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
    }
}


/// <summary>Tarjeta de métrica con icono, valor y etiqueta. Barra de color a la izquierda.</summary>
public class MetricCard : RoundedPanel
{
    private readonly Label _valor;
    private readonly Label _etiqueta;
    private readonly Label _icono;
    private readonly Color _acento;

    public MetricCard(string etiqueta, Color acento, string icono = "")
    {
        _acento = acento;
        Radius = 14;
        Height = 150;
        BackColor = UITheme.Card;

        // Icono en la esquina superior izquierda
        _icono = new Label
        {
            Text = icono,
            Font = new Font("Segoe UI Emoji", 17f),
            ForeColor = acento,
            AutoSize = false,
            Size = new Size(40, 40),
            Location = new Point(20, 18),
            TextAlign = ContentAlignment.MiddleLeft
        };
        // Etiqueta arriba, a la derecha del icono
        _etiqueta = new Label
        {
            Text = etiqueta,
            Font = new Font(UITheme.FontFamily, 11f, FontStyle.Regular),
            ForeColor = UITheme.TextSecondary,
            AutoSize = false,
            AutoEllipsis = true,
            Location = new Point(64, 26),
            Size = new Size(150, 24),
            TextAlign = ContentAlignment.MiddleLeft
        };
        // Valor grande abajo, ocupa todo el ancho (no se corta)
        _valor = new Label
        {
            Text = "0",
            Font = new Font(UITheme.FontFamilySemibold, 34f, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            AutoSize = false,
            AutoEllipsis = true,
            Location = new Point(20, 70),
            Size = new Size(180, 62),
            TextAlign = ContentAlignment.MiddleLeft
        };
        Controls.Add(_icono);
        Controls.Add(_etiqueta);
        Controls.Add(_valor);
    }

    public string Valor { get => _valor.Text; set => _valor.Text = value; }

    protected override void OnResize(EventArgs eventargs)
    {
        base.OnResize(eventargs);
        if (_etiqueta != null) _etiqueta.Width = Math.Max(50, Width - _etiqueta.Left - 14);
        if (_valor != null) _valor.Width = Math.Max(60, Width - _valor.Left - 14);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        // Barra de acento a la izquierda
        using var b = new SolidBrush(_acento);
        var rect = new Rectangle(0, 12, 5, Height - 24);
        e.Graphics.FillRectangle(b, rect);
    }
}
