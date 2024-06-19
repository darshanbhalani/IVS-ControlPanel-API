using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace IVS_API.Controllers.General
{
    [Route("[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        public DistrictController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        [HttpGet("GetAllDistricts")]
        public IActionResult GetAllDistricts(int stateid)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<DistrictModel> districts = new List<DistrictModel>();
            try
            {
                using(var cmd=new NpgsqlCommand("SELECT * FROM IVS_DISTRICT_GETALLDISTRICTSOFSTATE(@stateid)", _connection))
                {
                    cmd.Parameters.AddWithValue("stateid", stateid);
                    using(var reader = cmd.ExecuteReader()) { 
                        if (reader.Read())
                        {
                            while (reader.Read())
                            {
                                districts.Add(new DistrictModel
                                {
                                    DistrictId = reader.GetInt32(reader.GetOrdinal("districtid")),
                                    DistrictName = reader.GetString(reader.GetOrdinal("districtname")),
                                });
                            }
                        }
                    }
                }
                return Ok(new { success = true, header = new { requestTime= timeStamp,responsTime = TimeZoneIST.now() },body = new {data=districts } });
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, error = pex.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.Message });
            }
        }
    }
}
