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
            throw new NotImplementedException();
        }

        public List<PartialScore> GetHighscores()
        {
            throw new NotImplementedException();
        }

        public List<Score> GetScores(string token)
        {
            throw new NotImplementedException();
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
                return words;
            }
            catch (Exception) {}
            return words;
        }

        public string? Login(string username, string password)
        {
            FormUrlEncodedContent data = new([
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
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
