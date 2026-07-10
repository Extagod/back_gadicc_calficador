using Capa_Abstracciones.DTOs;

namespace Panel_Admin;

public partial class FuncionarioForm : Form
{
    public CrearEncargadoDto? Resultado { get; private set; }
    private readonly bool _esEdicion;

    public FuncionarioForm(CrearEncargadoDto? datosExistentes = null)
    {
        _esEdicion = datosExistentes is not null;
        InitializeComponent();

        if (datosExistentes is not null)
        {
            txtCedula.Text = datosExistentes.CedulaRucPersona;
            txtCedula.Inner.ReadOnly = true;
            txtCedula.Inner.BackColor = UI.UITheme.Background;
            txtNombre.Text = datosExistentes.Nombre;
            txtApellido.Text = datosExistentes.Apellido;
            txtCargo.Text = datosExistentes.Cargo ?? "";
            txtDireccion.Text = datosExistentes.Direccion ?? "";
            lblTitulo.Text = "Editar Funcionario";
        }
        else
        {
            lblTitulo.Text = "Nuevo Funcionario";
        }
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        lblErrorCedula.Text = "";
        lblErrorNombre.Text = "";
        lblErrorApellido.Text = "";

        var cedula = txtCedula.Text.Trim();
        if (string.IsNullOrWhiteSpace(cedula))
        {
            lblErrorCedula.Text = "La cédula es obligatoria.";
            return;
        }
        if (cedula.Length < 10 || cedula.Length > 14)
        {
            lblErrorCedula.Text = "La cédula debe tener entre 10 y 14 caracteres.";
            return;
        }
        if (string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            lblErrorNombre.Text = "El nombre es obligatorio.";
            return;
        }
        if (string.IsNullOrWhiteSpace(txtApellido.Text))
        {
            lblErrorApellido.Text = "El apellido es obligatorio.";
            return;
        }

        Resultado = new CrearEncargadoDto
        {
            CedulaRucPersona = cedula,
            Nombre = txtNombre.Text.Trim(),
            Apellido = txtApellido.Text.Trim(),
            Cargo = string.IsNullOrWhiteSpace(txtCargo.Text) ? null : txtCargo.Text.Trim(),
            Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim()
        };

        DialogResult = DialogResult.OK;
        Close();
    }

    private void BtnCancelar_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
