using System.Text.Json.Serialization;

namespace SkySaver.Models;

// Amadeus API response shapes

public class AmadeusTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

public class AmadeusFlightResponse
{
    [JsonPropertyName("data")]
    public List<AmadeusOffer> Data { get; set; } = [];

    [JsonPropertyName("dictionaries")]
    public AmadeusDictionaries? Dictionaries { get; set; }
}

public class AmadeusOffer
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("itineraries")]
    public List<AmadeusItinerary> Itineraries { get; set; } = [];

    [JsonPropertyName("price")]
    public AmadeusPrice Price { get; set; } = new();

    [JsonPropertyName("validatingAirlineCodes")]
    public List<string> ValidatingAirlineCodes { get; set; } = [];
}

public class AmadeusItinerary
{
    [JsonPropertyName("duration")]
    public string Duration { get; set; } = string.Empty;

    [JsonPropertyName("segments")]
    public List<AmadeusSegment> Segments { get; set; } = [];
}

public class AmadeusSegment
{
    [JsonPropertyName("departure")]
    public AmadeusEndpoint Departure { get; set; } = new();

    [JsonPropertyName("arrival")]
    public AmadeusEndpoint Arrival { get; set; } = new();

    [JsonPropertyName("carrierCode")]
    public string CarrierCode { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;
}

public class AmadeusEndpoint
{
    [JsonPropertyName("iataCode")]
    public string IataCode { get; set; } = string.Empty;

    [JsonPropertyName("at")]
    public DateTime At { get; set; }
}

public class AmadeusPrice
{
    [JsonPropertyName("total")]
    public string Total { get; set; } = "0";

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "EUR";
}

public class AmadeusDictionaries
{
    [JsonPropertyName("carriers")]
    public Dictionary<string, string> Carriers { get; set; } = [];
}
