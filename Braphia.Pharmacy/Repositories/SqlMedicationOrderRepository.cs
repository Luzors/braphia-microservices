using Braphia.Pharmacy.Database;
using Braphia.Pharmacy.Events;
using Braphia.Pharmacy.Models;
using Braphia.Pharmacy.Repositories.Interfaces;
using Infrastructure.Messaging;
using k8s.KubeConfigModels;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Pharmacy.Repositories
{
    public class SqlMedicationOrderRepository : IMedicationOrderRepository
    {
        private readonly ILogger<SqlMedicationOrderRepository> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly DBContext _context;

        public SqlMedicationOrderRepository(DBContext context, ILogger<SqlMedicationOrderRepository> logger, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> AddMedicationToMedicationOrderAsync(int medicationOrderId, Medication medication, int amount)
        {
            try
            {
                var medicationOrder = await _context.MedicationOrder.FindAsync(medicationOrderId);
                if (medicationOrder == null)
                {
                    _logger.LogWarning("Medication order with ID {MedicationOrderId} not found.", medicationOrderId);
                    return false;
                }
                medicationOrder.AddItem(medication, amount);
                _context.MedicationOrder.Update(medicationOrder);
                await _context.SaveChangesWithIdentityInsertAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding medication to medication order with ID {MedicationOrderId}.", medicationOrderId);
                return false;
            }
        }

        public async Task<bool> CompleteMedicationOrderAsync(int medicationOrderId)
        {
            try
            {
                var medicationOrder = await _context.MedicationOrder.FindAsync(medicationOrderId);
                if (medicationOrder == null)
                {
                    _logger.LogWarning("Medication order with ID {MedicationOrderId} not found.", medicationOrderId);
                    return false;
                }
                medicationOrder.CompleteOrder();
                _context.MedicationOrder.Update(medicationOrder);
                await _context.SaveChangesAsync();
                //TODO: event when order completed
                await _publishEndpoint.Publish(new Message(new MedicationOrderCompletedEvent(medicationOrder)));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing medication order with ID {MedicationOrderId}.", medicationOrderId);
                return false;
            }
        }

        public async Task<bool> CreateMedicationOrderAsync(MedicationOrder medicationOrder)
        {
            try
            {
                await _context.MedicationOrder.AddAsync(medicationOrder);
                await _context.SaveChangesWithIdentityInsertAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medication order.");
                return false;
            }
        }

        public async Task<bool> DeleteMedicationOrderAsync(int medicationOrderId)
        {
            try
            {
                var medicationOrder = await _context.MedicationOrder.FindAsync(medicationOrderId);
                if (medicationOrder == null)
                {
                    _logger.LogWarning("Medication order with ID {MedicationOrderId} not found.", medicationOrderId);
                    return false;
                }
                _context.MedicationOrder.Remove(medicationOrder);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medication order with ID {MedicationOrderId}.", medicationOrderId);
                return false;
            }
        }

        public async Task<IEnumerable<MedicationOrder>> GetAllMedicationOrdersAsync()
        {
            return await _context.MedicationOrder.ToListAsync();
        }

        public async Task<MedicationOrder?> GetMedicationOrderByIdAsync(int medicationOrderId)
        {
            return await _context.MedicationOrder.FindAsync(medicationOrderId);
        }

        public async Task<bool> RemoveMedicationFromMedicationOrderAsync(int medicationOrderId, Medication medication, int amount)
        {
            try
            {
                var medicationOrder = await _context.MedicationOrder.FindAsync(medicationOrderId);
                if (medicationOrder == null)
                {
                    _logger.LogWarning("Medication order with ID {MedicationOrderId} not found.", medicationOrderId);
                    return false;
                }
                medicationOrder.RemoveItem(medication, amount);
                _context.MedicationOrder.Update(medicationOrder);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing medication from medication order with ID {MedicationOrderId}.", medicationOrderId);
                return false;
            }
        }

        public async Task<bool> UpdateMedicationOrderAsync(int medicationOrderId, MedicationOrder updatedMedicationOrder)
        {
            try
            {
                var existingOrder = await _context.MedicationOrder.FindAsync(medicationOrderId);
                if (existingOrder == null)
                {
                    _logger.LogWarning("Medication order with ID {MedicationOrderId} not found.", medicationOrderId);
                    return false;
                }
                existingOrder.PatientId = updatedMedicationOrder.PatientId;
                existingOrder.PrescriptionId = updatedMedicationOrder.PrescriptionId;
                existingOrder.PharmacyId = updatedMedicationOrder.PharmacyId;
                existingOrder.Items = updatedMedicationOrder.Items;
                _context.MedicationOrder.Update(existingOrder);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medication order with ID {MedicationOrderId}.", medicationOrderId);
                return false;
            }
        }
    }
}
