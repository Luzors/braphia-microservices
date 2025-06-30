using Braphia.AppointmentManagement.Models;
using Braphia.AppointmentManagement.Models.States;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<bool> AddAppointmentAsync( Appointment appointment );
        Task<bool> UpdateAppointmentAsync(Appointment appointment );
        Task<bool> DeleteAppointmentAsync( int appointmentId );
        Task<Appointment> GetAppointmentByIdAsync(int appointmentId );
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<bool> AddFollowUpAppointmentAsync(int appointmentId, Appointment followUpAppointment );
        Task<bool> UpdateAppointmentStateAsync(int patientId, IAppointmentState appointmentState);

    }
}
