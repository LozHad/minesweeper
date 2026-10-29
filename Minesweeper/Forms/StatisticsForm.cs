using Minesweeper.Data;

namespace Minesweeper.Forms;

public partial class StatisticsForm : Form
{
    private readonly long userId;
    private readonly Label summaryLabel;
    private readonly DataGridView recordsGrid;

    public StatisticsForm(long userId)
    {
        this.userId = userId;

        Text = "Моя статистика";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(650, 430);
        MinimumSize = new Size(550, 350);

        summaryLabel = new Label
        {
            Text = "Загружаем статистику...",
            Location = new Point(15, 15),
            Size = new Size(610, 45),
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        recordsGrid = new DataGridView
        {
            Location = new Point(15, 70),
            Size = new Size(610, 290),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            AutoGenerateColumns = false
        };

        recordsGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Сложность",
            DataPropertyName = nameof(PlayerRecord.Difficulty)
        });

        recordsGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Время (сек.)",
            DataPropertyName = nameof(PlayerRecord.TimeSeconds)
        });

        recordsGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Дата",
            DataPropertyName = nameof(PlayerRecord.AchievedAt),
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "dd.MM.yyyy"
            }
        });

        var refreshButton = new Button
        {
            Text = "Обновить",
            Location = new Point(15, 370),
            Size = new Size(120, 35)
        };

        refreshButton.Click += async (sender, e) =>
        {
            refreshButton.Enabled = false;

            try
            {
                await LoadStatisticsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка загрузки статистики",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                refreshButton.Enabled = true;
            }
        };

        Controls.Add(summaryLabel);
        Controls.Add(recordsGrid);
        Controls.Add(refreshButton);

        Load += async (sender, e) =>
        {
            try
            {
                await LoadStatisticsAsync();
            }
            catch (Exception ex)
            {
                summaryLabel.Text = "Не удалось загрузить статистику.";

                MessageBox.Show(
                    ex.Message,
                    "Ошибка загрузки статистики",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        };
    }

    private async Task LoadStatisticsAsync()
    {
        var records = await StatisticsService.GetRecordsAsync(userId);

        int totalWins = records.Count;

        summaryLabel.Text = $"Всего побед: {totalWins}";

        recordsGrid.DataSource = null;
        recordsGrid.DataSource = records;

        if (totalWins == 0)
        {
            summaryLabel.Text =
                "Пока нет побед. Сыграй партию, чтобы появился первый результат!";
            return;
        }

        string[] difficulties =
        {
            "Лёгкий",
            "Средний",
            "Сложный",
            "Кастомный"
        };
        string bestResults = string.Join(
            "   |   ",
            difficulties.Select(difficulty =>
            {
                var best = records
                    .Where(r => r.Difficulty == difficulty)
                    .OrderBy(r => r.TimeSeconds)
                    .FirstOrDefault();

                return best == null
                    ? $"{difficulty}: —"
                    : $"{difficulty}: {best.TimeSeconds} сек.";
            }));

        summaryLabel.Text =
            $"Побед: {totalWins}\nЛучшее время — {bestResults}";
    }
}