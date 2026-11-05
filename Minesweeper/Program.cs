using Minesweeper.Data;

namespace Minesweeper
{
    internal static class Program
    {
        [STAThread]
        static async Task Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                await SqliteDatabase.InitializeAsync();

                using var loginForm = new Minesweeper.Forms.LoginForm();

                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    using var mainMenu = new Minesweeper.Forms.MainMenuForm(
                        loginForm.LoggedInUserId);

                    Application.Run(mainMenu);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось запустить игру:\n\n" + ex.Message,
                    "Ошибка запуска",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}