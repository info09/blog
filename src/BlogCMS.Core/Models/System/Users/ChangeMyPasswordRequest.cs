namespace BlogCMS.Core.Models.System.Users
{
    public class ChangeMyPasswordRequest
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
