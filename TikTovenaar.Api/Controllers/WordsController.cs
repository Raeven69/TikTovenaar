using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TikTovenaar.Api.Controllers
{
    [Route("words")]
    [ApiController]
    public class WordsController : ControllerBase
    {
        [HttpGet]
        public List<Word> Get()
        {
            Database db = new();
            NpgsqlConnection connection = db.GetConnection();
            connection.Open();
            List<Word> words = [];
            using NpgsqlCommand cmd = new(@"SELECT * FROM words", connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                words.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
            }
            return words;
        }
    }
}
