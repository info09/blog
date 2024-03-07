using MediatR;

namespace BlogCMS.Core.Events.LoginSuccessed
{
    public class LoginSuccessHandler : INotificationHandler<LoginSuccessEvent>
    {
        public Task Handle(LoginSuccessEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
