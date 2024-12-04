using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TikTovenaar.Api.Controllers
{
    [Route("score")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetScores()
        {
            int? userID = Utils.Authorize(HttpContext.Request);
            if (userID == null)
            {
                return new(new { type = "error", message = "Authorization failed." });
            }
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT wordsAmount, time, date, score, incorrectWords, incorrectLetters FROM scores WHERE userid = @userid", connection);
            cmd.Parameters.AddWithValue("userid", userID);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            List<ScoreEntry> scores = [];
            while (reader.Read())
            {
                ScoreEntry entry = new()
                {
                    
                    WordsAmount = reader.GetInt32(0),
                    Time = reader.GetFieldValue<TimeOnly>(1),
                    Date = reader.GetFieldValue<DateOnly>(2),
                    Value = reader.GetInt32(3),
                    IncorrectWords = [..reader.GetFieldValue<string[]>(4)],
                    IncorrectLetters = [..reader.GetFieldValue<char[]>(5)]
                };
                scores.Add(entry);
            }
            connection.Close();
            return new(scores);
        }

        [HttpPost]
        public JsonResult PostScore([FromForm] ScoreEntry score)
        {
            int? userID = Utils.Authorize(HttpContext.Request);
            if (userID == null)
            {
                return new(new { type = "error", message = "Authorization failed." });
            }
            if (!score.IsValid())
            {
                return new(new { type = "error", message = "Malformed data." });
            }
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"INSERT INTO scores(userid, wordsamount, time, date, score, incorrectwords, incorrectletters) VALUES(@userid, @wordsamount, @time, @date, @score, @incorrectwords, @incorrectletters)", connection);
            cmd.Parameters.AddWithValue("userid", userID);
            cmd.Parameters.AddWithValue("wordsamount", score.WordsAmount);
            cmd.Parameters.AddWithValue("time", score.Time);
            cmd.Parameters.AddWithValue("date", score.Date);
            cmd.Parameters.AddWithValue("score", score.Value);
            cmd.Parameters.AddWithValue("incorrectwords", score.IncorrectWords);
            cmd.Parameters.AddWithValue("incorrectletters", score.IncorrectLetters);
            cmd.ExecuteNonQuery();
            connection.Close();
            return new(new { type = "success", message = "Score added." });
        }
    }
}
