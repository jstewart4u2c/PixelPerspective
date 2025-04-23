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
        var query = $@"
                    search ""{gameName}"";
                    fields id, name, first_release_date, aggregated_rating, summary, cover.url, game_type; 
                    where game_type = 0;
                    limit 10;";

        var result = await _client.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: query);

        return result?.ToArray();
    }

    public async Task<Game?> GetGameByIdAsync(long id)
    {
        var query = $@"
                    fields id, name, first_release_date, aggregated_rating, summary, cover.url, videos.video_id;
                    where id = {id};
                    limit 1;";

        var result = await _client.QueryAsync<Game>(IGDBClient.Endpoints.Games, query);
        return result?.FirstOrDefault();
    }

}


