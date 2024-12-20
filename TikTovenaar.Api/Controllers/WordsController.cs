﻿using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TikTovenaar.Api.Controllers
{
    [Route("words")]
    [ApiController]
    public class WordsController : ControllerBase
    {
        List<Word> Words { get; } = [];

        public WordsController()
        {
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT * FROM words", connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Words.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
            }
            connection.Close();
        }

        [HttpGet]
        public List<Word> Get([FromQuery] int limit = -1, [FromQuery] int minLength = 0, [FromQuery] int maxLength = -1)
        {
            List<Word> words = Words.Where((Word word) => word.Length >= minLength && (maxLength == -1 || word.Length <= maxLength)).ToList();
            if (words.Count > 0)
            {
                return [.. Random.Shared.GetItems([..words], (limit == -1) ? words.Count : ((words.Count > limit) ? limit : words.Count))];
            }
            return [];
        }
    }
}
