using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace IVS_API.Controllers.General
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        public StateController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        [HttpGet("GetAllStates")]
        public IActionResult GetAllStates()
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<StateModel> states = new List<StateModel>();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_STATE_GETALLSTATES()", _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            while (reader.Read())
                            {
                                states.Add(new StateModel
                                {
                                    StateId = reader.GetInt32(reader.GetOrdinal("stateid")),
                                    StateName = reader.GetString(reader.GetOrdinal("statename")),
                                    StateAbbreiviation = reader.GetString(reader.GetOrdinal("stateabbreviation")),
                                });
                            }
                        }
                    }
                }
                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = states } });
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
