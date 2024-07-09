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
        public IActionResult SheduleStateElection(ElectionModel data, long createby)
        {
            DateTime timeStamp = TimeZoneIST.now();
            using (var cmd = new NpgsqlCommand($"ivs_stateelection_sheduleelection()", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.GetBoolean(0))
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
<<<<<<< HEAD
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch elections. Some thing went wrong." } });
=======
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch elections. Some thing went wrong." } });
>>>>>>> 4ce770d29ddbaacf0919cfd75c7017b197c1f930
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
    }
}