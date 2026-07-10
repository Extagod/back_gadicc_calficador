using System.Drawing;
using System.Windows.Forms;

namespace Panel_Admin.UI;

/// <summary>Aplica un estilo moderno y consistente a un DataGridView.</summary>
public static class GridStyler
{
    public static void Apply(DataGridView grid)
    {
        grid.BackgroundColor = UITheme.Card;
        grid.BorderStyle = BorderStyle.None;
        grid.GridColor = UITheme.Border;
        grid.EnableHeadersVisualStyles = false;
        grid.RowHeadersVisible = false;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.ColumnHeadersHeight = 44;
        grid.RowTemplate.Height = 40;

        // Cabecera
        grid.ColumnHeadersDefaultCellStyle.BackColor = UITheme.GridHeader;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font(UITheme.FontFamilySemibold, 10f, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UITheme.GridHeader;

        // Celdas
        grid.DefaultCellStyle.BackColor = UITheme.Card;
        grid.DefaultCellStyle.ForeColor = UITheme.TextPrimary;
        grid.DefaultCellStyle.Font = UITheme.Body;
        grid.DefaultCellStyle.SelectionBackColor = UITheme.GridSelection;
        grid.DefaultCellStyle.SelectionForeColor = UITheme.GridSelectionText;
        grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        grid.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);

        grid.AlternatingRowsDefaultCellStyle.BackColor = UITheme.GridRowAlt;
        grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = UITheme.GridSelection;
        grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = UITheme.GridSelectionText;

        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
    }
}
