using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Models;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories
{
    public class SqlReferralRepository : IReferralRepository
    {
        private DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public SqlReferralRepository(DbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context as DBContext ?? throw new ArgumentNullException(nameof(context), "Context must be of type WriteDbContext.");
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<bool> AddReferralAsync(Referral referral)
        {
            if (referral == null)
                throw new ArgumentNullException(nameof(referral), "Patient cannot be null.");
            await _context.Referral.AddAsync(referral);
            await _context.SaveChangesAsync();

            // Patient created event
            //await _publishEndpoint.Publish(new Message(new PatientRegisteredEvent(patient)));

            return true;
        }

        public async Task<bool> UpdateReferralAsync(Referral referral)
        {
            if (referral == null)
                throw new ArgumentNullException(nameof(referral), "Referral cannot be null.");

            var existingReferral = await _context.Referral.FindAsync(referral.Id) ?? throw new ArgumentException($"Referral with ID {referral.Id} not found.");
            existingReferral.PatientId = referral.PatientId;
            existingReferral.Reason = referral.Reason;
            existingReferral.ReferralDate = referral.ReferralDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReferralAsync(int referralId)
        {
            var referral = await _context.Referral.FindAsync(referralId) ?? throw new ArgumentException($"Referral with ID {referralId} not found.");
            _context.Referral.Remove(referral);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Referral> GetReferralByIdAsync(int referralId)
        {
            return await _context.Referral
                .FirstOrDefaultAsync(r => r.Id == referralId) 
                ?? throw new ArgumentException($"Referral with ID {referralId} not found.");
        }

        public async Task<IEnumerable<Referral>> GetAllReferralsAsync()
        {
            return await _context.Referral
                .ToListAsync() 
                ?? throw new ArgumentException("No referrals found.");
        }

        public Task<IEnumerable<Referral>> GetReferralsByPatientIdAsync(int patientId)
        {
            var referrals = _context.Referral
                .Where(r => r.PatientId == patientId)
                .ToListAsync();

            return referrals.ContinueWith(task => (IEnumerable<Referral>)task.Result);
        }
    }
}
