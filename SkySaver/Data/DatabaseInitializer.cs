using Microsoft.Data.Sqlite;
using System.IO;

namespace SkySaver.Data;

public static class DatabaseInitializer
{
    public static string DbPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SkySaver",
        "skysaver.db");

    public static void Initialize()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(DbPath)!);

        using var connection = new SqliteConnection($"Data Source={DbPath}");
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS PriceAlerts (
                Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                Origin          TEXT    NOT NULL,
                Destination     TEXT    NOT NULL,
                DepartureDate   TEXT    NOT NULL,
                TargetPrice     REAL    NOT NULL,
                LastCheckedPrice REAL,
                CreatedAt       TEXT    NOT NULL,
                LastCheckedAt   TEXT,
                IsActive        INTEGER NOT NULL DEFAULT 1,
                Currency        TEXT    NOT NULL DEFAULT 'EUR'
            );
            """;
        cmd.ExecuteNonQuery();
    }
}
