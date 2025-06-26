using Braphia.AppointmentManagement.Models;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<bool> AddAppointmentAsync( Appointment appointment );
        Task<bool> UpdateAppointmentAsync( Appointment appointment );
        Task<bool> DeleteAppointmentAsync( Guid appointmentId );
        Task<bool> UpdateAppointmentAsync( Guid appointmentId, Appointment appointment );
        Task<Appointment> GetAppointmentByIdAsync( Guid appointmentId );

         
    }
}
