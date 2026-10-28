using System.Windows.Forms;

namespace Minesweeper
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using var loginForm = new Minesweeper.Forms.LoginForm();

            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                using var mainMenu = new Minesweeper.Forms.MainMenuForm(
                    loginForm.LoggedInUserId);

                Application.Run(mainMenu);
            }
        }
    }
}