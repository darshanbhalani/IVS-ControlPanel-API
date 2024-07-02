using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace IVS_API.Controllers.Parties
{
    [Route("[controller]")]
    [ApiController]
    public class ElectionPartyController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        public ElectionPartyController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        [HttpGet("GetAllParties")]
        public IActionResult GetAllParties()
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<ElectionPartyModel> parties = new List<ElectionPartyModel>();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_PARTY_DISPLAYALLPARTIES()", _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                            while (reader.Read())
                            {
                                parties.Add(new ElectionPartyModel
                                {
                                    ElectionPartyId = reader.GetInt64(reader.GetOrdinal("electionpartyid")),
                                    ElectionPartyLogoUrl = reader["electionpartyprofileurl"] as byte[],
                                    ElectionPartyName = reader.GetString(reader.GetOrdinal("electionpartyname")),
                                    VerificationStatus = reader.GetString(reader.GetOrdinal("verificationstatusname")),
                                });
                        }
                    }
                }
                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = parties } });
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

        [HttpGet("GetAllVerifiedParties")]
        public IActionResult GetAllVerifiedParties()
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<ElectionPartyModel> parties = new List<ElectionPartyModel>();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_PARTY_DISPLAYALLVERIFIEDPARTIES()", _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            parties.Add(new ElectionPartyModel
                            {
                                ElectionPartyId = reader.GetInt64(reader.GetOrdinal("electionpartyid")),
                                ElectionPartyLogoUrl = reader["electionpartyprofileurl"] as byte[],
                                ElectionPartyName = reader.GetString(reader.GetOrdinal("electionpartyname")),
                                VerificationStatus = reader.GetString(reader.GetOrdinal("verificationstatusname")),
                            });
                        }
                    }
                }
                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = parties } });
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

        [HttpPost("AddNewParty")]
        public async Task<IActionResult> AddNewParty([FromForm] ElectionPartyModel party,IFormFile image)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<ElectionPartyModel> parties = new List<ElectionPartyModel>();
            try
            {
                if(image != null && image.Length >0) { 
                    using(var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        party.ElectionPartyLogoUrl= memoryStream.ToArray();
                    }
                }
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_PARTY_ADDNEWPARTY(@partylogourl,@partyname,@createdby)", _connection))
                {
                    cmd.Parameters.AddWithValue("partylogourl", NpgsqlTypes.NpgsqlDbType.Bytea, party.ElectionPartyLogoUrl);
                    cmd.Parameters.AddWithValue("partyname", party.ElectionPartyName);
                    cmd.Parameters.AddWithValue("createdby", 1);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Party successfully added." } });
                            }
                            else
                            {
                                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Party. Some thing went wrong." } });
                            }
                        }
                        else
                        {
                            return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Party. Some thing went wrong." } });
                        }
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                if (pex.SqlState == "23505")
                {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Party is already exist with same name." } });
                }
                else
                {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Party. Some thing went wrong." } });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to add Party. Some thing went wrong." } });
            }
        }
       
        [HttpGet("VefifyParty")]
        public IActionResult VefifyParty(long partyid,long verifiedby)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<ElectionPartyModel> parties = new List<ElectionPartyModel>();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_PARTY_VERIFYPARTY(@partid,@verifiedby)", _connection))
                {
                    cmd.Parameters.AddWithValue("partid", partyid);
                    cmd.Parameters.AddWithValue("verifiedby", verifiedby);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Party successfully verified." } });

                            }
                            else
                            {
                                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to verify Party." } });
                            }

                        }
                        else
                        {
                            return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to verify Party." } });
                        }
                    }
                }
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

        [HttpGet("DeleteParty")]
        public IActionResult DeleteParty(long partyid, long deletedby)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<ElectionPartyModel> parties = new List<ElectionPartyModel>();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_PARTY_DELETEPARTY(@partid,@verifiedby)", _connection))
                {
                    cmd.Parameters.AddWithValue("partid", partyid);
                    cmd.Parameters.AddWithValue("verifiedby", deletedby);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Party successfully deleted." } });
                            }
                            else
                            {
                                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to delete Party." } });
                            }
                        }
                        else
                        {
                            return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to delete Party." } });
                        }
                    }
                }
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
