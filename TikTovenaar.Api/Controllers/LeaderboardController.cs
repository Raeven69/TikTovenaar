using Microsoft.AspNetCore.Mvc;
using Npgsql;
using TikTovenaar.Logic;

namespace TikTovenaar.Api.Controllers
{
    [Route("leaderboards")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        [HttpGet]
        public Leaderboards Get([FromQuery] int limit = -1)
        {
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            string lim = (limit > -1) ? limit.ToString() : "NULL";
            using NpgsqlCommand cmd = new(@$"SELECT * FROM (SELECT DISTINCT ON (players.id) players.name, scores.score FROM players JOIN scores ON players.id = scores.userid ORDER BY players.id, scores.score DESC) highscores ORDER BY score DESC LIMIT {lim}", connection);
            cmd.Parameters.AddWithValue("limit", (limit > -1) ? limit.ToString() : "NULL");
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            List<PartialScore<int>> scores = [];
            while (reader.Read())
            {
                scores.Add(new(reader.GetString(0), reader.GetInt32(1)));
            }
            reader.Close();
            using NpgsqlCommand cmd2 = new(@$"SELECT * FROM (SELECT DISTINCT ON (players.id) players.name, scores.wpm FROM players JOIN scores ON players.id = scores.userid ORDER BY players.id, scores.wpm DESC) highscores ORDER BY wpm DESC LIMIT {lim}", connection);
            cmd2.Parameters.AddWithValue("limit", (limit > -1) ? limit.ToString() : "NULL");
            using NpgsqlDataReader reader2 = cmd2.ExecuteReader();
            List<PartialScore<double>> wpm = [];
            while (reader2.Read())
            {
                wpm.Add(new(reader2.GetString(0), reader2.GetDouble(1)));
            }
            reader2.Close();
            using NpgsqlCommand cmd3 = new(@$"SELECT * FROM players ORDER BY level DESC LIMIT {lim}", connection);
            cmd3.Parameters.AddWithValue("limit", (limit > -1) ? limit.ToString() : "NULL");
            using NpgsqlDataReader reader3 = cmd3.ExecuteReader();
            List<PartialScore<int>> levels = [];
            while (reader3.Read())
            {
                levels.Add(new(reader3.GetString(1), reader3.GetInt32(4)));
            }
            connection.Close();
            return new(scores, wpm, levels);
        }
    }
}