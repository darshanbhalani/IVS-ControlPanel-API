using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IVS_API.Hubs;

namespace IVS_API.Controllers.Parties
{
    [Route("[controller]")]
    [ApiController]
    public class ElectionPartyController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        private readonly IHubContext<ElectionPartyHub> _hubContext;

        public ElectionPartyController(NpgsqlConnection connection, IHubContext<ElectionPartyHub> hubContext)
        {
            _connection = connection;
            _connection.Open();
            _hubContext = hubContext;
        }

        [HttpGet("GetAllParties")]
        public async Task<IActionResult> GetAllParties()
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
        public async Task<IActionResult> GetAllVerifiedParties()
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
        public async Task<IActionResult> AddNewParty([FromForm] ElectionPartyModel party, IFormFile image)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<ElectionPartyModel> parties = new List<ElectionPartyModel>();
            try
            {
                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await broadcastParties();
                        party.ElectionPartyLogoUrl = memoryStream.ToArray();
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
                                await broadcastParties();
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

        [HttpGet("VerifyParty")]
        public async Task<IActionResult> VerifyParty(long partyid, long verifiedby)
        {
            bool success = false;
            DateTime timeStamp = TimeZoneIST.now();
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
                            success = reader.GetBoolean(0);
                        }
                        else
                        {
                            return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to verify Party." } });
                        }
                    }
                }
                if (success)
                {
                    await broadcastParties();
                    return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Party successfully verified." } });
                }
                else
                {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to verify Party." } });
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
        public async Task<IActionResult> DeleteParty(long partyid, long deletedby)
        {
            bool success = false;
            DateTime timeStamp = TimeZoneIST.now();
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
                            success = reader.GetBoolean(0);
                        }
                        else
                        {
                            return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to delete Party." } });
                        }
                    }
                }
                if (success)
                {
                    await broadcastParties();
                    return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Party successfully deleted." } });
                }
                else
                {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to delete Party." } });
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

        [HttpPost("UpdateParty")]
        public async Task<IActionResult> UpdateParty([FromForm] ElectionPartyModel party, IFormFile? image)
        {
            bool success = false;
            DateTime timeStamp = TimeZoneIST.now();
            try
            {
                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        party.ElectionPartyLogoUrl = memoryStream.ToArray();
                    }
                    using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_PARTIES_UPDATEPARTY1(@partid,@logo,@name,@deletedby)", _connection))
                    {
                        cmd.Parameters.AddWithValue("partid", party.ElectionPartyId!);
                        cmd.Parameters.AddWithValue("logo", party.ElectionPartyLogoUrl != null ? (object)party.ElectionPartyLogoUrl : DBNull.Value);
                        cmd.Parameters.AddWithValue("name", party.ElectionPartyName!);
                        cmd.Parameters.AddWithValue("deletedby", 1);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                success = reader.GetBoolean(0);
                            }
                            else
                            {
                                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to update Party." } });
                            }
                        }
                    }
                    if (success)
                    {
                        await broadcastParties();
                        return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Party details successfully updated." } });
                    }
                    else
                    {
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to update Party." } });
                    }
                }
                else
                {
                    using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_PARTIES_UPDATEPARTY2(@partyid,@name,@deletedby)", _connection))
                    {
                        cmd.Parameters.AddWithValue("partyid", party.ElectionPartyId!);
                        cmd.Parameters.AddWithValue("name", party.ElectionPartyName!);
                        cmd.Parameters.AddWithValue("deletedby", 1);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                success = reader.GetBoolean(0);
                            }
                            else
                            {
                                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to update Party." } });
                            }
                        }
                    }
                    if (success)
                    {
                        await broadcastParties();
                        return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Party details successfully updated." } });
                    }
                    else
                    {
                        return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to update Party." } });
                    }
                }
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to update Party." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { message = "Unable to update Party." } });
            }
        }

        private async Task broadcastParties()
        {
            try
            {
                var data = GetAllParties();
                await _hubContext.Clients.All.SendAsync("Broadcast-Parties", data);
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
