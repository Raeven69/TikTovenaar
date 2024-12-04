using Newtonsoft.Json;
using TikTovenaar.Logic;

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
            List<KeyValuePair<string, string>> data = [
                new("wordsAmount", score.WordsAmount.ToString()),
                new("time", score.Time.ToLongTimeString()),
                new("date", score.Date.ToLongDateString()),
                new("value", score.Value.ToString()),
            ];
            foreach (string word in score.IncorrectWords)
            {
                data.Add(new("incorrectWords", word));
            }
            foreach (char c in score.IncorrectLetters)
            {
                data.Add(new("incorrectLetters", c.ToString()));
            }
            request.Content = new FormUrlEncodedContent(data);
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
                    scores.Add(new((string)score.player, (int)score.value));
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
                    scores.Add(new((int)score.wordsAmount, TimeOnly.Parse((string)score.time), DateOnly.Parse((string)score.date), (int)score.value, score.incorrectWords.ToObject<List<string>>(), score.incorrectLetters.ToObject<List<char>>()));
                }
            }
            catch (Exception) {}
            return scores;
        }

        public List<string> GetWords(int limit = -1, int minLength = 0, int maxLength = -1)
        {
            HttpResponseMessage response = client.GetAsync($"words?limit={limit}&minLength={minLength}&maxLength={maxLength}").Result;
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            List<string> words = [];
            try
            {
                foreach (dynamic word in result!)
                {
                    words.Add((string)word.value);
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
                string token = result!.message.authentication;
                return token["Bearer ".Length..];
            }
            catch (Exception) {}
            return null;
        }

        public void Register(string token, string username, string password)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, "register");
            request.Headers.Authorization = new("Bearer", token);
            request.Content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            ]);
            client.SendAsync(request).Wait();
        }
    }
}
