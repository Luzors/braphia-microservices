using System.ComponentModel.DataAnnotations;
using Braphia.Accounting.Enums;

namespace Braphia.MedicalManagement.Models;

public class Test
{
    public Test() { }

    public Test(int rootId, int patientId, TestType testType, string description, decimal cost, DateTime completedDate, int medicalAnalysisId)
    {
        RootId = rootId;
        PatientId = patientId;
        TestType = testType;
        Description = description;
        Cost = cost;
        CompletedDate = completedDate;
        MedicalAnalysisId = medicalAnalysisId;
    }

    [Key]
    public int Id { get; set; }

    public int RootId { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    public TestType TestType { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public decimal Cost { get; set; }

    public DateTime CompletedDate { get; set; }
    
    public int MedicalAnalysisId { get; set; } 
}