using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;

namespace Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface
{
    public interface IAppointmentReadRepository
    {
        Task<bool> AddAppointmentAsync(AppointmentViewQueryModel appointment);
        Task<bool> UpdateAppointmentAsync(AppointmentViewQueryModel appointment);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
        Task<AppointmentViewQueryModel> GetAppointmentByIdAsync(int appointmentId);
        Task<IEnumerable<AppointmentViewQueryModel>> GetAllAppointmentsAsync();
        Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsByPatientIdAsync(int patientId);
        Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsByPhysicianIdAsync(int physicianId);
        Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsOfTodayAsync();

        Task<bool> UserIdChecked(int userId);

    }
}
