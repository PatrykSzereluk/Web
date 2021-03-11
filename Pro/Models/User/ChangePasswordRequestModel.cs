namespace Pro.Models.User
{
    public class ChangePasswordRequestModel : BaseRequestModel
    {
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
