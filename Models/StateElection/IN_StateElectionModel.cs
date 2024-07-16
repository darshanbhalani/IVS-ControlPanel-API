using Newtonsoft.Json;

namespace IVS_API.Models.StateElection
{
    public class IN_StateElectionModel
    {
        //public StateElectionModel? Election { get; set; }
        public long? StateElectionId { get; set; }
        public string ElectionDate { get; set; }
        public int? StateId { get; set; }
        public long ActionBy { get; set; }
    }
}
