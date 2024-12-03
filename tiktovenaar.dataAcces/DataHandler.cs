using Newtonsoft.Json;
using TikTovenaar.Logic;
using System.Net.Http.Headers;

namespace TikTovenaar.DataAccess
{
    public class DataHandler : IDataHandler
    {
        private readonly HttpClient client;

        public DataHandler()
        {
            client = new()
            {
                BaseAddress = new("https://tiktovenaar.nl/api/")
            };
        }

        public void AddScore(string token, Score score)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, "score");
            request.Headers.Authorization = new("Bearer", token);
            request.Content = new FormUrlEncodedContent([
                new("wordsAmount", score.WordsAmount.ToString()),
                new("time", score.Time.ToLongTimeString()),
                new("date", score.Date.ToLongDateString()),
                new("value", score.Value.ToString()),
                new("incorrectWords", string.Join(',', score.IncorrectWords)),
                new("incorrectLetters", string.Join(',', score.IncorrectLetters))
            ]);
            client.SendAsync(request).Wait();
        }

        public List<PartialScore> GetHighscores()
        {
            HttpResponseMessage response = client.GetAsync("highscores").Result;
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            List<PartialScore> scores = [];
            try
            {
                foreach (dynamic score in result!)
                {
                    scores.Add(new(score.player, score.value));
                }
            }
            catch (Exception) {}
            return scores;
        }

        public List<Score> GetScores(string token)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, "score");
            request.Headers.Authorization = new("Bearer", token);
            HttpResponseMessage response = client.SendAsync(request).Result;
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            List<Score> scores = [];
            try
            {
                foreach (dynamic score in result!)
                {
                    List<string> incorrectWords = score.IncorrectWords.Split(',', score.IncorrectWords).ToList();
                    IEnumerable<string> letters = score.IncorrectLetters.Split(',', score.IncorrectLetters);
                    List<char> incorrectLetters = letters.Select(c => c[0]).ToList();
                    scores.Add(new(score.WordsAmount, TimeOnly.Parse(score.Time), DateOnly.Parse(score.Date), score.Value, incorrectWords, incorrectLetters));
                }
            }
            catch (Exception) {}
            return scores;
        }

        public List<string> GetWords(int limit = -1, int minLength = 0, int maxLength = -1)
        {
            HttpResponseMessage response = client.GetAsync($"words?limit={limit}&minLenght={minLength}&maxLength={maxLength}").Result;
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            List<string> words = [];
            try
            {
                foreach (dynamic word in result!)
                {
                    words.Add(word.Value);
                }
            }
            catch (Exception) {}
            return words;
        }

        public string? Login(string username, string password)
        {
            FormUrlEncodedContent data = new([
                new("username", username),
                new("password", password)
            ]);
            HttpResponseMessage response = client.PostAsync("login", data).Result;
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            try
            {
                return result!.message.authentication;
            }
            catch (Exception) {}
            return null;
        }

        public void Register(string token, string username, string password)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, "register");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            ]);
            client.SendAsync(request).Wait();
        }
    }
}
