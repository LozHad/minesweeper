namespace Minesweeper.Data;

public class MinesweeperGame
{
    private readonly Random random = new();

    private bool[,] mines = new bool[1, 1];
    private bool[,] opened = new bool[1, 1];
    private bool[,] flagged = new bool[1, 1];

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int MineCount { get; private set; }

    public bool IsGameOver { get; private set; }
    public bool IsWon { get; private set; }
    public bool MinesPlaced { get; private set; }

    public int FlagsCount
    {
        get
        {
            int count = 0;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (flagged[x, y])
                        count++;

            return count;
        }
    }

    public MinesweeperGame(int width, int height, int mineCount)
    {
        if (width < 1 || height < 1 ||
            mineCount < 1 || mineCount >= width * height)
        {
            throw new ArgumentException("Некорректные параметры игры.");
        }

        Width = width;
        Height = height;
        MineCount = mineCount;

        mines = new bool[Width, Height];
        opened = new bool[Width, Height];
        flagged = new bool[Width, Height];
    }

    public bool IsOpened(int x, int y) => opened[x, y];

    public bool IsFlagged(int x, int y) => flagged[x, y];

    public bool HasMine(int x, int y) => mines[x, y];

    public int CountAdjacentMines(int x, int y)
    {
        int count = 0;

        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                int nx = x + dx;
                int ny = y + dy;

                if (IsInside(nx, ny) && mines[nx, ny])
                    count++;
            }
        }

        return count;
    }

    public void ToggleFlag(int x, int y)
    {
        if (IsGameOver || opened[x, y])
            return;

        flagged[x, y] = !flagged[x, y];
    }

    public void OpenCell(int x, int y)
    {
        if (IsGameOver || opened[x, y] || flagged[x, y])
            return;

        // Первый клик всегда безопасен.
        if (!MinesPlaced)
        {
            PlaceMines(x, y);
            MinesPlaced = true;
        }

        if (mines[x, y])
        {
            IsGameOver = true;
            IsWon = false;
            return;
        }

        OpenEmptyArea(x, y);
        CheckWin();
    }

    private void PlaceMines(int safeX, int safeY)
    {
        var positions = new List<(int x, int y)>();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (x == safeX && y == safeY)
                    continue;

                positions.Add((x, y));
            }
        }

        // Перемешиваем список, чтобы случайно выбрать клетки для мин.
        for (int i = positions.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
        }

        for (int i = 0; i < MineCount; i++)
        {
            var position = positions[i];
            mines[position.x, position.y] = true;
        }
    }

    private void OpenEmptyArea(int startX, int startY)
    {
        var queue = new Queue<(int x, int y)>();
        queue.Enqueue((startX, startY));

        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            int x = cell.x;
            int y = cell.y;

            if (!IsInside(x, y) || opened[x, y] || flagged[x, y])
                continue;

            if (mines[x, y])
                continue;

            opened[x, y] = true;

            if (CountAdjacentMines(x, y) != 0)
                continue;

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (IsInside(nx, ny) && !opened[nx, ny])
                        queue.Enqueue((nx, ny));
                }
            }
        }
    }

    private void CheckWin()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (!mines[x, y] && !opened[x, y])
                    return;
            }
        }

        IsWon = true;
        IsGameOver = true;
    }

    private bool IsInside(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
}