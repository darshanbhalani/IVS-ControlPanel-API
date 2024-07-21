namespace IVS_API.Models.StateElection
{
    public class StateElectionModel
    {
        public long? StateElectionId { get; set; }
        public string? ElectionStageName { get; set; }
        public int? ElectionStageId { get; set; }
        public string? StateName { get; set; }
        public int? StateId { get; set; }
        public DateOnly? ElectionDate { get; set; }
        public int? VerificationStatus { get; set; }
        public string? VerificationStatusName { get; set; }
    }
}
