using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using MediatR;


namespace Braphia.AppointmentManagement.Query.GetAppointmentsByPatient
{
    public class GetAppointmentsByPatientIdHandler : IRequestHandler<GetAppointmentsByPatientIdQuery, IEnumerable<AppointmentViewQueryModel>>
    {
        private readonly IAppointmentReadRepository _repository;
        public GetAppointmentsByPatientIdHandler(IAppointmentReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AppointmentViewQueryModel>> Handle(GetAppointmentsByPatientIdQuery request, CancellationToken cancellationToken)
        {
            
            return await _repository.GetAppointmentsByPatientIdAsync(request.PatientId);
        }
    }

}
