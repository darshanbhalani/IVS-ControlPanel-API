using IVS_API.Hubs;
using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using System.Data;
using System.Reflection;
using System.Xml.Linq;

namespace IVS_API.Controllers.Voters
{
    [Route("[controller]")]
    [ApiController]
    public class VoterController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        private readonly IHubContext<ElectionPartyHub> _hubContext;
        public VoterController(NpgsqlConnection connection, IHubContext<ElectionPartyHub> hubContext)
        {
            _connection = connection;
            _connection.Open();
            _hubContext = hubContext;
        }
        [HttpGet("GetVoterByEpic")]
        public IActionResult GetVoterByEpic(string voterid)
        {

            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                VoterModel voter = new VoterModel();
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_VOTER_GETVOTERBYEPIC(@in_voterid)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_voterid", voterid);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            voter.VoterId = reader.GetInt64(reader.GetOrdinal("voterid"));
                            voter.VoterEpic = reader.GetString(reader.GetOrdinal("voterepic"));
                            voter.VoterName = reader.GetString(reader.GetOrdinal("votername"));
                            voter.VoterFatherName = reader.GetString(reader.GetOrdinal("voterfathername"));
                            voter.VoterPhoneNumber = reader.GetInt64(reader.GetOrdinal("voterphonenumber"));
                            voter.VoterBirthDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("voterbirthdate")));
                            voter.VoterGender = reader.GetString(reader.GetOrdinal("votergender"));
                            voter.VoterAddress = reader.GetString(reader.GetOrdinal("voteraddress"));
                            voter.VoterAssemblyId = reader.GetInt32(reader.GetOrdinal("voterassemblyid"));
                            voter.StateAssemblyName = reader.GetString(reader.GetOrdinal("stateassemblyname"));
                            voter.VoterDistrictId = reader.GetInt32(reader.GetOrdinal("voterdistrictid"));
                            voter.DistrictName = reader.GetString(reader.GetOrdinal("districtname"));
                            voter.IssuedOn = reader.GetDateTime(reader.GetOrdinal("issuedon"));
                            voter.ApprovedOn = reader.IsDBNull(reader.GetOrdinal("approvedon")) ? null : reader.GetDateTime(reader.GetOrdinal("approvedon"));


                            return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = voter } });
                        }
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Voter not found with given voter id." } });


                    }
                }

            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch voter details." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch voter details." } });
            }


        }
    }
}
