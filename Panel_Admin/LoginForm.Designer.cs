namespace Panel_Admin;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;
    private TextBox txtUsuario;
    private TextBox txtPassword;
    private Button btnLogin;
    private Label lblUsuario;
    private Label lblPassword;
    private Label lblErrorUsuario;
    private Label lblErrorPassword;
    private Label lblError;
    private Label lblTitulo;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.txtUsuario = new TextBox();
        this.txtPassword = new TextBox();
        this.btnLogin = new Button();
        this.lblUsuario = new Label();
        this.lblPassword = new Label();
        this.lblErrorUsuario = new Label();
        this.lblErrorPassword = new Label();
        this.lblError = new Label();
        this.lblTitulo = new Label();
        this.SuspendLayout();

        // lblTitulo
        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        this.lblTitulo.Location = new Point(100, 20);
        this.lblTitulo.Text = "Panel Administrativo";

        // lblUsuario
        this.lblUsuario.AutoSize = true;
        this.lblUsuario.Location = new Point(50, 70);
        this.lblUsuario.Text = "Usuario:";

        // txtUsuario
        this.txtUsuario.Location = new Point(50, 90);
        this.txtUsuario.MaxLength = 50;
        this.txtUsuario.Size = new Size(280, 23);

        // lblErrorUsuario
        this.lblErrorUsuario.AutoSize = true;
        this.lblErrorUsuario.ForeColor = Color.Red;
        this.lblErrorUsuario.Location = new Point(50, 115);
        this.lblErrorUsuario.Text = "";

        // lblPassword
        this.lblPassword.AutoSize = true;
        this.lblPassword.Location = new Point(50, 140);
        this.lblPassword.Text = "Contraseña:";

        // txtPassword
        this.txtPassword.Location = new Point(50, 160);
        this.txtPassword.MaxLength = 128;
        this.txtPassword.PasswordChar = '*';
        this.txtPassword.Size = new Size(280, 23);

        // lblErrorPassword
        this.lblErrorPassword.AutoSize = true;
        this.lblErrorPassword.ForeColor = Color.Red;
        this.lblErrorPassword.Location = new Point(50, 185);
        this.lblErrorPassword.Text = "";

        // btnLogin
        this.btnLogin.Location = new Point(130, 220);
        this.btnLogin.Size = new Size(120, 35);
        this.btnLogin.Text = "Iniciar Sesión";
        this.btnLogin.Click += BtnLogin_Click;

        // lblError
        this.lblError.AutoSize = true;
        this.lblError.ForeColor = Color.Red;
        this.lblError.Location = new Point(50, 270);
        this.lblError.MaximumSize = new Size(280, 0);
        this.lblError.Text = "";

        // LoginForm
        this.ClientSize = new Size(380, 320);
        this.Controls.Add(this.lblTitulo);
        this.Controls.Add(this.lblUsuario);
        this.Controls.Add(this.txtUsuario);
        this.Controls.Add(this.lblErrorUsuario);
        this.Controls.Add(this.lblPassword);
        this.Controls.Add(this.txtPassword);
        this.Controls.Add(this.lblErrorPassword);
        this.Controls.Add(this.btnLogin);
        this.Controls.Add(this.lblError);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Login - Panel Administrativo";
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
