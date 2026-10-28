using Npgsql;

namespace Minesweeper.Data;

public static class AuthService
{
    public static async Task<bool> RegisterAsync(
        string username,
        string password)
    {
        username = username.Trim();

        if (username.Length < 3 || username.Length > 50)
            throw new ArgumentException(
                "Имя пользователя должно содержать от 3 до 50 символов.");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new ArgumentException(
                "Пароль должен содержать не менее 8 символов.");

        string passwordHash = PasswordHasher.HashPassword(password);

        await using var connection = Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO users (username, password)
            VALUES (@username, @password)
            ON CONFLICT (username) DO NOTHING
            RETURNING id;
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("username", username);
        command.Parameters.AddWithValue("password", passwordHash);

        object? result = await command.ExecuteScalarAsync();

        // null означает, что имя уже занято.
        return result != null;
    }

    public static async Task<long?> LoginAsync(
        string username,
        string password)
    {
        username = username.Trim();

        await using var connection = Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            SELECT id, password
            FROM users
            WHERE username = @username;
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("username", username);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        long userId = reader.GetInt64(0);
        string storedPassword = reader.GetString(1);

        if (!PasswordHasher.VerifyPassword(password, storedPassword))
            return null;

        return userId;
    }
}