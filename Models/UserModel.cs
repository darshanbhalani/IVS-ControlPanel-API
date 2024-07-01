namespace IVS_API.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string ProfileUrl { get; set; }
        public string Code { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
    }
}
