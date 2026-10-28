namespace Minesweeper.Forms;

public partial class DifficultyForm : Form
{
    private readonly ComboBox difficultyBox;
    private readonly NumericUpDown widthBox;
    private readonly NumericUpDown heightBox;
    private readonly NumericUpDown minesBox;

    public string SelectedDifficulty =>
        difficultyBox.SelectedItem?.ToString() ?? "Лёгкий";

    public int SelectedWidth => (int)widthBox.Value;
    public int SelectedHeight => (int)heightBox.Value;
    public int SelectedMines => (int)minesBox.Value;

    public DifficultyForm()
    {
        Text = "Выбор сложности";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(380, 340);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var difficultyLabel = new Label
        {
            Text = "Уровень сложности:",
            Location = new Point(25, 20),
            AutoSize = true
        };

        difficultyBox = new ComboBox
        {
            Location = new Point(25, 45),
            Size = new Size(320, 30),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        difficultyBox.Items.AddRange(new object[]
        {
            "Лёгкий",
            "Средний",
            "Сложный",
            "Кастомный"
        });

        difficultyBox.SelectedIndex = 0;

        var widthLabel = CreateLabel("Ширина поля:", 90);
        widthBox = CreateNumberBox(210, 88, 5, 30, 9);

        var heightLabel = CreateLabel("Высота поля:", 130);
        heightBox = CreateNumberBox(210, 128, 5, 24, 9);

        var minesLabel = CreateLabel("Количество мин:", 170);
        minesBox = CreateNumberBox(210, 168, 1, 600, 10);

        difficultyBox.SelectedIndexChanged += (sender, e) =>
        {
            ApplyPreset();
        };

        widthBox.ValueChanged += (sender, e) => UpdateMineLimit();
        heightBox.ValueChanged += (sender, e) => UpdateMineLimit();

        var startButton = new Button
        {
            Text = "Начать игру",
            Location = new Point(25, 235),
            Size = new Size(320, 40)
        };

        startButton.Click += (sender, e) =>
        {
            if (minesBox.Value >= widthBox.Value * heightBox.Value)
            {
                MessageBox.Show(
                    "Количество мин должно быть меньше количества клеток.");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        };

        var cancelButton = new Button
        {
            Text = "Отмена",
            Location = new Point(25, 280),
            Size = new Size(320, 35)
        };

        cancelButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.AddRange(new Control[]
        {
            difficultyLabel,
            difficultyBox,
            widthLabel,
            widthBox,
            heightLabel,
            heightBox,
            minesLabel,
            minesBox,
            startButton,
            cancelButton
        });

        ApplyPreset();
    }

    private static Label CreateLabel(string text, int top)
    {
        return new Label
        {
            Text = text,
            Location = new Point(25, top),
            Size = new Size(175, 25),
            TextAlign = ContentAlignment.MiddleLeft
        };
    }

    private static NumericUpDown CreateNumberBox(
        int left, int top, int minimum, int maximum, int value)
    {
        return new NumericUpDown
        {
            Location = new Point(left, top),
            Size = new Size(135, 25),
            Minimum = minimum,
            Maximum = maximum,
            Value = value
        };
    }

    private void ApplyPreset()
    {
        switch (SelectedDifficulty)
        {
            case "Лёгкий":
                SetValues(9, 9, 10);
                break;
            case "Средний":
                SetValues(16, 16, 40);
                break;

            case "Сложный":
                SetValues(30, 16, 99);
                break;

            case "Кастомный":
                // Пользователь задаёт размеры и количество мин.
                break;
        }
    }

    private void SetValues(int width, int height, int mines)
    {
        widthBox.Value = width;
        heightBox.Value = height;
        UpdateMineLimit();
        minesBox.Value = mines;
    }

    private void UpdateMineLimit()
    {
        decimal maxMines = widthBox.Value * heightBox.Value - 1;

        minesBox.Maximum = Math.Max(1, (int)maxMines);

        if (minesBox.Value > minesBox.Maximum)
            minesBox.Value = minesBox.Maximum;
    }
}