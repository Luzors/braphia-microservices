# This powershell script runs a shitton of curl requests to seed the system
# Might be some AI inspired stuff :)

Write-Host "Starting Braphia System Seeding..." -ForegroundColor Green

# Base URLs for microservices (adjust ports as needed)
$AppointmentManagementBaseUrl = "https://localhost:7049/api"
$AccountingBaseUrl = "https://localhost:7076/api"
$LaboratoryBaseUrl = "https://localhost:7014/api"
$PharmacyBaseUrl = "https://localhost:7042/api"
$UserManagementBaseUrl = "https://localhost:7260/api"
$MedicalManagementBaseUrl = "https://localhost:7060/api"

# Function to make curl requests with error handling
function Invoke-CurlRequest {
    param(
        [string]$Url,
        [string]$Method = "POST",
        [string]$Data = "",
        [string]$ContentType = "application/json"
    )
    
    try {
        $headers = @{ "Content-Type" = $ContentType }
        if (($Method -eq "POST" -or $Method -eq "PUT") -and $Data -ne "") {
            $result = Invoke-WebRequest -Method $Method -Uri $Url -Headers $headers -Body $Data -UseBasicParsing
        } else {
            $result = Invoke-WebRequest -Method $Method -Uri $Url -Headers $headers -UseBasicParsing
        }
        Write-Host "Success: $Method $Url" -ForegroundColor Green
        return $result
    }
    catch {
        Write-Host "Failed: $Method $Url - $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "Data: $Data" -ForegroundColor Yellow
        return $null
    }
}

# Create Insurers first (needed for patients)
Write-Host "Creating Insurers..." -ForegroundColor Yellow

$insurers = @(
    @{ Name = "HealthCare Plus"; ContactEmail = "contact@healthcareplus.com"; ContactPhone = "+1-555-0101" },
    @{ Name = "MediCare Insurance"; ContactEmail = "info@medicare.com"; ContactPhone = "+1-555-0102" },
    @{ Name = "VitalCare"; ContactEmail = "support@vitalcare.com"; ContactPhone = "+1-555-0103" },
    @{ Name = "SecureHealth"; ContactEmail = "claims@securehealth.com"; ContactPhone = "+1-555-0104" },
    @{ Name = "PremiumCare"; ContactEmail = "service@premiumcare.com"; ContactPhone = "+1-555-0105" }
)

foreach ($insurer in $insurers) {
    $data = $insurer | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$AccountingBaseUrl/Insurer" -Data $data
}

# Create General Practitioners
Write-Host "Creating General Practitioners..." -ForegroundColor Yellow

$gps = @(
    @{ FirstName = "John"; LastName = "Smith"; Email = "j.smith@clinic.com"; PhoneNumber = "+1-555-1001"; BirthDate = "1980-01-01T00:00:00Z" },
    @{ FirstName = "Sarah"; LastName = "Johnson"; Email = "s.johnson@clinic.com"; PhoneNumber = "+1-555-1002"; BirthDate = "1985-05-05T00:00:00Z" },
    @{ FirstName = "Michael"; LastName = "Brown"; Email = "m.brown@clinic.com"; PhoneNumber = "+1-555-1003"; BirthDate = "1990-10-10T00:00:00Z" },
    @{ FirstName = "Emily"; LastName = "Davis"; Email = "e.davis@clinic.com"; PhoneNumber = "+1-555-1004"; BirthDate = "1992-02-02T00:00:00Z" },
    @{ FirstName = "David"; LastName = "Wilson"; Email = "d.wilson@clinic.com"; PhoneNumber = "+1-555-1005"; BirthDate = "1988-08-08T00:00:00Z" }
)

foreach ($gp in $gps) {
    $data = $gp | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$UserManagementBaseUrl/GeneralPracticioner" -Data $data
}

# Create Physicians
Write-Host "Creating Physicians..." -ForegroundColor Yellow

$physicians = @(
    @{ FirstName = "Robert"; LastName = "Anderson"; Email = "r.anderson@hospital.com"; PhoneNumber = "+1-555-2001"; BirthDate = "1975-12-25T00:00:00Z"; Specialization = 0 },
    @{ FirstName = "Lisa"; LastName = "Martinez"; Email = "l.martinez@hospital.com"; PhoneNumber = "+1-555-2002"; BirthDate = "1983-06-30T00:00:00Z"; Specialization = 8 },
    @{ FirstName = "James"; LastName = "Taylor"; Email = "j.taylor@hospital.com"; PhoneNumber = "+1-555-2003"; BirthDate = "1970-11-11T00:00:00Z"; Specialization = 4 },
    @{ FirstName = "Maria"; LastName = "Garcia"; Email = "m.garcia@hospital.com"; PhoneNumber = "+1-555-2004"; BirthDate = "1982-09-09T00:00:00Z"; Specialization = 1 },
    @{ FirstName = "Christopher"; LastName = "Lee"; Email = "c.lee@hospital.com"; PhoneNumber = "+1-555-2005"; BirthDate = "1978-03-03T00:00:00Z"; Specialization = 3 },
    @{ FirstName = "Amanda"; LastName = "White"; Email = "a.white@hospital.com"; PhoneNumber = "+1-555-2006"; BirthDate = "1986-07-07T00:00:00Z"; Specialization = 7 },
    @{ FirstName = "Kevin"; LastName = "Thompson"; Email = "k.thompson@hospital.com"; PhoneNumber = "+1-555-2007"; BirthDate = "1984-04-04T00:00:00Z"; Specialization = 2 },
    @{ FirstName = "Jessica"; LastName = "Miller"; Email = "j.miller@hospital.com"; PhoneNumber = "+1-555-2008"; BirthDate = "1990-08-08T00:00:00Z"; Specialization = 5 }
)

foreach ($physician in $physicians) {
    $data = $physician | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$UserManagementBaseUrl/Physician" -Data $data
}

# Create Receptionists (needed for appointments)
Write-Host "Creating Receptionists..." -ForegroundColor Yellow

$receptionists = @(
    @{ FirstName = "Alice"; LastName = "Cooper"; Email = "a.cooper@clinic.com"; PhoneNumber = "+1-555-3001"; BirthDate = "1985-03-15T00:00:00Z" },
    @{ FirstName = "Bob"; LastName = "Johnson"; Email = "b.johnson@clinic.com"; PhoneNumber = "+1-555-3002"; BirthDate = "1990-07-20T00:00:00Z" },
    @{ FirstName = "Carol"; LastName = "Williams"; Email = "c.williams@clinic.com"; PhoneNumber = "+1-555-3003"; BirthDate = "1988-11-10T00:00:00Z" },
    @{ FirstName = "David"; LastName = "Brown"; Email = "d.brown@clinic.com"; PhoneNumber = "+1-555-3004"; BirthDate = "1992-05-25T00:00:00Z" }
)

foreach ($receptionist in $receptionists) {
    $data = $receptionist | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$UserManagementBaseUrl/Receptionist" -Data $data
}

# Create Pharmacies
Write-Host "Creating Pharmacies..." -ForegroundColor Yellow

$pharmacies = @(
    @{ Name = "CityPharm Downtown"; Address = "123 Main St, Downtown"; PhoneNumber = "+1-555-4001"; Email = "downtown@citypharm.com" },
    @{ Name = "HealthMart Plaza"; Address = "456 Oak Ave, Midtown"; PhoneNumber = "+1-555-4002"; Email = "plaza@healthmart.com" },
    @{ Name = "MediCenter Pharmacy"; Address = "789 Pine Rd, Uptown"; PhoneNumber = "+1-555-4003"; Email = "uptown@medicenter.com" },
    @{ Name = "QuickCare Drugs"; Address = "321 Elm St, Westside"; PhoneNumber = "+1-555-4004"; Email = "westside@quickcare.com" },
    @{ Name = "WellnessRx"; Address = "654 Maple Dr, Eastside"; PhoneNumber = "+1-555-4005"; Email = "eastside@wellnessrx.com" }
)

foreach ($pharmacy in $pharmacies) {
    $data = $pharmacy | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$PharmacyBaseUrl/Pharmacy" -Data $data
}

# Create Medications
Write-Host "Creating Medications..." -ForegroundColor Yellow

$medications = @(
    @{ Name = "Aspirin"; Dosage = "325mg"; Manufacturer = "PharmaCorp"; Price = 12.99; Description = "Pain reliever and anti-inflammatory"; ExpiryDate = "2028-07-01T16:30:39.022Z" },
    @{ Name = "Ibuprofen"; Dosage = "200mg"; Manufacturer = "MediLab"; Price = 8.49; Description = "Nonsteroidal anti-inflammatory drug"; ExpiryDate = "2028-07-01T16:30:39.022Z" },
    @{ Name = "Acetaminophen"; Dosage = "500mg"; Manufacturer = "HealthGen"; Price = 6.99; Description = "Pain reliever and fever reducer"; ExpiryDate = "2028-07-01T16:30:39.022Z" },
    @{ Name = "Amoxicillin"; Dosage = "250mg"; Manufacturer = "BioPharm"; Price = 15.75; Description = "Antibiotic for bacterial infections"; ExpiryDate = "2028-07-01T16:30:39.022Z" },
    @{ Name = "Lisinopril"; Dosage = "10mg"; Manufacturer = "CardioMed"; Price = 22.50; Description = "ACE inhibitor for high blood pressure"; ExpiryDate = "2028-07-01T16:30:39.022Z" },
    @{ Name = "Metformin"; Dosage = "500mg"; Manufacturer = "DiabetesCare"; Price = 18.25; Description = "Diabetes medication"; ExpiryDate = "2028-07-01T16:30:39.022Z" },
    @{ Name = "Atorvastatin"; Dosage = "20mg"; Manufacturer = "CholesterolRx"; Price = 28.99; Description = "Cholesterol lowering medication"; ExpiryDate = "2028-07-01T16:30:39.022Z" },
    @{ Name = "Omeprazole"; Dosage = "20mg"; Manufacturer = "GastroMed"; Price = 16.75; Description = "Proton pump inhibitor"; ExpiryDate = "2028-07-01T16:30:39.022Z" }
)

foreach ($medication in $medications) {
    $data = $medication | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$PharmacyBaseUrl/Medication" -Data $data
}

# Create Central Laboratories (needed for lab appointments)
Write-Host "Creating Central Laboratories..." -ForegroundColor Yellow

$centralLabs = @(
    @{ LaboratoryName = "Central Medical Lab"; Address = "100 Medical Center Dr"; PhoneNumber = "+1-555-5001"; Email = "contact@centralmedlab.com" },
    @{ LaboratoryName = "Advanced Diagnostics"; Address = "200 Science Blvd"; PhoneNumber = "+1-555-5002"; Email = "info@advanceddiag.com" },
    @{ LaboratoryName = "QuickTest Laboratory"; Address = "300 Research Ave"; PhoneNumber = "+1-555-5003"; Email = "support@quicktest.com" }
)

foreach ($lab in $centralLabs) {
    $data = $lab | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$LaboratoryBaseUrl/CentralLaboratory" -Data $data
}

# Create Referrals (required for appointments)
Write-Host "Creating Referrals..." -ForegroundColor Yellow

$referrals = @(
    @{ PatientId = 1; GeneralPracticionerId = 1; ReferralDate = "2024-01-01T00:00:00Z"; Reason = "Routine checkup" },
    @{ PatientId = 2; GeneralPracticionerId = 2; ReferralDate = "2024-01-02T00:00:00Z"; Reason = "Follow-up on test results" },
    @{ PatientId = 3; GeneralPracticionerId = 3; ReferralDate = "2024-01-03T00:00:00Z"; Reason = "Specialist consultation" },
    @{ PatientId = 4; GeneralPracticionerId = 4; ReferralDate = "2024-01-04T00:00:00Z"; Reason = "Chronic condition management" },
    @{ PatientId = 5; GeneralPracticionerId = 5; ReferralDate = "2024-01-05T00:00:00Z"; Reason = "Medication review" }
)

foreach ($referral in $referrals) {
    $data = $referral | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$UserManagementBaseUrl/Referral" -Data $data
}

# Create Appointments (using AppointmentManagement service)
Write-Host "Creating Appointments..." -ForegroundColor Yellow
$appointments = @(
    @{ PatientId = 1; PhysicianId = 1; ReceptionistId = 1; ReferralId = 1; ScheduledTime = "2024-01-15T09:00:00Z" },
    @{ PatientId = 2; PhysicianId = 2; ReceptionistId = 2; ReferralId = 2; ScheduledTime = "2024-01-16T10:30:00Z" },
    @{ PatientId = 3; PhysicianId = 3; ReceptionistId = 3; ReferralId = 3; ScheduledTime = "2024-01-17T14:00:00Z" },
    @{ PatientId = 4; PhysicianId = 4; ReceptionistId = 4; ReferralId = 4; ScheduledTime = "2024-01-18T11:15:00Z" },
    @{ PatientId = 5; PhysicianId = 5; ReceptionistId = 1; ReferralId = 5; ScheduledTime = "2024-01-19T08:45:00Z" }
)

foreach ($appointment in $appointments) {
    $data = $appointment | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$AppointmentManagementBaseUrl/Appointments" -Data $data
}

# Create Medical Analysis
Write-Host "Creating Medical Analysis..." -ForegroundColor Yellow

$medicalAnalyses = @(
    @{ PatientId = 1; PhysicianId = 1; Description = "Routine blood work"; AppointmentId = 1 },
    @{ PatientId = 2; PhysicianId = 2; Description = "Follow-up on diabetes management"; AppointmentId = 2 },
    @{ PatientId = 3; PhysicianId = 3; Description = "Chest pain evaluation"; AppointmentId = 3 },
    @{ PatientId = 4; PhysicianId = 4; Description = "Annual physical exam"; AppointmentId = 4 },
    @{ PatientId = 5; PhysicianId = 5; Description = "Medication review and adjustment"; AppointmentId = 5 }
)

foreach ($analysis in $medicalAnalyses) {
    $data = $analysis | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$MedicalManagementBaseUrl/MedicalAnalysis" -Data $data
}

# Create Prescriptions
Write-Host "Creating Prescriptions..." -ForegroundColor Yellow

$prescriptions = @(
    @{ Medicine = "Aspirin"; Dose = "325"; Unit = 0; PatientId = 1; PhysicianId = 1; MedicalAnalysisId = 1 },
    @{ Medicine = "Ibuprofen"; Dose = "200"; Unit = 1; PatientId = 2; PhysicianId = 2; MedicalAnalysisId = 2 },
    @{ Medicine = "Amoxicillin"; Dose = "250"; Unit = 0; PatientId = 3; PhysicianId = 3; MedicalAnalysisId = 3 },
    @{ Medicine = "Lisinopril"; Dose = "10"; Unit = 0; PatientId = 4; PhysicianId = 4; MedicalAnalysisId = 4 },
    @{ Medicine = "Metformin"; Dose = "500"; Unit = 1; PatientId = 5; PhysicianId = 5; MedicalAnalysisId = 5 },
    @{ Medicine = "Atorvastatin"; Dose = "20"; Unit = 0; PatientId = 1; PhysicianId = 1; MedicalAnalysisId = 1 }
)

foreach ($prescription in $prescriptions) {
    $data = $prescription | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$MedicalManagementBaseUrl/Prescription" -Data $data
}

# Create Lab Tests
Write-Host "Creating Laboratory Tests..." -ForegroundColor Yellow

$labTests = @(
    @{ patientId = 1; testType = 0; description = "Complete Blood Count"; cost = 45.00; medicalAnalysisId = 1 },
    @{ patientId = 1; testType = 1; description = "Urinalysis"; cost = 30.00; medicalAnalysisId = 2 },
    @{ patientId = 1; testType = 2; description = "Biopsy of the skin"; cost = 25.00; medicalAnalysisId = 3 },
    @{ patientId = 1; testType = 3; description = "Cultures"; cost = 65.00; medicalAnalysisId = 4 }, 
    @{ patientId = 2; testType = 1; description = "Urinalysis"; cost = 30.00; medicalAnalysisId = 2 },
    @{ patientId = 3; testType = 4; description = "Chest X-ray"; cost = 120.00; medicalAnalysisId = 3 },
    @{ patientId = 4; testType = 3; description = "Blood Glucose Test"; cost = 25.00; medicalAnalysisId = 4 },
    @{ patientId = 5; testType = 1; description = "Cholesterol Panel"; cost = 55.00; medicalAnalysisId = 5 },
    @{ patientId = 1; testType = 5; description = "MRI Scan"; cost = 65.00; medicalAnalysisId = 1 },
    @{ patientId = 2; testType = 6; description = "Ct of the liver"; cost = 40.00; medicalAnalysisId = 2 },
    @{ patientId = 3; testType = 7; description = "Ultrasound of the heart"; cost = 350.00; medicalAnalysisId = 3 }
)

foreach ($test in $labTests) {
    $data = $test | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$MedicalManagementBaseUrl/Test" -Data $data
}

# Create Medication Orders
Write-Host "Creating Medication Orders..." -ForegroundColor Yellow

$medicationOrders = @(
    @{ patientId = 1; prescriptionId = 1; pharmacyId = 1; createdAt = "2025-07-01T17:00:31.352Z" },
    @{ patientId = 2; prescriptionId = 2; pharmacyId = 2; createdAt = "2025-07-01T17:05:31.352Z" },
    @{ patientId = 3; prescriptionId = 3; pharmacyId = 3; createdAt = "2025-07-01T17:10:31.352Z" },
    @{ patientId = 4; prescriptionId = 4; pharmacyId = 4; createdAt = "2025-07-01T17:15:31.352Z" },
    @{ patientId = 5; prescriptionId = 5; pharmacyId = 5; createdAt = "2025-07-01T17:20:31.352Z" }
)

foreach ($order in $medicationOrders) {
    $data = $order | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$PharmacyBaseUrl/MedicationOrder" -Data $data
}

# Assign patients to insurers
Write-Host "Assigning Patients to Insurers..." -ForegroundColor Yellow

$patientInsurerAssignments = @(
    @{ PatientId = 1; InsurerId = 1 },
    @{ PatientId = 2; InsurerId = 2 },
    @{ PatientId = 3; InsurerId = 3 },
    @{ PatientId = 4; InsurerId = 4 },
    @{ PatientId = 5; InsurerId = 5 },
    @{ PatientId = 6; InsurerId = 1 },
    @{ PatientId = 7; InsurerId = 2 },
    @{ PatientId = 8; InsurerId = 3 },
    @{ PatientId = 9; InsurerId = 4 },
    @{ PatientId = 10; InsurerId = 5 }
)

foreach ($assignment in $patientInsurerAssignments) {
    Invoke-CurlRequest -Url "$AccountingBaseUrl/Patient/$($assignment.PatientId)/assign-insurer/$($assignment.InsurerId)" -Method "PUT"
}
# Add medications to orders
Write-Host "Adding Medications to Orders..." -ForegroundColor Yellow

$medicationAdditions = @(
    @{ OrderId = 1; MedicationId = 1; Amount = 1 },
    @{ OrderId = 2; MedicationId = 2; Amount = 1 },
    @{ OrderId = 3; MedicationId = 4; Amount = 1 },
    @{ OrderId = 4; MedicationId = 5; Amount = 1 },
    @{ OrderId = 5; MedicationId = 6; Amount = 1 }
)

foreach ($addition in $medicationAdditions) {
    Invoke-CurlRequest -Url "$PharmacyBaseUrl/MedicationOrder/$($addition.OrderId)/medication?medicationId=$($addition.MedicationId)&amount=$($addition.Amount)" -Method "POST"
}

# Complete some medication orders
Write-Host "Completing Some Medication Orders..." -ForegroundColor Yellow

$medicationOrdersToComplete = @(1, 2, 3)

foreach ($orderId in $medicationOrdersToComplete) {
    Invoke-CurlRequest -Url "$PharmacyBaseUrl/MedicationOrder/$orderId/complete" -Method "PUT"
}

# Complete some tests to trigger notifications
Write-Host "Completing Some Laboratory Tests..." -ForegroundColor Yellow

$testCompletions = @(
    @{ TestId = 1; Result = "Normal CBC values. WBC: 7.2, RBC: 4.5, Hemoglobin: 14.2" },
    @{ TestId = 2; Result = "Normal urinalysis. No protein, glucose, or bacteria detected" },
    @{ TestId = 3; Result = "Clear chest X-ray. No abnormalities detected" },
    @{ TestId = 4; Result = "Glucose: 95 mg/dL (normal), Creatinine: 1.0 mg/dL (normal)" },
    @{ TestId = 5; Result = "Total cholesterol: 185 mg/dL, HDL: 55 mg/dL, LDL: 110 mg/dL" }
)

foreach ($completion in $testCompletions) {
    $data = @{ Result = $completion.Result } | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$LaboratoryBaseUrl/Test/$($completion.TestId)/Complete" -Method "PUT" -Data $data
}

# Add the accounting seeding (pay of some invoices partially and completely)
Write-Host "Processing Invoice Payments..." -ForegroundColor Yellow

# 1. Get all invoices
try {
    $invoicesResponse = Invoke-CurlRequest -Url "$AccountingBaseUrl/Invoice" -Method "GET"
    $invoices = $invoicesResponse | ConvertFrom-Json
    
    if ($invoices -and $invoices.Count -gt 0) {
        Write-Host "Found $($invoices.Count) invoices to process" -ForegroundColor Cyan
        
        # 2. Take about 2/3rds of invoices
        $invoicesToProcess = $invoices | Select-Object -First ([Math]::Ceiling($invoices.Count * 2 / 3))
        Write-Host "Processing $($invoicesToProcess.Count) invoices for payments" -ForegroundColor Cyan
        
        # 3. Split them in half
        $halfCount = [Math]::Ceiling($invoicesToProcess.Count / 2)
        $invoicesForFullPayment = $invoicesToProcess | Select-Object -First $halfCount
        $invoicesForPartialPayment = $invoicesToProcess | Select-Object -Skip $halfCount
        
        # 4. Pay half of them completely
        Write-Host "Making Full Payments..." -ForegroundColor Green
        foreach ($invoice in $invoicesForFullPayment) {
            $paymentData = @{
                InsurerId = $invoice.InsurerId
                PaymentAmount = $invoice.AmountOutstanding
                PaymentReference = "FULL-PAY-$(Get-Random -Maximum 9999)"
            }
            
            $data = $paymentData | ConvertTo-Json -Compress
            Invoke-CurlRequest -Url "$AccountingBaseUrl/Invoice/$($invoice.Id)/payment" -Data $data -Method "POST"
        }
        
        # 5. Pay the other half partially (50-80% of amount)
        Write-Host "Making Partial Payments..." -ForegroundColor Green
        foreach ($invoice in $invoicesForPartialPayment) {
            $partialPercentage = Get-Random -Minimum 50 -Maximum 81  # 50-80%
            $partialAmount = [Math]::Round($invoice.AmountOutstanding * $partialPercentage / 100, 2)
            
            $paymentData = @{
                InsurerId = $invoice.InsurerId
                PaymentAmount = $partialAmount
                PaymentReference = "PARTIAL-PAY-$(Get-Random -Maximum 9999)"
            }
            
            $data = $paymentData | ConvertTo-Json -Compress
            Invoke-CurlRequest -Url "$AccountingBaseUrl/Invoice/$($invoice.Id)/payment" -Data $data -Method "POST"
        }

        # 6. Adjust invoice amount on one of the partially paid invoices and then make an additional partial payment
        if ($invoicesForPartialPayment.Count -gt 0) {
            $randomPartialInvoice = $invoicesForPartialPayment | Get-Random
            
            # First, make a negative invoice amount adjustment to demonstrate reduction functionality
            $negativeAdjustmentPercentage = Get-Random -Minimum 5 -Maximum 16  # 5-15% of original amount
            $negativeAdjustmentAmount = -[Math]::Round($randomPartialInvoice.TotalAmount * $negativeAdjustmentPercentage / 100, 2)
            
            $negativeAdjustmentReasons = @(
                "Insurance review reduction", 
                "Coverage limitation adjustment",
                "Claim denial - partial",
                "Network discount application",
                "Contract rate correction"
            )
            $negativeAdjustmentReason = $negativeAdjustmentReasons | Get-Random
            
            $negativeAdjustmentData = @{
                InsurerId = $randomPartialInvoice.InsurerId
                AdjustmentAmount = $negativeAdjustmentAmount
                Reason = $negativeAdjustmentReason
                Reference = "NEG-ADJ-$(Get-Random -Maximum 9999)"
            }
            
            Write-Host "Making negative adjustment of $negativeAdjustmentAmount for invoice $($randomPartialInvoice.Id). Reason: $negativeAdjustmentReason" -ForegroundColor Magenta
            $negativeAdjustmentJson = $negativeAdjustmentData | ConvertTo-Json -Compress
            Invoke-CurlRequest -Url "$AccountingBaseUrl/Invoice/$($randomPartialInvoice.Id)/adjustment" -Data $negativeAdjustmentJson -Method "POST"
            
            # Then make a positive invoice amount adjustment to demonstrate increase functionality
            $positiveAdjustmentPercentage = Get-Random -Minimum 3 -Maximum 10  # 3-9% of original amount (smaller than negative)
            $positiveAdjustmentAmount = [Math]::Round($randomPartialInvoice.TotalAmount * $positiveAdjustmentPercentage / 100, 2)
            
            $positiveAdjustmentReasons = @(
                "Additional services discovered",
                "Billing error correction",
                "Prior authorization change",
                "Clinical coding update",
                "Service upgrade approved"
            )
            $positiveAdjustmentReason = $positiveAdjustmentReasons | Get-Random
            
            $positiveAdjustmentData = @{
                InsurerId = $randomPartialInvoice.InsurerId
                AdjustmentAmount = $positiveAdjustmentAmount
                Reason = $positiveAdjustmentReason
                Reference = "POS-ADJ-$(Get-Random -Maximum 9999)"
            }
            
            Write-Host "Making positive adjustment of +$positiveAdjustmentAmount for invoice $($randomPartialInvoice.Id). Reason: $positiveAdjustmentReason" -ForegroundColor Cyan
            $positiveAdjustmentJson = $positiveAdjustmentData | ConvertTo-Json -Compress
            Invoke-CurlRequest -Url "$AccountingBaseUrl/Invoice/$($randomPartialInvoice.Id)/adjustment" -Data $positiveAdjustmentJson -Method "POST"
            
            # Then make an additional partial payment
            $additionalPartialPercentage = Get-Random -Minimum 10 -Maximum 21  # 10-20%
            $additionalPartialAmount = [Math]::Round($randomPartialInvoice.AmountOutstanding * $additionalPartialPercentage / 100, 2)
            
            $paymentData = @{
                InsurerId = $randomPartialInvoice.InsurerId
                PaymentAmount = $additionalPartialAmount
                PaymentReference = "ADDITIONAL-PARTIAL-PAY-$(Get-Random -Maximum 9999)"
            }
            
            $data = $paymentData | ConvertTo-Json -Compress
            Invoke-CurlRequest -Url "$AccountingBaseUrl/Invoice/$($randomPartialInvoice.Id)/payment" -Data $data -Method "POST"
        }
        
        Write-Host "Payment processing completed!" -ForegroundColor Green
        Write-Host "  - Full payments: $($invoicesForFullPayment.Count)" -ForegroundColor Cyan
        Write-Host "  - Partial payments: $($invoicesForPartialPayment.Count)" -ForegroundColor Cyan
    }
    else {
        Write-Host " No invoices found to process payments" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "Error processing invoice payments: $($_.Exception.Message)" -ForegroundColor Red
}

# Conclude
Write-Host "Braphia System Seeding Completed!" -ForegroundColor Green
Write-Host "You can now start using the system with seeded data." -ForegroundColor Cyan
Write-Host "Check the logs for any errors or issues during seeding." -ForegroundColor Yellow