using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using MediatR;

namespace Braphia.AppointmentManagement.Query.GetAppointmentsByPatient
{
    public class GetAppointmentsByPatientIdQuery : IRequest<IEnumerable<AppointmentViewQueryModel>>
    {
        public int PatientId { get; set; }
        public GetAppointmentsByPatientIdQuery(int patientId) => PatientId = patientId;
    }
}
