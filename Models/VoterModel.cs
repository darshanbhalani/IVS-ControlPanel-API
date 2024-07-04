namespace IVS_API.Models
{
    public class VoterModel
    {
        public long VoterId { get; set; }
        public string VoterEpic { get; set; }
        public string VoterName { get; set; }
        public string VoterFatherName { get; set; }
        public long VoterPhoneNumber { get; set; }
        public DateTime VoterBirthDate { get; set; }
        public string VoterGender { get; set; }
        public string VoterAddress { get; set; }
        public int VoterAssemblyId { get; set; }
        public string StateAssemblyName { get; set; }
        public int VoterDistrictId { get; set; }
        public string DistrictName { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ApprovedOn { get; set; }
    }
}
