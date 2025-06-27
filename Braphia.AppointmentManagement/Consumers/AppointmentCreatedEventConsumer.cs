using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Models.States;
using MassTransit;

namespace Braphia.AppointmentManagement.Consumers
{
    public class AppointmentCreatedEventConsumer : IConsumer<AppointmentCreatedEvent>
    {
        private readonly AppointmentReadRepository _readRepo;

        public AppointmentCreatedEventConsumer(AppointmentReadRepository readRepo)
        {
            _readRepo = readRepo;
        }

        public async Task Consume(ConsumeContext<AppointmentCreatedEvent> context)
        {
            var evt = context.Message;

            var viewModel = new AppointmentViewQueryModel
            {
                AppointmentId = evt.AppointmentId,
                PatientId = evt.PatientId,
                PatientFirstName = evt.PatientFirstName,
                PatientLastName = evt.PatientLastName,
                PatientEmail = evt.PatientEmail,
                PatientPhoneNumber = evt.PatientPhoneNumber,

                PhysicianId = evt.PhysicianId,
                PhysicianFirstName = evt.PhysicianFirstName,
                PhysicianLastName = evt.PhysicianLastName,
                PhysicianSpecialization = evt.PhysicianSpecialization,

                ReceptionistId = evt.ReceptionistId,
                ReceptionistFirstName = evt.ReceptionistFirstName,
                ReceptionistLastName = evt.ReceptionistLastName,
                ReceptionistEmail = evt.ReceptionistEmail,

                ReferralId = evt.ReferralId,
                ReferralDate = evt.ReferralDate,
                ReferralReason = evt.ReferralReason,

                StateName = evt.StateName,
            };

            await _readRepo.AddAppointmentAsync(viewModel);
        }
    }

}
