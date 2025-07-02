using MediatR;

namespace Braphia.AppointmentManagement.Commands.UserCheckId
{
    public class UserCheckIdCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public UserCheckIdCommand(int userId)
        {
            UserId = userId;
        }
    }
}
