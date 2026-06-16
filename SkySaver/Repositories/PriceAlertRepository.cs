using Microsoft.Data.Sqlite;
using SkySaver.Data;
using SkySaver.Models;

namespace SkySaver.Repositories;

public class PriceAlertRepository : IPriceAlertRepository
{
    private string ConnectionString => $"Data Source={DatabaseInitializer.DbPath}";

    public async Task<IEnumerable<PriceAlert>> GetAllAsync()
    {
        var alerts = new List<PriceAlert>();
        await using var conn = new SqliteConnection(ConnectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM PriceAlerts ORDER BY CreatedAt DESC";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            alerts.Add(MapRow(reader));

        return alerts;
    }

    public async Task<IEnumerable<PriceAlert>> GetActiveAsync()
    {
        var alerts = new List<PriceAlert>();
        await using var conn = new SqliteConnection(ConnectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM PriceAlerts WHERE IsActive = 1";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            alerts.Add(MapRow(reader));

        return alerts;
    }

    public async Task<int> AddAsync(PriceAlert alert)
    {
        await using var conn = new SqliteConnection(ConnectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO PriceAlerts (Origin, Destination, DepartureDate, TargetPrice, CreatedAt, IsActive, Currency)
            VALUES ($origin, $dest, $date, $target, $created, 1, $currency);
            SELECT last_insert_rowid();
            """;
        cmd.Parameters.AddWithValue("$origin", alert.Origin);
        cmd.Parameters.AddWithValue("$dest", alert.Destination);
        cmd.Parameters.AddWithValue("$date", alert.DepartureDate.ToString("o"));
        cmd.Parameters.AddWithValue("$target", (double)alert.TargetPrice);
        cmd.Parameters.AddWithValue("$created", alert.CreatedAt.ToString("o"));
        cmd.Parameters.AddWithValue("$currency", alert.Currency);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task UpdateAsync(PriceAlert alert)
    {
        await using var conn = new SqliteConnection(ConnectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE PriceAlerts
            SET LastCheckedPrice = $lastPrice,
                LastCheckedAt    = $lastChecked,
                IsActive         = $active
            WHERE Id = $id
            """;
        cmd.Parameters.AddWithValue("$lastPrice", alert.LastCheckedPrice.HasValue ? (object)(double)alert.LastCheckedPrice.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("$lastChecked", alert.LastCheckedAt.HasValue ? (object)alert.LastCheckedAt.Value.ToString("o") : DBNull.Value);
        cmd.Parameters.AddWithValue("$active", alert.IsActive ? 1 : 0);
        cmd.Parameters.AddWithValue("$id", alert.Id);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var conn = new SqliteConnection(ConnectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM PriceAlerts WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    private static PriceAlert MapRow(SqliteDataReader r) => new()
    {
        Id = r.GetInt32(r.GetOrdinal("Id")),
        Origin = r.GetString(r.GetOrdinal("Origin")),
        Destination = r.GetString(r.GetOrdinal("Destination")),
        DepartureDate = DateTime.Parse(r.GetString(r.GetOrdinal("DepartureDate"))),
        TargetPrice = (decimal)r.GetDouble(r.GetOrdinal("TargetPrice")),
        LastCheckedPrice = r.IsDBNull(r.GetOrdinal("LastCheckedPrice")) ? null : (decimal)r.GetDouble(r.GetOrdinal("LastCheckedPrice")),
        CreatedAt = DateTime.Parse(r.GetString(r.GetOrdinal("CreatedAt"))),
        LastCheckedAt = r.IsDBNull(r.GetOrdinal("LastCheckedAt")) ? null : DateTime.Parse(r.GetString(r.GetOrdinal("LastCheckedAt"))),
        IsActive = r.GetInt32(r.GetOrdinal("IsActive")) == 1,
        Currency = r.GetString(r.GetOrdinal("Currency")),
    };
}
