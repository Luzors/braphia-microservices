using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Enums;
using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Events.InternalEvents;
using Braphia.AppointmentManagement.Models;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories
{
    public class SqlAppointmentRepository : IAppointmentRepository
    {
        private readonly DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public SqlAppointmentRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context must be of type WriteDbContext.");
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }
        public async Task<bool> AddAppointmentAsync(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            await _context.Appointments.AddAsync(appointment);
            var succes = await _context.SaveChangesAsync() > 0;
            if (!succes) return false;
            await _publishEndpoint.Publish(new Message(new AppointmentScheduledEvent(appointment)));
            return true;
        }
        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            Console.WriteLine("UpdateAppointment");
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            var existingAppointment = await _context.Appointments.FindAsync(appointment.Id)
                ?? throw new ArgumentException($"Appointment with ID {appointment.Id} not found.");
            existingAppointment.PatientId = appointment.PatientId;
            existingAppointment.PhysicianId = appointment.PhysicianId;
            existingAppointment.ScheduledTime = appointment.ScheduledTime;
            existingAppointment.state = appointment.state;
            existingAppointment.ReferralId = appointment.ReferralId;
            existingAppointment.IsPreAppointmentQuestionnaireFilled = appointment.IsPreAppointmentQuestionnaireFilled;
            if (appointment.PreAppointmentQuestionnaire != null)
            {
                existingAppointment.PreAppointmentQuestionnaire = appointment.PreAppointmentQuestionnaire;
            }

            _context.Appointments.Update(existingAppointment);
            var succes = await _context.SaveChangesAsync() > 0;
            if (!succes) return false;
            await _publishEndpoint.Publish(new Message(new AppointmentModifiedEvent(appointment.Id, existingAppointment)));
            return true;
        }
        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            var appointment = await GetAppointmentByIdAsync(appointmentId);
            if (appointment == null) return false;
            _context.Appointments.Remove(appointment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.patient)
                .Include(a => a.physician)
                .Include(a => a.referral)
                .Include(a => a.receptionist)
                .FirstOrDefaultAsync(a => a.Id == appointmentId)
                ?? await _context.Appointments
                .FindAsync(appointmentId);
            return appointment ?? throw new ArgumentException($"Appointment with ID {appointmentId} not found.");
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments.ToListAsync()
                   ?? throw new ArgumentException("No appointments found in the database.");
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.patient)
                .Include(a => a.physician)
                .Include(a => a.referral)
                .Include(a => a.receptionist)
                .ToListAsync()
                ?? throw new ArgumentException($"No appointments found for patient with ID {patientId}.");
        }

        public async Task<bool> AddFollowUpAppointmentAsync(int appointmentId, Appointment followUpAppointment)
        {
            if (followUpAppointment == null)
                throw new ArgumentNullException(nameof(followUpAppointment), "Follow-up appointment cannot be null.");
            var existingAppointment = await GetAppointmentByIdAsync(appointmentId);
            if (existingAppointment == null)
                throw new ArgumentException($"Appointment with ID {appointmentId} not found.");
            existingAppointment.SetFollowUpAppointment(followUpAppointment);
            _context.Appointments.Update(existingAppointment);
            var succes = await _context.SaveChangesAsync() > 0;
            if (!succes) return false;
            await _publishEndpoint.Publish(new Message(new AppointmentScheduledFollowUpEvent(existingAppointment.Id, followUpAppointment)));
            return true;
        }

        public async Task<bool> UpdateAppointmentStateAsync(int patientId, AppointmentStateEnum state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state), "State cannot be null.");
            var appointment = await GetAppointmentByIdAsync(patientId);
            if (appointment == null)
                throw new ArgumentException($"Appointment with ID {patientId} not found.");
            appointment.state = state;
            _context.Appointments.Update(appointment);
            var succes = await _context.SaveChangesAsync() > 0;
            if (!succes) return false;
            await _publishEndpoint.Publish(new Message(new InternalAppointmentStateChangedEvent() { AppointmentId = appointment.Id, NewState = state}));
            return true;
        }

        public async Task<bool> AddFollowUpAppointment(int appointmentId, Appointment followUpAppointment)
        {
            if (followUpAppointment == null)
                throw new ArgumentNullException(nameof(followUpAppointment), "Follow-up appointment cannot be null.");
            var originalAppointment = await GetAppointmentByIdAsync(appointmentId);
            if (originalAppointment == null)
                throw new ArgumentException($"Original appointment with ID {appointmentId} not found.");
            var result = await AddAppointmentAsync(followUpAppointment);
            if (!result)
                throw new InvalidOperationException("Failed to add follow-up appointment.");
            originalAppointment.SetFollowUpAppointment(followUpAppointment);
            var succes = await UpdateAppointmentAsync(originalAppointment);
            if (!succes) return false;
            await _publishEndpoint.Publish(new Message(new AppointmentScheduledFollowUpEvent(originalAppointment.Id, followUpAppointment)));
            return true;
        }
    }
}
