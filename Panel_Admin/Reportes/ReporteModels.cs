using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;

namespace Panel_Admin.Reportes;

/// <summary>Filtros para la generación de un reporte.</summary>
public class ReporteFiltro
{
    public DateTime Desde { get; set; }
    public DateTime Hasta { get; set; }
    public string? CedulaFuncionario { get; set; }   // null = todos
    public string? Cargo { get; set; }               // null = todos
    public ValorCalificacion? Tipo { get; set; }     // null = todas
    public bool IncluirComentarios { get; set; } = true;
    public bool IncluirDetalle { get; set; } = true;
}

/// <summary>Fila de detalle de una calificación en el reporte.</summary>
public class ReporteFila
{
    public DateTime Fecha { get; set; }
    public string Cedula { get; set; } = "";
    public string Funcionario { get; set; } = "";
    public string Cargo { get; set; } = "";
    public string Valor { get; set; } = "";
    public int ValorNum { get; set; }
    public string Comentario { get; set; } = "";
}

/// <summary>Resumen por funcionario.</summary>
public class ReporteFuncionario
{
    public string Cedula { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Cargo { get; set; } = "";
    public int Total { get; set; }
    public int Excelente { get; set; }
    public int Buena { get; set; }
    public int Regular { get; set; }
    public int Mala { get; set; }
    public double PorcentajeExcelente => Total == 0 ? 0 : (double)Excelente / Total * 100;
    public double IndiceSatisfaccion => Total == 0 ? 0 :
        (Excelente * 100 + Buena * 75 + Regular * 50 + Mala * 25) / (double)Total;
}

/// <summary>Resultado completo del reporte, consumido por la vista previa y las exportaciones.</summary>
public class ReporteResultado
{
    public ReporteFiltro Filtro { get; set; } = new();
    public DateTime GeneradoEn { get; set; } = DateTime.Now;
    public bool FuncionarioUnico { get; set; }
    public string? NombreFuncionario { get; set; }

    public int Total { get; set; }
    public int Excelente { get; set; }
    public int Buena { get; set; }
    public int Regular { get; set; }
    public int Mala { get; set; }

    public double PctExcelente => Total == 0 ? 0 : (double)Excelente / Total * 100;
    public double PctBuena => Total == 0 ? 0 : (double)Buena / Total * 100;
    public double PctRegular => Total == 0 ? 0 : (double)Regular / Total * 100;
    public double PctMala => Total == 0 ? 0 : (double)Mala / Total * 100;

    public List<ReporteFila> Filas { get; set; } = new();
    public List<ReporteFuncionario> PorFuncionario { get; set; } = new();

    public bool TieneDatos => Total > 0;
}
