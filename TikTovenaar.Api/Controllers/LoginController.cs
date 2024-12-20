﻿using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TikTovenaar.Api.Controllers
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public JsonResult Login([FromForm] User user)
        {
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT * FROM players WHERE name = @name", connection);
            cmd.Parameters.AddWithValue("name", user.Username);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string password = reader.GetString(2);
                try
                {
                    if (!BCrypt.Net.BCrypt.Verify(user.Password, password))
                    {
                        connection.Close();
                        return new(new { type = "error", message = "Username or password incorrect." });
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    return new(new { type = "error", message = "Username or password incorrect." });
                }
                int userID = reader.GetInt32(0);
                string token = Utils.CreateToken(userID);
                bool admin = reader.GetString(3) == "admin";
                connection.Close();
                int gainedXP = Utils.LoginBonus(userID, out int streak);
                return new(new { type = "success", message = new { authentication = $"Bearer {token}", admin, gainedXP, streak } });
            }
            connection.Close();
            return new(new { type = "error", message = "Username or password incorrect." });
        }
    }
}
