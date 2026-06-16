using System.Text.Json.Serialization;

namespace SkySaver.Models;

public class AviationStackResponse
{
    [JsonPropertyName("data")]
    public List<AviationStackFlight> Data { get; set; } = new List<AviationStackFlight>();
}

public class AviationStackFlight
{
    [JsonPropertyName("flight")]
    public AviationStackFlightId? Flight { get; set; }

    [JsonPropertyName("airline")]
    public AviationStackAirline? Airline { get; set; }

    [JsonPropertyName("departure")]
    public AviationStackEndpoint? Departure { get; set; }

    [JsonPropertyName("arrival")]
    public AviationStackEndpoint? Arrival { get; set; }

    [JsonPropertyName("flight_status")]
    public string? FlightStatus { get; set; }
}

public class AviationStackFlightId
{
    [JsonPropertyName("iata")]
    public string? Iata { get; set; }
}

public class AviationStackAirline
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("iata")]
    public string? Iata { get; set; }
}

public class AviationStackEndpoint
{
    [JsonPropertyName("iata")]
    public string? Iata { get; set; }

    [JsonPropertyName("scheduled")]
    public DateTime? Scheduled { get; set; }
}
