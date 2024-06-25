using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace IVS_API.Controllers.StateElections
{
    [Route("[controller]")]
    [ApiController]
    public class ElectionController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        public ElectionController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        [HttpGet("SheduleStateElection")]
        public IActionResult SheduleStateElection(ElectionModel data,long createby)
        {
            DateTime timeStamp = TimeZoneIST.now();
            using (var cmd=new NpgsqlCommand($"ivs_stateelection_sheduleelection()",_connection))
            {
                using(var reader = cmd.ExecuteReader())
                {
                    if(reader.GetBoolean(0))
                    {
                        return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Election successfully sheduled." } });
                    }
                    else
                    {
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to shedule election." } });
                    }
                }
            }
        }

        
    }
}
