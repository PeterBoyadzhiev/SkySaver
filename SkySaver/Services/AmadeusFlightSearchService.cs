using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using SkySaver.Models;

namespace SkySaver.Services;

public class AmadeusFlightSearchService : IFlightSearchService
{
    private readonly HttpClient _http;
    private readonly string _clientId;
    private readonly string _clientSecret;

    private string? _accessToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public AmadeusFlightSearchService(HttpClient http, string clientId, string clientSecret)
    {
        _http = http;
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(SearchQuery query)
    {
        await EnsureTokenAsync();

        var url = $"https://test.api.amadeus.com/v2/shopping/flight-offers" +
                  $"?originLocationCode={Uri.EscapeDataString(query.Origin.ToUpper())}" +
                  $"&destinationLocationCode={Uri.EscapeDataString(query.Destination.ToUpper())}" +
                  $"&departureDate={query.DepartureDate:yyyy-MM-dd}" +
                  $"&adults={query.Adults}" +
                  $"&max=10" +
                  $"&currencyCode=EUR";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

        using var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AmadeusFlightResponse>(raw, JsonOpts);

        return result?.Data.Select(o => MapOffer(o, result.Dictionaries)) ?? [];
    }

    public async Task<decimal?> GetLowestPriceAsync(string origin, string destination, DateTime date)
    {
        var flights = await SearchFlightsAsync(new SearchQuery
        {
            Origin = origin,
            Destination = destination,
            DepartureDate = date,
        });
        var list = flights.ToList();
        return list.Count == 0 ? null : list.Min(f => f.Price);
    }

    private async Task EnsureTokenAsync()
    {
        if (_accessToken != null && DateTime.UtcNow < _tokenExpiry)
            return;

        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
        });

        using var response = await _http.PostAsync("https://test.api.amadeus.com/v1/security/oauth2/token", body);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<AmadeusTokenResponse>(JsonOpts)
            ?? throw new InvalidOperationException("Failed to obtain Amadeus access token.");

        _accessToken = token.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(token.ExpiresIn - 30);
    }

    private static Flight MapOffer(AmadeusOffer offer, AmadeusDictionaries? dict)
    {
        var itinerary = offer.Itineraries.First();
        var firstSeg = itinerary.Segments.First();
        var lastSeg = itinerary.Segments.Last();
        var airlineCode = offer.ValidatingAirlineCodes.FirstOrDefault() ?? firstSeg.CarrierCode;
        var airlineName = dict?.Carriers.GetValueOrDefault(airlineCode) ?? airlineCode;
        var duration = ParseIsoDuration(itinerary.Duration);

        return new Flight
        {
            Id = offer.Id,
            Airline = airlineName,
            AirlineCode = airlineCode,
            Origin = firstSeg.Departure.IataCode,
            Destination = lastSeg.Arrival.IataCode,
            DepartureTime = firstSeg.Departure.At,
            ArrivalTime = lastSeg.Arrival.At,
            Duration = duration,
            Price = decimal.Parse(offer.Price.Total, System.Globalization.CultureInfo.InvariantCulture),
            Currency = offer.Price.Currency,
            Stops = itinerary.Segments.Count - 1,
        };
    }

    private static TimeSpan ParseIsoDuration(string iso)
    {
        // PT2H30M -> 2h30m
        if (string.IsNullOrEmpty(iso)) return TimeSpan.Zero;
        var s = iso.TrimStart('P', 'T');
        int hours = 0, minutes = 0;
        var hIdx = s.IndexOf('H');
        if (hIdx >= 0) { int.TryParse(s[..hIdx], out hours); s = s[(hIdx + 1)..]; }
        var mIdx = s.IndexOf('M');
        if (mIdx >= 0) int.TryParse(s[..mIdx], out minutes);
        return new TimeSpan(hours, minutes, 0);
    }
}
