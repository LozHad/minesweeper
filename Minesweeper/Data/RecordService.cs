using Microsoft.Data.Sqlite;

namespace Minesweeper.Data;

public static class RecordService
{
    public static async Task SaveRecordAsync(
        long userId,
        string difficulty,
        int timeSeconds)
    {
        if (timeSeconds < 1)
            timeSeconds = 1;

        await using var connection = SqliteDatabase.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO records (user_id, level_id, time_seconds)
            SELECT @userId, id, @timeSeconds
            FROM difficulty_levels
            WHERE level_name = @difficulty;
            """;

        await using var command = new SqliteCommand(sql, connection);

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@difficulty", difficulty);
        command.Parameters.AddWithValue("@timeSeconds", timeSeconds);

        int rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException(
                $"Уровень сложности «{difficulty}» не найден в базе данных.");
        }
    }
}