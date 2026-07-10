using System.Drawing;

namespace Panel_Admin.UI;

/// <summary>
/// Paleta institucional y tipografías centralizadas para todo el panel.
/// </summary>
public static class UITheme
{
    // === Colores principales (institucionales CAÑAR) ===
    public static readonly Color Primary = Color.FromArgb(28, 74, 138);      // azul institucional
    public static readonly Color PrimaryDark = Color.FromArgb(20, 54, 104);  // hover/pressed
    public static readonly Color PrimaryLight = Color.FromArgb(233, 240, 250); // azul muy claro
    public static readonly Color Sidebar = Color.FromArgb(19, 44, 88);       // azul marino del logo
    public static readonly Color SidebarHover = Color.FromArgb(30, 60, 110);
    public static readonly Color SidebarActive = Color.FromArgb(28, 74, 138);

    // Acento rojo del logo (detalle de la montaña)
    public static readonly Color Accent = Color.FromArgb(192, 32, 46);

    // === Fondos ===
    public static readonly Color Background = Color.FromArgb(245, 247, 250);  // #F5F7FA
    public static readonly Color Card = Color.White;
    public static readonly Color Border = Color.FromArgb(217, 225, 234);      // #D9E1EA
    public static readonly Color HeaderBar = Color.White;

    // === Texto ===
    public static readonly Color TextPrimary = Color.FromArgb(33, 43, 54);
    public static readonly Color TextSecondary = Color.FromArgb(109, 122, 138);
    public static readonly Color TextOnDark = Color.FromArgb(236, 240, 245);
    public static readonly Color TextMuted = Color.FromArgb(150, 160, 172);

    // === Estados (calificaciones) ===
    public static readonly Color Excelente = Color.FromArgb(46, 125, 50);    // verde profesional
    public static readonly Color Buena = Color.FromArgb(102, 187, 106);      // verde claro
    public static readonly Color Regular = Color.FromArgb(245, 124, 0);      // naranja
    public static readonly Color Mala = Color.FromArgb(211, 47, 47);         // rojo
    public static readonly Color Info = Color.FromArgb(33, 150, 243);        // azul info

    // === Estados neutros ===
    public static readonly Color Success = Color.FromArgb(46, 125, 50);
    public static readonly Color Warning = Color.FromArgb(245, 124, 0);
    public static readonly Color Danger = Color.FromArgb(211, 47, 47);
    public static readonly Color Neutral = Color.FromArgb(96, 108, 122);

    // === Grid ===
    public static readonly Color GridHeader = Color.FromArgb(30, 90, 168);
    public static readonly Color GridRowAlt = Color.FromArgb(245, 248, 252);
    public static readonly Color GridSelection = Color.FromArgb(234, 242, 255);
    public static readonly Color GridSelectionText = Color.FromArgb(33, 43, 54);

    // === Tipografías ===
    public const string FontFamily = "Segoe UI";
    public const string FontFamilySemibold = "Segoe UI Semibold";

    public static Font TitleXL => new(FontFamilySemibold, 20f, FontStyle.Bold);
    public static Font Title => new(FontFamilySemibold, 15f, FontStyle.Bold);
    public static Font Subtitle => new(FontFamily, 11f, FontStyle.Regular);
    public static Font SectionTitle => new(FontFamilySemibold, 12.5f, FontStyle.Bold);
    public static Font CardValue => new(FontFamilySemibold, 24f, FontStyle.Bold);
    public static Font CardLabel => new(FontFamily, 9.5f, FontStyle.Regular);
    public static Font Body => new(FontFamily, 10f, FontStyle.Regular);
    public static Font BodyBold => new(FontFamilySemibold, 10f, FontStyle.Bold);
    public static Font Small => new(FontFamily, 9f, FontStyle.Regular);
    public static Font Button => new(FontFamilySemibold, 10f, FontStyle.Bold);

    /// <summary>Color de una calificación por su valor.</summary>
    public static Color ColorCalificacion(int valor) => valor switch
    {
        1 => Excelente,
        2 => Buena,
        3 => Regular,
        4 => Mala,
        _ => Neutral
    };
}
