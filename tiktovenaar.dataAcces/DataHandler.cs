﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
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
            if (result is not JArray && result is not Leaderboards && (string)result.type != "success")
            {
                throw new RequestFailedException((string)result.message);
            }
        }

        public int AddScore(string token, Score score)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, "score");
            request.Headers.Authorization = new("Bearer", token);
            List<KeyValuePair<string, string>> data = [
                new("wordsAmount", score.WordsAmount.ToString()),
                new("value", score.Value.ToString()),
                new("duration", score.Duration.ToString()),
                new("wpm", score.WPM.ToString().Replace(',', '.'))
            ];
            foreach (string word in score.IncorrectWords)
            {
                data.Add(new("incorrectWords", word));
            }
            foreach (char c in score.IncorrectLetters)
            {
                data.Add(new("incorrectLetters", c.ToString()));
            }
            foreach (string word in score.CorrectWords)
            {
                data.Add(new("correctWords", word));
            }
            request.Content = new FormUrlEncodedContent(data);
            ThrowIfError(client.SendAsync(request).Result);
            return 0;
        }

        public Leaderboards GetLeaderboards(int limit = -1)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync($"leaderboards?limit={limit}").Result;
                dynamic? leaderboards = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                List<PartialScore<int>> scores = [];
                List<PartialScore<double>> wpm = [];
                List<PartialScore<int>> levels = [];
                foreach (dynamic score in leaderboards!.scores)
                {
                    scores.Add(new((string)score.player, (int)score.value));
                }
                foreach (dynamic wpmScore in leaderboards!.wpm)
                {
                    wpm.Add(new((string)wpmScore.player, (double)wpmScore.value));
                }
                foreach (dynamic level in leaderboards!.levels)
                {
                    levels.Add(new((string)level.player, (int)level.value));
                }
                return new(scores, wpm, levels);
            }
            catch (Exception)
            {
                throw new RequestFailedException("Something went wrong.");
            }
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
                Score entry = new()
                {
                    WordsAmount = (int)score.wordsAmount,
                    Value = (int)score.value,
                    IncorrectWords = score.incorrectWords.ToObject<List<string>>(),
                    IncorrectLetters = score.incorrectLetters.ToObject<List<char>>(),
                    CorrectWords = score.correctWords.ToObject<List<string>>(),
                    Duration = TimeSpan.Parse((string)score.duration),
                };
                scores.Add(entry);
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

        public LoginResponse Login(string username, string password)
        {
            FormUrlEncodedContent data = new([
                new("username", username),
                new("password", password)
            ]);
            HttpResponseMessage response = client.PostAsync("login", data).Result;
            ThrowIfError(response);
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            string token = result!.message.authentication;
            token = token["Bearer ".Length..];
            bool admin = result!.message.admin;
            int gainedXP = result!.message.gainedXP;
            int streak = result!.message.streak - 1;
            return new(username, token, admin, gainedXP, streak);
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

        public List<PartialUser> GetUsers(string token)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, "user");
            request.Headers.Authorization = new("Bearer", token);
            HttpResponseMessage response = client.SendAsync(request).Result;
            ThrowIfError(response);
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            List<PartialUser> users = [];
            foreach (dynamic user in result!)
            {
                users.Add(new((int)user.id, (string)user.name));
            }
            return users;
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

        public Definition GetDefinition(string word)
        {
            HttpResponseMessage response = client.GetAsync($"definition?word={word}").Result;
            ThrowIfError(response);
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            return new((string)result!.message.category, (string)result!.message.meaning);
        }

        public LoginResponse Authorize(string token)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, "authorize");
            request.Headers.Authorization = new("Bearer", token);
            HttpResponseMessage response = client.SendAsync(request).Result;
            ThrowIfError(response);
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            string name = result!.message.username;
            bool admin = result!.message.admin;
            int gainedXP = result!.message.gainedXP;
            int streak = result!.message.streak - 1;
            return new(name, token, admin, gainedXP, streak);
        }

        public Level GetLevel(string token)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, "level");
            request.Headers.Authorization = new("Bearer", token);
            HttpResponseMessage response = client.SendAsync(request).Result;
            ThrowIfError(response);
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            int level = result!.message.level;
            int xp = result!.message.xp;
            return new(level, xp);
        }
    }
}