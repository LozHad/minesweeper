using Minesweeper.Data;

namespace Minesweeper.Forms;

public partial class RegistrationForm : Form
{
    private readonly TextBox usernameBox;
    private readonly TextBox passwordBox;
    private readonly TextBox confirmPasswordBox;

    public RegistrationForm()
    {
        Text = "Регистрация";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(380, 300);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var titleLabel = new Label
        {
            Text = "Создание аккаунта",
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
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
            Text = "Пароль (не менее 8 символов):",
            Location = new Point(25, 115),
            AutoSize = true
        };

        passwordBox = new TextBox
        {
            Location = new Point(25, 137),
            Size = new Size(330, 25),
            UseSystemPasswordChar = true
        };

        var confirmLabel = new Label
        {
            Text = "Повторите пароль:",
            Location = new Point(25, 170),
            AutoSize = true
        };

        confirmPasswordBox = new TextBox
        {
            Location = new Point(25, 192),
            Size = new Size(330, 25),
            UseSystemPasswordChar = true
        };

        var registerButton = new Button
        {
            Text = "Зарегистрироваться",
            Location = new Point(25, 235),
            Size = new Size(330, 35)
        };

        registerButton.Click += async (sender, e) =>
        {
            registerButton.Enabled = false;

            try
            {
                if (passwordBox.Text != confirmPasswordBox.Text)
                {
                    MessageBox.Show("Пароли не совпадают.");
                    return;
                }

                bool registered = await AuthService.RegisterAsync(
                    usernameBox.Text,
                    passwordBox.Text);

                if (!registered)
                {
                    MessageBox.Show(
                        "Такое имя пользователя уже занято.");
                    return;
                }

                MessageBox.Show(
                    "Регистрация прошла успешно! Теперь войдите в аккаунт.");

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Проверьте данные");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка регистрации",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                registerButton.Enabled = true;
            }
        };

        Controls.AddRange(new Control[]
        {
            titleLabel,
            usernameLabel,
            usernameBox,
            passwordLabel,
            passwordBox,
            confirmLabel,
            confirmPasswordBox,
            registerButton
        });

        AcceptButton = registerButton;
    }
}