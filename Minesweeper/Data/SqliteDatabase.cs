using Microsoft.Data.Sqlite;

namespace Minesweeper.Data;

public static class SqliteDatabase
{
    private static readonly string DatabaseFolder =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "Minesweeper");

    private static readonly string DatabasePath =
        Path.Combine(DatabaseFolder, "minesweeper.db");

    private static readonly string ConnectionString =
        new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            ForeignKeys = true
        }.ToString();

    public static SqliteConnection CreateConnection()
    {
        return new SqliteConnection(ConnectionString);
    }

    public static async Task InitializeAsync()
    {
        Directory.CreateDirectory(DatabaseFolder);

        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT NOT NULL UNIQUE,
                password TEXT NOT NULL,
                created_at TEXT NOT NULL DEFAULT (date('now'))
            );

            CREATE TABLE IF NOT EXISTS difficulty_levels (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                level_name TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS records (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id INTEGER NOT NULL,
                level_id INTEGER NOT NULL,
                time_seconds INTEGER NOT NULL
                    CHECK (time_seconds > 0),
                achieved_at TEXT NOT NULL DEFAULT (date('now')),

                FOREIGN KEY (user_id)
                    REFERENCES users(id) ON DELETE CASCADE,

                FOREIGN KEY (level_id)
                    REFERENCES difficulty_levels(id)
            );

            INSERT OR IGNORE INTO difficulty_levels (level_name)
            VALUES
                ('Лёгкий'),
                ('Средний'),
                ('Сложный'),
                ('Кастомный');
            """;

        await using var command =
            new SqliteCommand(sql, connection);

        await command.ExecuteNonQueryAsync();
    }
}