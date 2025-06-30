using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Query.GetAppointmentById;
using MediatR;

namespace Braphia.AppointmentManagement.Query.GetAppointmentIdChecked
{
    public class GetAppointmentIdCheckedQuery : IRequest<bool> 
    {
        public int AppointmentId { get; set; }
        public GetAppointmentIdCheckedQuery(int appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}