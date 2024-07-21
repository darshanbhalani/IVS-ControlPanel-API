using IVS_API.Hubs;
using IVS_API.Models.StateElection;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using NpgsqlTypes;

namespace IVS_API.Controllers.StateElections
{
    [Route("[controller]")]
    [ApiController]
    public class ElectionController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        private readonly IHubContext<ElectionPartyHub> _hubContext;
        public ElectionController(NpgsqlConnection connection, IHubContext<ElectionPartyHub> hubContext)
        {
            _connection = connection;
            _connection.Open();
            _hubContext = hubContext;
        }

        [HttpGet("GetAllStateElections")]
        public IActionResult GetAllStateElections()
        {
            DateTime timeStamp = TimeZoneIST.now();

            List<StateElectionModel> elections = new List<StateElectionModel>();
            try
            {
                using (var command = new NpgsqlCommand("SELECT * FROM PUBLIC.IVS_STATEELECTION_GETALLELECTIONS()", _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            elections.Add(new StateElectionModel
                            {
                                StateElectionId = reader.GetInt64(reader.GetOrdinal("stateelectionid")),
                                ElectionStageName = reader.GetString(reader.GetOrdinal("electionstagename")),
                                ElectionStageId = reader.GetInt32(reader.GetOrdinal("electionstageid")),
                                StateName = reader.GetString(reader.GetOrdinal("statename")),
                                StateId = reader.GetInt32(reader.GetOrdinal("stateid")),
                                ElectionDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("electiondate"))),
                                VerificationStatus = reader.GetInt32(reader.GetOrdinal("verificationstatus")),
                                VerificationStatusName = reader.GetString(reader.GetOrdinal("verificationstatusname")),
                            });
                        }
                        return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = elections } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch elections. Some thing went wrong." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch elections. Some thing went wrong." } });
            }
        }

        [HttpGet("GetAllLiveElections")]
        public IActionResult GetAllLiveElections()
        {
            DateTime timeStamp = TimeZoneIST.now();

            List<StateElectionModel> elections = new List<StateElectionModel>();
            try
            {
                using (var command = new NpgsqlCommand("SELECT * FROM PUBLIC.IVS_STATEELECTION_GETLIVEELECTIONS()", _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            elections.Add(new StateElectionModel
                            {
                                StateElectionId = reader.GetInt64(reader.GetOrdinal("stateelectionid")),
                                ElectionStageName = reader.GetString(reader.GetOrdinal("electionstagename")),
                                ElectionStageId = reader.GetInt32(reader.GetOrdinal("electionstageid")),
                                StateName = reader.GetString(reader.GetOrdinal("statename")),
                                StateId = reader.GetInt32(reader.GetOrdinal("stateid")),
                                ElectionDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("electiondate"))),
                                VerificationStatus = reader.GetInt32(reader.GetOrdinal("verificationstatus"))
                            });
                        }
                        return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = elections } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch elections. Some thing went wrong." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch elections. Some thing went wrong." } });
            }
        }

        [HttpGet("GetUpcommingElections")]
        public IActionResult GetAllUpcommingElections()
        {
            DateTime timeStamp = TimeZoneIST.now();

            List<StateElectionModel> elections = new List<StateElectionModel>();
            try
            {
                using (var command = new NpgsqlCommand("SELECT * FROM PUBLIC.IVS_STATEELECTION_GETUPCOMMINGELECTIONS()", _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            elections.Add(new StateElectionModel
                            {
                                StateElectionId = reader.GetInt64(reader.GetOrdinal("stateelectionid")),
                                ElectionStageName = reader.GetString(reader.GetOrdinal("electionstagename")),
                                ElectionStageId = reader.GetInt32(reader.GetOrdinal("electionstageid")),
                                StateName = reader.GetString(reader.GetOrdinal("statename")),
                                StateId = reader.GetInt32(reader.GetOrdinal("stateid")),
                                ElectionDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("electiondate"))),
                                VerificationStatus = reader.GetInt32(reader.GetOrdinal("verificationstatus"))
                            });
                        }
                        return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = elections } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch elections. Some thing went wrong." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch elections. Some thing went wrong." } });
            }
        }

        [HttpGet("GetCompletedElections")]
        public IActionResult GetAllCompletedElections()
        {
            DateTime timeStamp = TimeZoneIST.now();

            List<StateElectionModel> elections = new List<StateElectionModel>();
            try
            {
                using (var command = new NpgsqlCommand("SELECT * FROM PUBLIC.IVS_STATEELECTION_GETCOMPLETEDELECTIONS()", _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            elections.Add(new StateElectionModel
                            {
                                StateElectionId = reader.GetInt64(reader.GetOrdinal("stateelectionid")),
                                ElectionStageName = reader.GetString(reader.GetOrdinal("electionstagename")),
                                ElectionStageId = reader.GetInt32(reader.GetOrdinal("electionstageid")),
                                StateName = reader.GetString(reader.GetOrdinal("statename")),
                                StateId = reader.GetInt32(reader.GetOrdinal("stateid")),
                                ElectionDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("electiondate"))),
                                VerificationStatus = reader.GetInt32(reader.GetOrdinal("verificationstatus"))
                            });
                        }
                        return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = elections } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch elections. Some thing went wrong." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch elections. Some thing went wrong." } });
            }
        }

        [HttpPost("SheduleStateElection")]
        public async Task<IActionResult> SheduleStateElection(IN_StateElectionModel data)
        {
            data.ElectionDate = data.ElectionDate.Split('T')[0];
            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                using (var cmd = new NpgsqlCommand($"SELECT * FROM IVS_STATEELECTION_SHEDULEELECTION(@in_stateId,@in_date,@in_by)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_stateid", data.StateId!);
                    cmd.Parameters.Add(new NpgsqlParameter("in_date", NpgsqlDbType.Date) { Value = DateOnly.Parse(data.ElectionDate) });
                    cmd.Parameters.AddWithValue("in_by", data.ActionBy);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                await broadcastStateElections();
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Election successfully sheduled." } });
                            }
                        }
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to shedule election because state has alredy uncomplete election." } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to shedule election. Somthing went wrong." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to shedule election. Something went wrong." } });
            }
        }

        [HttpGet("DeleteStateElection")]
        public async Task<IActionResult> DeleteStateElection(long electionId, long actionBy)
        {
            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                using (var cmd = new NpgsqlCommand($"SELECT * FROM IVS_STATEELECTION_DELETEELECTION(@in_electionId,@in_actionBy)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_electionId", electionId);
                    cmd.Parameters.AddWithValue("in_actionBy", actionBy);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                await broadcastStateElections();
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Election successfully deleted." } });
                            }
                        }
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to delete election." } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to delete election. Somthing went wrong." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to delete election. Something went wrong." } });
            }
        }

        [HttpPost("UpdateStateElection")]
        public async Task<IActionResult> UpdateStateElection(IN_StateElectionModel data)
        {
            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                using (var cmd = new NpgsqlCommand($"SELECT * FROM IVS_STATEELECTION_UPDATEELECTION(@in_electionId,@in_date,@in_actionBy);", _connection))
                {
                    cmd.Parameters.AddWithValue("in_electionId", data.StateElectionId!);
                    cmd.Parameters.Add(new NpgsqlParameter("in_date", NpgsqlDbType.Date) { Value = DateOnly.Parse(data.ElectionDate) });
                    cmd.Parameters.AddWithValue("in_actionBy", data.ActionBy);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                await broadcastStateElections();
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Election successfully reshedule." } });
                            }
                        }
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to reshedule election." } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new
                {
                    success = false,
                    header = new
                    {
                        requestTime = timeStamp,
                        responsTime = TimeZoneIST.now()
                    },
                    body = new { error = "Unable to reshedule election. Somthing went wrong." }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to reshedule election. Something went wrong." } });
            }
        }

        [HttpGet("VerifyStateElection")]
        public async Task<IActionResult> VerifyStateElection(long electionId, long actionBy)
        {
            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                using (var cmd = new NpgsqlCommand($"SELECT * FROM IVS_STATEELECTION_VERIFYELECTION(@in_electionId,@in_verifiedBy)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_electionId", electionId);
                    cmd.Parameters.AddWithValue("in_verifiedBy", actionBy);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                await broadcastStateElections();
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Election successfully verified." } });
                            }
                        }
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to verify election." } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new
                {
                    success = false,
                    header = new
                    {
                        requestTime = timeStamp,
                        responsTime = TimeZoneIST.now()
                    },
                    body = new { error = "Unable to verify election. Somthing went wrong." }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to verify election. Something went wrong." } });
            }
        }

        [HttpGet("LockStateElection")]
        public async Task<IActionResult> LockStateElection(long electionId, long actionBy)
        {
            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                using (var cmd = new NpgsqlCommand($"SELECT * FROM IVS_STATEELECTION_LOCKELECTION(@in_electionId,@in_actionBy)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_electionId", electionId);
                    cmd.Parameters.AddWithValue("in_actionBy", actionBy);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                await broadcastStateElections();
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Election successfully locked." } });
                            }
                        }
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to lock election : " + reader.GetString(1) } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new
                {
                    success = false,
                    header = new
                    {
                        requestTime = timeStamp,
                        responsTime = TimeZoneIST.now()
                    },
                    body = new { error = "Unable to lock election. Somthing went wrong." }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to lock election. Something went wrong." } });
            }
        }

        private async Task broadcastStateElections()
        {
            try
            {
                var data = GetAllStateElections();
                await _hubContext.Clients.All.SendAsync("Broadcast-StateElections", data);
            }
            catch (NpgsqlException pex)
            {
            }
            catch (Exception ex)
            {

            }
        }
    }
}