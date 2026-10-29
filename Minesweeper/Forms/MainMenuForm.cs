namespace Minesweeper.Forms;

public partial class MainMenuForm : Form
{
    private readonly long userId;

    public MainMenuForm(long userId)
    {
        this.userId = userId;

        Text = "Сапёр — главное меню";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(420, 440);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Color.FromArgb(238, 242, 246);

        var title = new Label
        {
            Text = "САПЁР",
            Font = new Font("Segoe UI", 26, FontStyle.Bold),
            ForeColor = Color.FromArgb(35, 55, 80),
            Location = new Point(30, 25),
            Size = new Size(360, 55),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var subtitle = new Label
        {
            Text = "Найди все мины и не взорвись!",
            Font = new Font("Segoe UI", 10),
            Location = new Point(30, 80),
            Size = new Size(360, 30),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var newGameButton = CreateButton("Новая игра", 125);
        var statisticsButton = CreateButton("Моя статистика", 190);
        var settingsButton = CreateButton("Настройки", 255);
        var exitButton = CreateButton("Выход", 320);

        newGameButton.Click += (sender, e) =>
        {
            using var difficultyForm = new DifficultyForm();

            if (difficultyForm.ShowDialog(this) != DialogResult.OK)
                return;

            using var gameForm = new GameForm(
                userId,
                difficultyForm.SelectedDifficulty,
                difficultyForm.SelectedWidth,
                difficultyForm.SelectedHeight,
                difficultyForm.SelectedMines);

            gameForm.ShowDialog(this);
        };

        statisticsButton.Click += (sender, e) =>
        {
            using var statiscticsForm = new StatisticsForm(userId);
            statiscticsForm.ShowDialog(this);
        };

        settingsButton.Click += (sender, e) =>
        {
            MessageBox.Show(
                "Настройки оформления добавим на следующем этапе.",
                "Настройки");
        };

        exitButton.Click += (sender, e) => Close();

        Controls.AddRange(new Control[]
        {
            title,
            subtitle,
            newGameButton,
            statisticsButton,
            settingsButton,
            exitButton
        });
    }

    private static Button CreateButton(string text, int top)
    {
        return new Button
        {
            Text = text,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Location = new Point(80, top),
            Size = new Size(260, 48),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(35, 55, 80),
            Cursor = Cursors.Hand
        };
    }
}