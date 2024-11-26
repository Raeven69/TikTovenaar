using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Npgsql;
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

        public static bool IsAuthorized(HttpRequest request, bool admin = false)
        {
            if (!request.Headers.TryGetValue("Authorization", out StringValues value))
            {
                return false;
            }
            string? token = value;
            if (token == null)
            {
                return false;
            }
            token = token["Bearer ".Length..];
            try
            {
                dynamic? payload = JsonConvert.DeserializeObject(decoder.Decode(token, false));
                if (payload == null)
                {
                    return false;
                }
                NpgsqlConnection connection = new Database().GetConnection();
                connection.Open();
                using NpgsqlCommand cmd = new(@"SELECT * FROM players WHERE id = @id", connection);
                cmd.Parameters.AddWithValue("id", (int)payload.sub);
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    connection.Close();
                    return false;
                }
                if (admin && reader.GetString(3) != "admin")
                {
                    connection.Close();
                    return false;
                }
                connection.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
