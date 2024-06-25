using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IVS_API.Controllers.Account
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        private readonly IConfiguration _configuration;
        public AccountController(NpgsqlConnection connection, IConfiguration configuration)
        {
            _connection = connection;
            _configuration = configuration;
            _connection.Open();
        }

        [HttpPost("Login")]
        public IActionResult Login(EmployeeModel data)
        {
            DateTime timeStamp = TimeZoneIST.now();
            using (var cmd = new NpgsqlCommand($"select * from ivs_account_login(@in_username,@in_password);", _connection))
            {
                cmd.Parameters.AddWithValue("in_username", data.EmployeeCode);
                cmd.Parameters.AddWithValue("in_password", data.Password);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string token = generateToken(data.EmployeeCode, reader.GetString(reader.GetOrdinal("employeename")), reader.GetInt32(reader.GetOrdinal("employeedesignation")));
                        return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { token = token } });
                    }
                    else
                    {
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Invalid EmployeeCode or Password." } });
                    }
                }
            }
        }

        private string generateToken(string employeeCode, string employeeName, int employeeRole)
        {
            var claims = new[]
            {
                new Claim("employeeCode", employeeCode), 
                new Claim("employeeName", employeeName), 
                new Claim("tokenType", "employee-login"), 
                new Claim("createdOn", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")), 
                new Claim("roleId", employeeRole.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(30),
                 claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
