using Npgsql;

namespace Minesweeper.Data;

public static class Database
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;" +
        "Database=minesweeper_db;" +
        "Username=postgres;" +
        "Password=1111";
    public static NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(ConnectionString);
    }

    public static async Task TestConnectionAsync()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
    }
}