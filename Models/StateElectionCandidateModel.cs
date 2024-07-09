namespace IVS_API.Models
{
    public class StateElectionCandidateModel
    {
        public long? Id { get; set; }  
        public string? Name { get; set;}
        public byte[]? ProfileUrl { get; set;}
        public string? Gender { get; set;}
        public long? PartyId { get; set;}
        public string? PartyName { get; set;}
        public string? Epic { get; set;}
        public int? AssemblyId { get; set;}
        public string? AssemblyName { get; set;}
        public long? ElectionId { get; set;}
        public string? verificationStatus { get; set; }

    }
}
