using IGDB;
using IGDB.Models;

public class IGDBService
{
    private readonly IGDBClient _client;

    public IGDBService()
    {
        var clientId = Environment.GetEnvironmentVariable("IGDB_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("IGDB_CLIENT_SECRET");
        _client = new IGDBClient(clientId, clientSecret);
    }

    public async Task<Game[]?> SearchGamesAsync(string gameName)
    {
        var query = $"fields id, name, first_release_date, total_rating, summary, cover.*; where id = 25657;";

        var result = await _client.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: query);

        System.Diagnostics.Debug.WriteLine($"Found {result?.Length ?? 0} games for {gameName}");

        return result?.ToArray();
    }

}


