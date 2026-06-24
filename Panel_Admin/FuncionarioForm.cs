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
            txtCedula.ReadOnly = true; // No editar cédula en modo edición
            txtNombre.Text = datosExistentes.Nombre;
            txtApellido.Text = datosExistentes.Apellido;
            txtCargo.Text = datosExistentes.Cargo ?? "";
            txtDireccion.Text = datosExistentes.Direccion ?? "";
        }

        this.Text = _esEdicion ? "Editar Funcionario" : "Nuevo Funcionario";
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        lblErrorCedula.Text = "";
        lblErrorNombre.Text = "";
        lblErrorApellido.Text = "";

        if (string.IsNullOrWhiteSpace(txtCedula.Text))
        {
            lblErrorCedula.Text = "La cédula es obligatoria.";
            return;
        }
        if (txtCedula.Text.Trim().Length < 10 || txtCedula.Text.Trim().Length > 14)
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
            CedulaRucPersona = txtCedula.Text.Trim(),
            Nombre = txtNombre.Text.Trim(),
            Apellido = txtApellido.Text.Trim(),
            Cargo = string.IsNullOrWhiteSpace(txtCargo.Text) ? null : txtCargo.Text.Trim(),
            Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim()
        };

        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void BtnCancelar_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
