using MediatR;

namespace BlogCMS.Core.Events.LoginSuccessed
{
    public class LoginSuccessEvent : INotification
    {
        public string Email { get; set; }

        public LoginSuccessEvent(string email)
        {
            Email = email;
        }
    }
}
