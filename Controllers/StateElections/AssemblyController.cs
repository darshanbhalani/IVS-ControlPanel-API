using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data.Common;

namespace IVS_API.Controllers.StateElections
{
    [Route("[controller]")]
    [ApiController]
    public class AssemblyController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        public AssemblyController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }
        [HttpGet("GetAllAssemblyByState")]
        public IActionResult GetAllAssemblyByState(int stateid)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<AsseblyModel> asseblies = new List<AsseblyModel>();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM ivs_stateassemblies_getallstateassemblies(@stateid)", _connection))
                {
                    cmd.Parameters.AddWithValue("stateid", stateid);
                    using (var reader = cmd.ExecuteReader())
                    {
                            while (reader.Read())
                            {
                                asseblies.Add(new AsseblyModel
                                {
                                    AsseblyId = reader.GetInt32(reader.GetOrdinal("stateassemblyid")),
                                    AsseblyName = reader.GetString(reader.GetOrdinal("stateassemblyname")),
                                    AsseblyDistrict = reader.GetString(reader.GetOrdinal("districtname"))
                                });
                            }
                    }
                }
                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = asseblies } });
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

        [HttpGet("GetAllAssemblyByDistrict")]
        public IActionResult GetAllAssemblyByDistrict(int districtid)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<AsseblyModel> asseblies = new List<AsseblyModel>();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM ivs_stateassemblies_getallstateassembliesbydistrict(@districtid)", _connection))
                {
                    cmd.Parameters.AddWithValue("districtid", districtid);
                    using (var reader = cmd.ExecuteReader())
                    {
                            while (reader.Read())
                            {
                                asseblies.Add(new AsseblyModel
                                {
                                    AsseblyId = reader.GetInt32(reader.GetOrdinal("stateassemblyid")),
                                    AsseblyName = reader.GetString(reader.GetOrdinal("stateassemblyname")),
                                });
                            }
                    }
                }
                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = asseblies } });
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
