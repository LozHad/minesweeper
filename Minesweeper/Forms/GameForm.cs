using Minesweeper.Data;

namespace Minesweeper.Forms;

public partial class GameForm : Form
{
    private readonly MinesweeperGame game;
    private readonly Button[,] cellButtons;
    private readonly Label statusLabel;
    private readonly Label timerLabel;
    private readonly System.Windows.Forms.Timer gameTimer;

    private int elapsedSeconds;
    private bool resultShown;
    private readonly long userId;
    private readonly string difficulty;

    public GameForm(
        long userId,
        string difficulty,
        int width,
        int height,
        int mineCount)
    {
        this.userId = userId;
        this.difficulty = difficulty;
        game = new MinesweeperGame(width, height, mineCount);
        cellButtons = new Button[width, height];

        Text = $"Сапёр — {difficulty}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;

        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 55
        };

        statusLabel = new Label
        {
            Text = $"Мины: {mineCount}",
            Location = new Point(10, 15),
            Size = new Size(150, 25),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        timerLabel = new Label
        {
            Text = "Время: 0 сек.",
            Location = new Point(170, 15),
            Size = new Size(150, 25),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        topPanel.Controls.Add(statusLabel);
        topPanel.Controls.Add(timerLabel);

        var boardPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = width,
            RowCount = height,
            Padding = new Padding(5),
            Margin = new Padding(0)
        };

        for (int x = 0; x < width; x++)
        {
            boardPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 32));
        }

        for (int y = 0; y < height; y++)
        {
            boardPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 32));
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int cellX = x;
                int cellY = y;

                var button = new Button
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(1),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Tag = (x, y),
                    BackColor = SystemColors.Control
                };

                button.MouseUp += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        game.ToggleFlag(cellX, cellY);
                        RefreshBoard();
                    }
                    else if (e.Button == MouseButtons.Left)
                    {
                        game.OpenCell(cellX, cellY);
                        RefreshBoard();
                        CheckGameResult();
                    }
                };

                cellButtons[x, y] = button;
                boardPanel.Controls.Add(button, x, y);
            }
        }

        Controls.Add(boardPanel);
        Controls.Add(topPanel);

        gameTimer = new System.Windows.Forms.Timer
        {
            Interval = 1000
        };

        gameTimer.Tick += (sender, e) =>
        {
            if (!game.IsGameOver && game.MinesPlaced)
            {
                elapsedSeconds++;
                timerLabel.Text = $"Время: {elapsedSeconds} сек.";
            }
        };

        gameTimer.Start();
    }

    private void RefreshBoard()
    {
        statusLabel.Text =
            $"Мины: {game.MineCount - game.FlagsCount}";

        for (int y = 0; y < game.Height; y++)
        {
            for (int x = 0; x < game.Width; x++)
            {
                Button button = cellButtons[x, y];

                if (game.IsFlagged(x, y))
                {
                    button.Text = "⚑";
                    button.BackColor = Color.Khaki;
                }
                else if (game.IsOpened(x, y))
                {
                    button.Enabled = false;
                    button.BackColor = Color.LightGray;

                    if (game.HasMine(x, y))
                    {
                        button.Text = "●";
                        button.BackColor = Color.IndianRed;
                    }
                    else
                    {
                        int count = game.CountAdjacentMines(x, y);
                        button.Text = count == 0 ? "" : count.ToString();
                        button.ForeColor = GetNumberColor(count);
                    }
                }
                else if (game.IsGameOver && game.HasMine(x, y))
                {
                    button.Text = "●";
                    button.BackColor = Color.IndianRed;
                }
                else
                {
                    button.Text = "";
                }

                if (game.IsGameOver)
                    button.Enabled = false;
            }
        }
    }

    private static Color GetNumberColor(int count)
    {
        return count switch
        {
            1 => Color.Blue,
            2 => Color.Green,
            3 => Color.Red,
            4 => Color.DarkBlue,
            5 => Color.Brown,
            6 => Color.Teal,
            7 => Color.Black,
            8 => Color.DimGray,
            _ => Color.Black
        };
    }

    private async void CheckGameResult()
    {
        if (!game.IsGameOver || resultShown)
            return;

        resultShown = true;
        gameTimer.Stop();
        RefreshBoard();

        if (game.IsWon)
        {
            try
            {
                await RecordService.SaveRecordAsync(
                    userId,
                    difficulty,
                    elapsedSeconds);

                MessageBox.Show(
                    $"Поздравляем! Ты выиграл!\n" +
                    $"Время: {elapsedSeconds} сек.\n" +
                    "Результат сохранён!",
                    "Победа",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ты выиграл за {elapsedSeconds} сек., " +
                    "но сохранить результат не удалось.\n\n" +
                    ex.Message,
                    "Ошибка сохранения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        else
        {
            MessageBox.Show(
                "Ты попал на мину! Попробуй ещё раз.",
                "Игра окончена",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        gameTimer.Stop();
        gameTimer.Dispose();
        base.OnFormClosed(e);
    }
}