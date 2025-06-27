using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;

namespace Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface
{
    public interface IMongoAppointmentReadRepository
    {
        Task<bool> AddAppointmentAsync(AppointmentViewQueryModel appointment);
        Task<bool> UpdateAppointmentAsync(AppointmentViewQueryModel appointment);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
        //Task<AppointmentViewQueryModel> GetAppointmentByIdAsync(Guid appointmentId);
        //Task<IEnumerable<AppointmentViewQueryModel>> GetAllAppointmentsAsync();
        //Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsByPatientIdAsync(Guid patientId);
        //Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsByPhysicianIdAsync(Guid physicianId);
        //Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsOfTodayAsync();
    }
}
