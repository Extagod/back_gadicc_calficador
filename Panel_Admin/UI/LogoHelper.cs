using System.Drawing;

namespace Panel_Admin.UI;

/// <summary>Carga el logo institucional desde la carpeta Assets del ejecutable, con respaldo.</summary>
public static class LogoHelper
{
    private static Image? _cache;
    private static bool _intentado;

    private static readonly string[] NombresPosibles =
    {
        "logo_canar.png", "logo_canar.jpg", "logo_canar.jpeg", "logo.png", "logo.jpg"
    };

    /// <summary>Devuelve el logo si existe el archivo, o null si no se encuentra.</summary>
    public static Image? Logo
    {
        get
        {
            if (_intentado) return _cache;
            _intentado = true;
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var carpetas = new[]
                {
                    Path.Combine(baseDir, "Assets"),
                    baseDir
                };
                foreach (var carpeta in carpetas)
                {
                    foreach (var nombre in NombresPosibles)
                    {
                        var ruta = Path.Combine(carpeta, nombre);
                        if (File.Exists(ruta))
                        {
                            using var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read);
                            _cache = Image.FromStream(fs);
                            return _cache;
                        }
                    }
                }
            }
            catch { _cache = null; }
            return _cache;
        }
    }

    public static bool Existe => Logo != null;
}
