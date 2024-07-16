using IVS_API.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace IVS_API.Controllers.StateElections
{
    [Route("[controller]")]
    [ApiController]
    public class LiveElectionController : ControllerBase
    {
        private readonly NpgsqlConnection _connection;
        private readonly IHubContext<ElectionPartyHub> _hubContext;
        public LiveElectionController(NpgsqlConnection connection, IHubContext<ElectionPartyHub> hubContext)
        {
            _connection = connection;
            _connection.Open();
            _hubContext = hubContext;
        }


        private async Task broadcastLiveSatistics()
        {
            try
            {
            //    var data = GetAllStateElections();
            //    await _hubContext.Clients.All.SendAsync("Broadcast-LiveSatistics", data);
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
