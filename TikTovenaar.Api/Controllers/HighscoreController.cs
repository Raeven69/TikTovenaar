using Microsoft.AspNetCore.Mvc;
using Npgsql;
using TikTovenaar.Logic;

namespace TikTovenaar.Api.Controllers
{
    [Route("highscores")]
    [ApiController]
    public class HighscoreController : ControllerBase
    {
        [HttpGet]
        public List<PartialScore> Get()
        {
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT * FROM (SELECT DISTINCT ON (players.id) players.name, scores.score FROM players JOIN scores ON players.id = scores.userid ORDER BY players.id, scores.score DESC) highscores ORDER BY score DESC;", connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            List<PartialScore> scores = [];
            while (reader.Read())
            {
                scores.Add(new(reader.GetString(0), reader.GetInt32(1)));
            }
            connection.Close();
            return scores;
        }
    }
}