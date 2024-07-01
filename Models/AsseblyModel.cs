namespace IVS_API.Models
{
    public class AsseblyModel
    {
        public int AsseblyId { get; set; }
        public string AsseblyName { get; set; }
        public string AsseblyDistrict { get; set; }
        public string FusionDistrictId { get; set; }
        public long TotalVoters { get; set; }
    }
}
