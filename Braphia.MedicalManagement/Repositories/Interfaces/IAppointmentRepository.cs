using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> GetAppointmentAsync(int id);

        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();

        Task<bool> AddAppointmentAsync(Appointment appointment, bool ignoreIdentity = false);

        Task<bool> DeleteAppointmentAsync(int id);

        Task<bool> UpdateAppointmentAsync(Appointment appointment, bool ignoreIdentity = false);
    }
}
