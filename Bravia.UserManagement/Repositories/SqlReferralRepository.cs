using Braphia.UserManagement.Database;
using Braphia.UserManagement.Events;
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.UserManagement.Repositories
{
    public class SqlReferralRepository : IReferralRepository
    {
        private DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public SqlReferralRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<bool> AddReferralAsync(Referral referral)
        {
            if (referral == null)
                throw new ArgumentNullException(nameof(referral), "Referral cannot be null.");
            await _context.Referral.AddAsync(referral);
            await _context.SaveChangesAsync();
            await _publishEndpoint.Publish(new Message(new ReferralSubmittedEvent(referral)));
            return true;
        }

        public async Task<bool> UpdateReferralAsync(Referral referral)
        {
            if (referral == null)
                throw new ArgumentNullException(nameof(referral), "Referral cannot be null.");
            var existingReferral = await _context.Referral.FirstOrDefaultAsync(r => r.Id == referral.Id) ?? throw new ArgumentException($"Referral with ID {referral.Id} not found.");
            existingReferral.PatientId = referral.PatientId;
            existingReferral.GeneralPracticionerId = referral.GeneralPracticionerId;
            existingReferral.Reason = referral.Reason;
            existingReferral.ReferralDate = referral.ReferralDate;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReferralAsync(int referralId)
        {
            var referral = await _context.Referral.FirstOrDefaultAsync(r => r.Id == referralId) ?? throw new ArgumentException($"Referral with ID {referralId} not found.");
            _context.Referral.Remove(referral);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Referral?> GetReferralByIdAsync(int referralId)
        {
            return await _context.Referral.FirstOrDefaultAsync(r => r.Id == referralId);
        }

        public async Task<IEnumerable<Referral>> GetAllReferralsAsync()
        {
            return await _context.Referral.ToListAsync();
        }

        public async Task<IEnumerable<Referral>> GetReferralsByPatientIdAsync(int patientId)
        {
            return await _context.Patient
                .Include(p => p.Referrals)
                .Where(p => p.Id == patientId)
                .SelectMany(p => p.Referrals.AsEnumerable())
                .ToListAsync();
        }

        public async Task<IEnumerable<Referral>> GetReferralsByGeneralPracticionerIdAsync(int generalPracticionerId)
        {
            return await _context.GeneralPracticioner
               .Include(gp => gp.Referrals)
               .Where(gp => gp.Id == generalPracticionerId)
               .SelectMany(gp => gp.Referrals.AsEnumerable())
               .ToListAsync();
        }
    }
}
