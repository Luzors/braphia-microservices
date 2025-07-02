using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using MediatR;

namespace Braphia.AppointmentManagement.Query.GetAppointmentIdChecked
{
    public class GetAppointmentIdCheckedHandler : IRequestHandler<GetAppointmentIdCheckedQuery, bool>
    {
        private readonly IAppointmentReadRepository _repository;
        public GetAppointmentIdCheckedHandler(IAppointmentReadRepository appointmentRepository)
        {
            _repository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository), "Appointment repository cannot be null.");
        }
        public async Task<bool> Handle(GetAppointmentIdCheckedQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Query cannot be null.");
            }
            var appointment = await _repository.GetAppointmentByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                throw new ArgumentException($"Appointment with ID {request.AppointmentId} not found.", nameof(request.AppointmentId));
            }
            return appointment.IsIdChecked;
        }
    }
}
