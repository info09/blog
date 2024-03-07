using MediatR;

namespace BlogCMS.Core.Events.RegisterSuccessed
{
    public class RegisterSuccessHandler : INotificationHandler<RegisterSuccessEvent>
    {
        public Task Handle(RegisterSuccessEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
