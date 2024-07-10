using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Npgsql;
using NpgsqlTypes;
using System.Data.Common;
using System.IO;
using System.Reflection;

namespace IVS_API.Controllers.StateElections
{
    [Route("[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        public CandidateController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        [HttpPost("AddCandidate")]
        public async Task<IActionResult> AddCandidate([FromForm] StateElectionCandidateModel data, IFormFile? image)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<ElectionPartyModel> parties = new List<ElectionPartyModel>();
            try
            {
                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream); 
                        data.ProfileUrl = memoryStream.ToArray();
                    }
                }
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_STATEELECTIONCANDIDATES_ADDCANDIDATE(@in_candidateepic,@in_candidatename,@in_candidateprofileurl,@in_candidategender,@in_candidatepartyid,@in_candidateassemblyid,@in_candidateelectionid,@in_createdby)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_candidateepic", data.Epic);
                    cmd.Parameters.AddWithValue("in_candidatename", data.Name);
                    cmd.Parameters.AddWithValue("in_candidateprofileurl", data.ProfileUrl != null ? (object)data.ProfileUrl : DBNull.Value);
                    cmd.Parameters.AddWithValue("in_candidategender", data.Gender);
                    cmd.Parameters.AddWithValue("in_candidatepartyid", data.PartyId != null ? (object)data.PartyId : DBNull.Value);
                    cmd.Parameters.AddWithValue("in_candidateassemblyid", data.AssemblyId);
                    cmd.Parameters.AddWithValue("in_candidateelectionid", data.ElectionId);
                    cmd.Parameters.AddWithValue("in_createdby", 1);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Candidate successfully added." } });
                            }
                            else
                            {
                                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Candidate. Some thing went wrong." } });
                            }
                        }
                        else
                        {
                            return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Candidate. Some thing went wrong." } });
                        }
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                if (pex.SqlState == "23505")
                {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Candidate is already exist in currect election." } });
                }
                else
                {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Candidate. Some thing went wrong." } });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Party. Some thing went wrong." } });
            }
        }

        [HttpGet("GetAllCandidates")]
        public IActionResult GetAllCandidates(long electionid)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<StateElectionCandidateModel> candidates = new List<StateElectionCandidateModel>();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_STATEELECTIONCANDIDATES_GETALLCANDIDATE(@in_electionid)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_electionid", electionid);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            candidates.Add(
                                 new StateElectionCandidateModel
                                 {
                                     Id = reader.GetInt64(reader.GetOrdinal("stateelectioncandidateid")),
                                     ProfileUrl = reader["stateelectioncandidateprofileurl"] as byte[],
                                     Name = reader.GetString(reader.GetOrdinal("stateelectioncandidatename")),
                                     Gender = reader.GetString(reader.GetOrdinal("stateelectioncandidategender")),
                                     PartyId = reader.IsDBNull(reader.GetOrdinal("stateelectioncandidatepartyid")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("stateelectioncandidatepartyid")),
                                     PartyName = reader.GetString(reader.GetOrdinal("electionpartyname")),
                                     Epic = reader.GetString(reader.GetOrdinal("stateelectioncandidateepic")),
                                     AssemblyId = reader.GetInt32(reader.GetOrdinal("stateelectioncandidateassemblyid")),
                                     AssemblyName = reader.GetString(reader.GetOrdinal("stateassemblyname")),
                                     verificationStatus = reader.GetString(reader.GetOrdinal("verificationstatusname"))
                                 }
                                );
                        }
                    }
                }
                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = candidates } });

            }
            catch (NpgsqlException pex)
            {
                if (pex.SqlState == "23505")
                {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Candidate is already exist in currect election." } });
                }
                else
                {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Candidate. Some thing went wrong." } });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Party. Some thing went wrong." } });
            }
        }

        [HttpGet("GetAllCandidatesOfAssembly")]
        public IActionResult GetAllCandidatesOfAssembly(long electionid,int assemblyId)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<StateElectionCandidateModel> candidates = new List<StateElectionCandidateModel>();
            try
            {

                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_STATEELECTIONCANDIDATES_GETALLCANDIDATESBYASSEMBLY(@in_electionid, @in_assemblyid)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_electionid", electionid);
                    cmd.Parameters.AddWithValue("in_assemblyid", assemblyId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            candidates.Add(
                                 new StateElectionCandidateModel
                                 {
                                     Id = reader.GetInt64(reader.GetOrdinal("stateelectioncandidateid")),
                                     ProfileUrl = reader["stateelectioncandidateprofileurl"] as byte[],
                                     Name = reader.GetString(reader.GetOrdinal("stateelectioncandidatename")),
                                     Gender = reader.GetString(reader.GetOrdinal("stateelectioncandidategender")),
                                     PartyId = reader.IsDBNull(reader.GetOrdinal("stateelectioncandidatepartyid")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("stateelectioncandidatepartyid")),
                                     PartyName = reader.GetString(reader.GetOrdinal("electionpartyname")),
                                     Epic = reader.GetString(reader.GetOrdinal("stateelectioncandidateepic")),
                                     AssemblyId = reader.GetInt32(reader.GetOrdinal("stateelectioncandidateassemblyid")),
                                     AssemblyName = reader.GetString(reader.GetOrdinal("stateassemblyname")),
                                     verificationStatus = reader.GetString(reader.GetOrdinal("verificationstatusname"))
                                 }
                                );
                        }
                    }
                }
                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data=candidates} });
                        
            }
            catch (NpgsqlException pex)
            {
                if (pex.SqlState == "23505")
                {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Candidate is already exist in currect election." } });
                }
                else
                {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Candidate. Some thing went wrong." } });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Party. Some thing went wrong." } });
            }
        }

        [HttpGet("VerifyCandidate")]
        public IActionResult VerifyCandidate(long candidateId, long verifiedBy)
        {
            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_STATEELECTIONCANDIDATES_VERIFYCANDIDATE(@in_candidateid, @in_verifiedby)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_candidateid", candidateId);
                    cmd.Parameters.AddWithValue("in_verifiedby", verifiedBy);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if(reader.Read()) { 
                            if(reader.GetBoolean(0)) {
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Candidate successfully verified." } });
                            }
                            else
                            {
                                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to verify candidate. Some thing went wrong." } });
                            }
                        }
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to verify candidate. Some thing went wrong." } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to verify candidate. Some thing went wrong." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to verify candidate. Some thing went wrong." } });
            }
        }

        [HttpGet("DeleteCandidate")]
        public IActionResult DeleteCandidate(long candidateId, long deletedBy)
        {
            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_STATEELECTIONCANDIDATES_DELETECANDIDATE(@in_candidateid, @in_deletedby)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_candidateid", candidateId);
                    cmd.Parameters.AddWithValue("in_deletedby", deletedBy);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Candidate successfully deleted." } });
                            }
                            else
                            {
                                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to deleted candidate. Some thing went wrong." } });
                            }
                        }
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to deleted candidate. Some thing went wrong." } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to deleted candidate. Some thing went wrong." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to deleted candidate. Some thing went wrong." } });
            }
        }

        [HttpGet("UpdateCandidate")]
        public IActionResult UpdateCandidate(long candidateId, long deletedBy)
        {
            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_STATEELECTIONCANDIDATES_DELETECANDIDATE(@in_candidateid, @in_deletedby)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_candidateid", candidateId);
                    cmd.Parameters.AddWithValue("in_verifiedby", deletedBy);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Candidate successfully deleted." } });
                            }
                            else
                            {
                                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to update candidate. Some thing went wrong." } });
                            }
                        }
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to update candidate. Some thing went wrong." } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to update candidate. Some thing went wrong." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to update candidate. Some thing went wrong." } });
            }
        }
    }
}
