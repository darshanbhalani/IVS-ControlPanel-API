namespace IVS_API.Models.Dashboard
{
    public class DashboardModel
    {
        public CountsModel Counts { get; set; }
        public GenderWiseVotersModel GenderWiseVoters { get; set; }
        public IVotesModel IVotes { get; set; }
        public List<YearWiseVotersModel> YearWiseVoters { get; set; }
    }
}
