namespace IVS_API.Models
{
    public class ElectionPartyModel
    {
        public long? ElectionPartyId { get; set; }
        public byte[]? ElectionPartyLogoUrl { get; set; }
        public string? ElectionPartyName { get; set; }
        public string? VerificationStatus { get; set; }
    }
}
