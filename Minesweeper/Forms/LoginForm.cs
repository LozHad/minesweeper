using Minesweeper.Data;

namespace Minesweeper.Forms;

public partial class LoginForm : Form
{
    private readonly TextBox usernameBox;
    private readonly TextBox passwordBox;

    public long LoggedInUserId { get; private set; }

    public LoginForm()
    {
        Text = "Вход в Сапёр";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(380, 245);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var titleLabel = new Label
        {
            Text = "Сапёр",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            Location = new Point(20, 15),
            Size = new Size(340, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var usernameLabel = new Label
        {
            Text = "Имя пользователя:",
            Location = new Point(25, 60),
            AutoSize = true
        };

        usernameBox = new TextBox
        {
            Location = new Point(25, 82),
            Size = new Size(330, 25),
            MaxLength = 50
        };

        var passwordLabel = new Label
        {
            Text = "Пароль:",
            Location = new Point(25, 115),
            AutoSize = true
        };

        passwordBox = new TextBox
        {
            Location = new Point(25, 137),
            Size = new Size(330, 25),
            UseSystemPasswordChar = true
        };

        var loginButton = new Button
        {
            Text = "Войти",
            Location = new Point(25, 180),
            Size = new Size(155, 35)
        };

        var registerButton = new Button
        {
            Text = "Регистрация",
            Location = new Point(200, 180),
            Size = new Size(155, 35)
        };

        loginButton.Click += async (sender, e) =>
        {
            loginButton.Enabled = false;
            registerButton.Enabled = false;

            try
            {
                long? userId = await AuthService.LoginAsync(
                    usernameBox.Text,
                    passwordBox.Text);

                if (userId == null)
                {
                    MessageBox.Show(
                        "Неверное имя пользователя или пароль.");
                    return;
                }

                LoggedInUserId = userId.Value;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка входа",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loginButton.Enabled = true;
                registerButton.Enabled = true;
            }
        };

        registerButton.Click += (sender, e) =>
        {
            using var form = new RegistrationForm();
            form.ShowDialog(this);
        };

        Controls.AddRange(new Control[]
        {
            titleLabel,
            usernameLabel,
            usernameBox,
            passwordLabel,
            passwordBox,
            loginButton,
            registerButton
        });

        AcceptButton = loginButton;
    }
}