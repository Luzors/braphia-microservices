using Laboratory.Models;

namespace Laboratory.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<bool> AddAppointmentAsync(Appointment appointment);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> DeleteAppointmentAsync(Guid appointmentId);
        Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
    }
}
