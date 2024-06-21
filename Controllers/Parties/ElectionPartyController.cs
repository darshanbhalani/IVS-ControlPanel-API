using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

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
                                    ElectionPartyLogoUrl = reader.GetString(reader.GetOrdinal("electionpartyprofileurl")),
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
                                ElectionPartyLogoUrl = reader.GetString(reader.GetOrdinal("electionpartyprofileurl")),
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
        public IActionResult AddNewParty(ElectionPartyModel party, long createdby)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<ElectionPartyModel> parties = new List<ElectionPartyModel>();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_PARTY_ADDNEWPARTY(@partylogourl,@partyname,@createdby)", _connection))
                {
                    cmd.Parameters.AddWithValue("partylogourl", party.ElectionPartyLogoUrl);
                    cmd.Parameters.AddWithValue("partyname", party.ElectionPartyName);
                    cmd.Parameters.AddWithValue("createdby", createdby);
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
                                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to add Party." } });
                            }
                        }
                        else
                        {
                            return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to add Party." } });
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
       
        [HttpPut("VefifyParty")]
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

        [HttpDelete("DeleteParty")]
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
