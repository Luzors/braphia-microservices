using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<bool> AddAppointmentAsync(Appointment appointment);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
    }
}
