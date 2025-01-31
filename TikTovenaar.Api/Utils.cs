﻿using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Npgsql;
using System.IO;
using System.Security.Cryptography;

namespace TikTovenaar.Api
{
    public class Utils
    {
        private static readonly JsonNetSerializer serializer = new();
        private static readonly UtcDateTimeProvider provider = new();
        private static readonly JwtValidator validator = new(serializer, provider);
        private static readonly JwtBase64UrlEncoder encoder = new();
        private static readonly JwtDecoder decoder = new(serializer, validator, encoder, GetAlgorithm());

        private static RS256Algorithm GetAlgorithm()
        {
            RSA publicKey = RSA.Create();
            publicKey.ImportFromPem(File.ReadAllText("public.key"));
            RSA privateKey = RSA.Create();
            privateKey.ImportFromPem(File.ReadAllText("private.key"));
            return new RS256Algorithm(publicKey, privateKey);
        }

        public static string CreateToken(int userid)
        {
            return JwtBuilder.Create().WithAlgorithm(GetAlgorithm()).AddClaim("sub", userid).AddClaim("exp", DateTimeOffset.Now.AddDays(14).ToUnixTimeSeconds()).Encode();
        }

        public static int? Authorize(HttpRequest request, bool admin = false)
        {
            if (!request.Headers.TryGetValue("Authorization", out StringValues value))
            {
                return null;
            }
            string? token = value;
            if (token == null)
            {
                return null;
            }
            token = token["Bearer ".Length..];
            try
            {
                dynamic? payload = JsonConvert.DeserializeObject(decoder.Decode(token, false));
                if (payload == null)
                {
                    return null;
                }
                NpgsqlConnection connection = new Database().GetConnection();
                connection.Open();
                using NpgsqlCommand cmd = new(@"SELECT * FROM players WHERE id = @id", connection);
                cmd.Parameters.AddWithValue("id", (int)payload.sub);
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    connection.Close();
                    return null;
                }
                if (admin && reader.GetString(3) != "admin")
                {
                    connection.Close();
                    return null;
                }
                connection.Close();
                return (int)payload.sub;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int LoginBonus(int userID, out int streak)
        {
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT * FROM players WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("id", userID);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                streak = 0;
                return 0;
            }
            int level = reader.GetInt32(4);
            DateTime lastLogin = reader.GetDateTime(5);
            int loginStreak = reader.GetInt32(6);
            int xp = reader.GetInt32(7);
            reader.Close();
            int gainedXP = 0;
            if ((DateTime.Now - lastLogin).TotalDays >= 2)
            {
                loginStreak = 1;
            }
            else if ((DateTime.Now - lastLogin).TotalDays >= 1)
            {
                gainedXP = loginStreak * 10000;
                loginStreak++;
                xp += gainedXP;
                int gainedLevel = xp / 100000;
                level += gainedLevel;
                xp %= 100000;
            }
            using NpgsqlCommand update = new(@"UPDATE players SET level = @level, lastlogin = @currentDate, loginstreak = @loginstreak, xp = @xp WHERE id = @id", connection);
            update.Parameters.AddWithValue("level", level);
            update.Parameters.AddWithValue("currentDate", DateTime.Now);
            update.Parameters.AddWithValue("loginstreak", loginStreak);
            update.Parameters.AddWithValue("xp", xp);
            update.Parameters.AddWithValue("id", userID);
            update.ExecuteNonQuery();
            connection.Close();
            streak = loginStreak;
            return gainedXP;
        }

        public static void GainXP(int userID, int gainedXP)
        {
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT * FROM players WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("id", userID);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int level = reader.GetInt32(4);
                int xp = reader.GetInt32(7);
                reader.Close();
                xp += gainedXP;
                level += xp / 100000;
                xp %= 100000;
                using NpgsqlCommand update = new(@"UPDATE players SET level = @level, xp = @xp WHERE id = @id", connection);
                update.Parameters.AddWithValue("level", level);
                update.Parameters.AddWithValue("xp", xp);
                update.Parameters.AddWithValue("id", userID);
                update.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
