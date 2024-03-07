using MediatR;

namespace BlogCMS.Core.Events.RegisterSuccessed
{
    public class RegisterSuccessEvent : INotification
    {
        public string Email { get; set; }

        public RegisterSuccessEvent(string email)
        {
            Email = email;
        }
    }
}
