using IVS_API.Models;
using Microsoft.AspNetCore.SignalR;

namespace IVS_API.Hubs
{
    public class ElectionPartyHub:Hub
    {
        public async Task SendPartiesUpdate(List<ElectionPartyModel> parties)
        {
            await Clients.All.SendAsync("PartiesUpdated", parties);
        }
    }
}
