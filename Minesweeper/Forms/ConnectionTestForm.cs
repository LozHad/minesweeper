using Minesweeper.Data;

namespace Minesweeper.Forms;

public partial class ConnectionTestForm : Form
{
    private readonly Label statusLabel;
    private readonly Button testButton;

    public ConnectionTestForm()
    {
        Text = "Проверка PostgreSQL";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(420, 160);

        statusLabel = new Label
        {
            Text = "Нажми кнопку для проверки соединения.",
            Location = new Point(20, 20),
            Size = new Size(380, 40),
            TextAlign = ContentAlignment.MiddleCenter
        };

        testButton = new Button
        {
            Text = "Проверить подключение",
            Location = new Point(100, 80),
            Size = new Size(220, 35)
        };

        testButton.Click += async (sender, e) =>
        {
            testButton.Enabled = false;
            statusLabel.Text = "Подключение...";

            try
            {
                await Database.TestConnectionAsync();
                statusLabel.Text = "Подключение успешно!";
                statusLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Ошибка подключения. Подробности в сообщении.";
                MessageBox.Show(
                    ex.Message,
                    "Ошибка PostgreSQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                testButton.Enabled = true;
            }
        };

        Controls.Add(statusLabel);
        Controls.Add(testButton);
    }
}