namespace IVS_API.Models
{
    public class ElectionModel
    {
        public long ElectionId { get; set; } 
        public string StateName { get; set; }
        public int StateId { get; set;}
        public DateOnly electionDate { get; set; }
    }
}
