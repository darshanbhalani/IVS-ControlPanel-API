using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Http;
<<<<<<< HEAD
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
=======
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Data.Common;
>>>>>>> f741d7aa5814a808f279266025a534c80ef95481
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
<<<<<<< HEAD
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
=======
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
                            var token = generateToken(reader.GetInt64(reader.GetOrdinal("employeeid")), reader.GetString(reader.GetOrdinal("employeename")), reader.GetInt32(reader.GetOrdinal("employeedesignation")));
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

        [HttpGet("GetUserProfileDetails")]
        public IActionResult GetUserProfileDetails(long userId)
        {
            DateTime timeStamp = TimeZoneIST.now();
            UserModel user = new UserModel();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_ACCOUNT_GETUSERPROFILE(@in_userId)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user.Code = reader.GetString(reader.GetOrdinal("employeecode"));
                            user.ProfileUrl = reader.GetString(reader.GetOrdinal("employeeprofileurl"));
                            user.UserName  = reader.GetString(reader.GetOrdinal("employeename"));
                            user.Email = reader.GetString(reader.GetOrdinal("employeeemail"));
                            user.PhoneNumber = reader.GetInt64(reader.GetOrdinal("employeephonenumber"));
                            user.Gender = reader.GetString(reader.GetOrdinal("employeegender"));
                            user.Role = reader.GetString(reader.GetOrdinal("employeedesignationname"));

                            return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "User profile data successfully fetched.", data = user } });
                        }
                        else
                        {
                            return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch user profile data." } });
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

        private string generateToken(long employeeId,string employeeName,int employeeRoleId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim("employeeId", employeeId.ToString()), 
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
>>>>>>> f741d7aa5814a808f279266025a534c80ef95481
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
