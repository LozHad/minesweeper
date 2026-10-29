using Npgsql;

namespace Minesweeper.Data;

public class PlayerRecord
{
    public string Difficulty { get; set; } = "";
    public int TimeSeconds { get; set; }
    public System.DateTime AchievedAt { get; set; }
}

public static class StatisticsService
{
    public static async Task<List<PlayerRecord>> GetRecordsAsync(long userId)
    {
        var records = new List<PlayerRecord>();

        await using var connection = Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            SELECT d.level_name, r.time_seconds, r.achieved_at
            FROM records AS r
            JOIN difficulty_levels AS d ON d.id = r.level_id
            WHERE r.user_id = @userId
            ORDER BY r.achieved_at DESC, r.time_seconds ASC;
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("userId", userId);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            records.Add(new PlayerRecord
            {
                Difficulty = reader.GetString(0),
                TimeSeconds = reader.GetInt32(1),
                AchievedAt = reader.GetDateTime(2)
            });
        }

        return records;
    }
}