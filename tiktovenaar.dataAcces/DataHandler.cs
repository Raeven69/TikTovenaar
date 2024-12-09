using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
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

        private static void ThrowIfError(HttpResponseMessage response)
        {
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result) ?? throw new RequestFailedException("Something went wrong.");
            Console.WriteLine(result);
            if (result is not JArray && (string)result.type != "success")
            {
                throw new RequestFailedException((string)result.message);
            }
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
            ThrowIfError(client.SendAsync(request).Result);
        }

        public List<PartialScore> GetHighscores(int limit = -1)
        {
            HttpResponseMessage response = client.GetAsync($"highscores?limit={limit}").Result;
            ThrowIfError(response);
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            List<PartialScore> scores = [];
            foreach (dynamic score in result!)
            {
                scores.Add(new((string)score.player, (int)score.value));
            }
            return scores;
        }

        public List<Score> GetScores(string token)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, "score");
            request.Headers.Authorization = new("Bearer", token);
            HttpResponseMessage response = client.SendAsync(request).Result;
            ThrowIfError(response);
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            List<Score> scores = [];
            foreach (dynamic score in result!)
            {
                scores.Add(new((int)score.wordsAmount, TimeOnly.Parse((string)score.time), DateOnly.Parse((string)score.date), (int)score.value, score.incorrectWords.ToObject<List<string>>(), score.incorrectLetters.ToObject<List<char>>()));
            }
            return scores;
        }

        public List<string> GetWords(int limit = -1, int minLength = 0, int maxLength = -1)
        {
            HttpResponseMessage response = client.GetAsync($"words?limit={limit}&minLength={minLength}&maxLength={maxLength}").Result;
            ThrowIfError(response);
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            List<string> words = [];
            foreach (dynamic word in result!)
            {
                words.Add((string)word.value);
            }
            return words;
        }

        public string Login(string username, string password, out bool admin)
        {
            FormUrlEncodedContent data = new([
                new("username", username),
                new("password", password)
            ]);
            HttpResponseMessage response = client.PostAsync("login", data).Result;
            ThrowIfError(response);
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            string token = result!.message.authentication;
            admin = result!.message.admin;
            return token["Bearer ".Length..];
        }

        public void Register(string token, string username, string password)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, "user");
            request.Headers.Authorization = new("Bearer", token);
            request.Content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            ]);
            ThrowIfError(client.SendAsync(request).Result);
        }

        public void DeleteUser(string token, string username)
        {
            using HttpRequestMessage request = new(HttpMethod.Delete, "user");
            request.Headers.Authorization = new("Bearer", token);
            request.Content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("username", username)
            ]);
            ThrowIfError(client.SendAsync(request).Result);
        }
    }
}
