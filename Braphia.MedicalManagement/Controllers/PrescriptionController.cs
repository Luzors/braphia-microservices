using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.MedicalManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrescriptionController : Controller
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly ILogger<PrescriptionController> _logger;

        public PrescriptionController(IPrescriptionRepository prescriptionRepository, ILogger<PrescriptionController> logger)
        {
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IEnumerable<Prescription>> GetAsync()
        {
            _logger.LogInformation("Fetching all prescriptions");
            try
            {
                IEnumerable<Prescription> prescriptions = await _prescriptionRepository.GetAllPrescriptionsAsync();
                return prescriptions;
            } catch
            {
                _logger.LogError("Error fetching prescriptions");
                throw new Exception("Internal server error while fetching prescriptions");
            }

        }
    }
}
