using Braphia.AppointmentManagement.Models;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<bool> AddAppointmentAsync( Appointment appointment );
        Task<bool> UpdateAppointmentAsync( int id,Appointment appointment );
        Task<bool> DeleteAppointmentAsync( int appointmentId );
        Task<Appointment> GetAppointmentByIdAsync(int appointmentId );
        Task<bool> AddFollowUpAppointmentAsync(int appointmentId, Appointment followUpAppointment );


    }
}
