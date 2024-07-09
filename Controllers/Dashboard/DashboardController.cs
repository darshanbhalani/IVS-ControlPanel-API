using IVS_API.Models.Dashboard;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data.Common;

namespace IVS_API.Controllers.Dashboard
{
    [Route("[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        public DashboardController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        [HttpGet("GetVotersPerYear")]
        public IActionResult GetVotersPerYear()
        {
            DateTime timeStamp = TimeZoneIST.now();
            DashboardModel dashboard = new DashboardModel();
            List<YearWiseVotersModel> voters = new List<YearWiseVotersModel>();
            CountsModel counts = new CountsModel();
            IVotesModel iVotes = new IVotesModel();
            GenderWiseVotersModel totalVoters = new GenderWiseVotersModel();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_DASHBOARD_GETCOUNTS()", _connection))
                {

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            counts.TotalStates = reader.GetInt64(reader.GetOrdinal("totalstates"));
                            counts.TotalDistricts = reader.GetInt64(reader.GetOrdinal("totaldistricts"));
                            counts.TotalAssemblies = reader.GetInt64(reader.GetOrdinal("totalassemblies"));
                        }
                    }
                }
                
                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_DASHBOARD_COUNTVOTERBYGENDER()", _connection))
                {

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            totalVoters.Male = reader.GetInt64(reader.GetOrdinal("male"));
                            totalVoters.Female = reader.GetInt64(reader.GetOrdinal("female"));
                            totalVoters.Other = reader.GetInt64(reader.GetOrdinal("other"));
                            totalVoters.Total = reader.GetInt64(reader.GetOrdinal("total"));
                        }
                    }
                }

                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_DASHBOARD_COUNTVOTERSYEARWISE()", _connection))
                {

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            voters.Add(
                                 new YearWiseVotersModel
                                 {
                                     Male = reader.GetInt64(reader.GetOrdinal("male")),
                                     Female = reader.GetInt64(reader.GetOrdinal("female")),
                                     Other = reader.GetInt64(reader.GetOrdinal("other")),
                                     Total = reader.GetInt64(reader.GetOrdinal("male")) + reader.GetInt64(reader.GetOrdinal("female")) + reader.GetInt64(reader.GetOrdinal("other")),
                                     Year = reader.GetInt32(reader.GetOrdinal("year"))
                                 }
                                );
                        }
                    }
                }

                using (var cmd = new NpgsqlCommand("SELECT * FROM IVS_DASHBOARD_TOTALIV()", _connection))
                {

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            iVotes.Male = reader.GetInt64(reader.GetOrdinal("male"));
                            iVotes.Female = reader.GetInt64(reader.GetOrdinal("female"));
                            iVotes.Other = reader.GetInt64(reader.GetOrdinal("other"));
                            iVotes.Total = reader.GetInt64(reader.GetOrdinal("total"));
                        }
                    }
                }
                dashboard.Counts = counts;
                dashboard.GenderWiseVoters = totalVoters;
                dashboard.YearWiseVoters = voters;
                dashboard.IVotes = iVotes;

                return Ok(new { success = true, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { data = dashboard } });

            }
            catch (NpgsqlException pex)
            {
                    return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch data. Some thing went wrong." } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, header = new { requestTime = timeStamp, responsTime = TimeZoneIST.now() }, body = new { error = "Unable to fetch data. Some thing went wrong." } });
            }
        }
    }
}
