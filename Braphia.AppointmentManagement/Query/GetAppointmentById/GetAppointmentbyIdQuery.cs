using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using MediatR;

namespace Braphia.AppointmentManagement.Query.GetAppointmentById
{
    public class GetAppointmentByIdQuery : IRequest<AppointmentViewQueryModel>
    {
        public int AppointmentId { get; set; }
        public GetAppointmentByIdQuery(int appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}
