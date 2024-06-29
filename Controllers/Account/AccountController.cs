using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Data.Common;
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
            _connection.Open();
            _configuration = configuration; 
        }

        [HttpGet("Login")]
        public IActionResult Login(string userName,string password)
        {
            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_ACCOUNT_LOGIN(@in_userName,@in_password)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_userName", userName);
                    cmd.Parameters.AddWithValue("in_password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var token = generateToken(userName, reader.GetString(reader.GetOrdinal("employeename")), reader.GetInt32(reader.GetOrdinal("employeedesignation")));
                             return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Login Successfull.", loginToken=token } });
                        }
                        else
                        {
                            return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Invalid UserCode or Password."} });
                        }
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, error = pex.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, error = ex.Message });
            }
        }


        private string generateToken(string employeeCode,string employeeName,int employeeRoleId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim("employeeCode", employeeCode), 
        new Claim("employeeName", employeeName), 
        new Claim("tokenType", "employee-login"), 
        new Claim("createdOn", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")), 
        new Claim("employeeRoleId", employeeRoleId.ToString())
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["Jwt:TokenValidity"]!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
