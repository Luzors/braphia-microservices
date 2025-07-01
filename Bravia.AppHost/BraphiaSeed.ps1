# This powershell script runs a shitton of curl requests to seed the system
# Might be some AI inspired stuff :)

Write-Host "🌱 Starting Braphia System Seeding..." -ForegroundColor Green

# Base URLs for microservices (adjust ports as needed)
$UserManagementBaseUrl = "https://localhost:7001"
$MedicalManagementBaseUrl = "https://localhost:7002"
$PharmacyBaseUrl = "https://localhost:7003"
$LaboratoryBaseUrl = "https://localhost:7004"
$AccountingBaseUrl = "https://localhost:7005"

# Function to make curl requests with error handling
function Invoke-CurlRequest {
    param(
        [string]$Url,
        [string]$Method = "POST",
        [string]$Data = "",
        [string]$ContentType = "application/json"
    )
    
    try {
        if ($Method -eq "POST" -and $Data -ne "") {
            $result = curl -X $Method $Url -H "Content-Type: $ContentType" -d $Data --insecure --silent
        } else {
            $result = curl -X $Method $Url --insecure --silent
        }
        Write-Host "✅ Success: $Method $Url" -ForegroundColor Green
        return $result
    }
    catch {
        Write-Host "❌ Failed: $Method $Url - $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Create Insurers first (needed for patients)
Write-Host "`n🏢 Creating Insurers..." -ForegroundColor Yellow

$insurers = @(
    @{ Name = "HealthCare Plus"; ContactEmail = "contact@healthcareplus.com"; PhoneNumber = "+1-555-0101" },
    @{ Name = "MediCare Insurance"; ContactEmail = "info@medicare.com"; PhoneNumber = "+1-555-0102" },
    @{ Name = "VitalCare"; ContactEmail = "support@vitalcare.com"; PhoneNumber = "+1-555-0103" },
    @{ Name = "SecureHealth"; ContactEmail = "claims@securehealth.com"; PhoneNumber = "+1-555-0104" },
    @{ Name = "PremiumCare"; ContactEmail = "service@premiumcare.com"; PhoneNumber = "+1-555-0105" }
)

foreach ($insurer in $insurers) {
    $data = $insurer | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$AccountingBaseUrl/api/Insurer" -Data $data
}

# Create General Practitioners
Write-Host "`n👨‍⚕️ Creating General Practitioners..." -ForegroundColor Yellow

$gps = @(
    @{ FirstName = "John"; LastName = "Smith"; Email = "j.smith@clinic.com"; PhoneNumber = "+1-555-1001"; Specialization = "Family Medicine" },
    @{ FirstName = "Sarah"; LastName = "Johnson"; Email = "s.johnson@clinic.com"; PhoneNumber = "+1-555-1002"; Specialization = "Internal Medicine" },
    @{ FirstName = "Michael"; LastName = "Brown"; Email = "m.brown@clinic.com"; PhoneNumber = "+1-555-1003"; Specialization = "General Practice" },
    @{ FirstName = "Emily"; LastName = "Davis"; Email = "e.davis@clinic.com"; PhoneNumber = "+1-555-1004"; Specialization = "Family Medicine" },
    @{ FirstName = "David"; LastName = "Wilson"; Email = "d.wilson@clinic.com"; PhoneNumber = "+1-555-1005"; Specialization = "Preventive Medicine" }
)

foreach ($gp in $gps) {
    $data = $gp | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$UserManagementBaseUrl/api/GeneralPracticioner" -Data $data
}

# Create Physicians
Write-Host "`n👩‍⚕️ Creating Physicians..." -ForegroundColor Yellow

$physicians = @(
    @{ FirstName = "Robert"; LastName = "Anderson"; Email = "r.anderson@hospital.com"; PhoneNumber = "+1-555-2001"; Specialization = "Cardiology" },
    @{ FirstName = "Lisa"; LastName = "Martinez"; Email = "l.martinez@hospital.com"; PhoneNumber = "+1-555-2002"; Specialization = "Dermatology" },
    @{ FirstName = "James"; LastName = "Taylor"; Email = "j.taylor@hospital.com"; PhoneNumber = "+1-555-2003"; Specialization = "Orthopedics" },
    @{ FirstName = "Maria"; LastName = "Garcia"; Email = "m.garcia@hospital.com"; PhoneNumber = "+1-555-2004"; Specialization = "Neurology" },
    @{ FirstName = "Christopher"; LastName = "Lee"; Email = "c.lee@hospital.com"; PhoneNumber = "+1-555-2005"; Specialization = "Gastroenterology" },
    @{ FirstName = "Amanda"; LastName = "White"; Email = "a.white@hospital.com"; PhoneNumber = "+1-555-2006"; Specialization = "Endocrinology" },
    @{ FirstName = "Kevin"; LastName = "Thompson"; Email = "k.thompson@hospital.com"; PhoneNumber = "+1-555-2007"; Specialization = "Pulmonology" },
    @{ FirstName = "Jessica"; LastName = "Miller"; Email = "j.miller@hospital.com"; PhoneNumber = "+1-555-2008"; Specialization = "Oncology" }
)

foreach ($physician in $physicians) {
    $data = $physician | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$UserManagementBaseUrl/api/Physician" -Data $data
}

# Create Patients
Write-Host "`n👤 Creating Patients..." -ForegroundColor Yellow

$patients = @(
    @{ FirstName = "Alice"; LastName = "Cooper"; Email = "alice.cooper@email.com"; PhoneNumber = "+1-555-3001"; BirthDate = "1985-03-15T00:00:00Z" },
    @{ FirstName = "Bob"; LastName = "Dylan"; Email = "bob.dylan@email.com"; PhoneNumber = "+1-555-3002"; BirthDate = "1978-07-22T00:00:00Z" },
    @{ FirstName = "Carol"; LastName = "King"; Email = "carol.king@email.com"; PhoneNumber = "+1-555-3003"; BirthDate = "1992-11-08T00:00:00Z" },
    @{ FirstName = "Daniel"; LastName = "Craig"; Email = "daniel.craig@email.com"; PhoneNumber = "+1-555-3004"; BirthDate = "1968-04-12T00:00:00Z" },
    @{ FirstName = "Emma"; LastName = "Stone"; Email = "emma.stone@email.com"; PhoneNumber = "+1-555-3005"; BirthDate = "1990-09-25T00:00:00Z" },
    @{ FirstName = "Frank"; LastName = "Sinatra"; Email = "frank.sinatra@email.com"; PhoneNumber = "+1-555-3006"; BirthDate = "1955-12-12T00:00:00Z" },
    @{ FirstName = "Grace"; LastName = "Kelly"; Email = "grace.kelly@email.com"; PhoneNumber = "+1-555-3007"; BirthDate = "1988-02-28T00:00:00Z" },
    @{ FirstName = "Henry"; LastName = "Ford"; Email = "henry.ford@email.com"; PhoneNumber = "+1-555-3008"; BirthDate = "1975-06-18T00:00:00Z" },
    @{ FirstName = "Ivy"; LastName = "League"; Email = "ivy.league@email.com"; PhoneNumber = "+1-555-3009"; BirthDate = "1995-01-03T00:00:00Z" },
    @{ FirstName = "Jack"; LastName = "Sparrow"; Email = "jack.sparrow@email.com"; PhoneNumber = "+1-555-3010"; BirthDate = "1982-08-14T00:00:00Z" }
)

foreach ($patient in $patients) {
    $data = $patient | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$UserManagementBaseUrl/api/Patient" -Data $data
}

# Create Pharmacies
Write-Host "`n💊 Creating Pharmacies..." -ForegroundColor Yellow

$pharmacies = @(
    @{ Name = "CityPharm Downtown"; Address = "123 Main St, Downtown"; PhoneNumber = "+1-555-4001"; Email = "downtown@citypharm.com" },
    @{ Name = "HealthMart Plaza"; Address = "456 Oak Ave, Midtown"; PhoneNumber = "+1-555-4002"; Email = "plaza@healthmart.com" },
    @{ Name = "MediCenter Pharmacy"; Address = "789 Pine Rd, Uptown"; PhoneNumber = "+1-555-4003"; Email = "uptown@medicenter.com" },
    @{ Name = "QuickCare Drugs"; Address = "321 Elm St, Westside"; PhoneNumber = "+1-555-4004"; Email = "westside@quickcare.com" },
    @{ Name = "WellnessRx"; Address = "654 Maple Dr, Eastside"; PhoneNumber = "+1-555-4005"; Email = "eastside@wellnessrx.com" }
)

foreach ($pharmacy in $pharmacies) {
    $data = $pharmacy | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$PharmacyBaseUrl/api/Pharmacy" -Data $data
}

# Create Medications
Write-Host "`n💉 Creating Medications..." -ForegroundColor Yellow

$medications = @(
    @{ Name = "Aspirin"; Dosage = "325mg"; Manufacturer = "PharmaCorp"; Price = 12.99; Description = "Pain reliever and anti-inflammatory" },
    @{ Name = "Ibuprofen"; Dosage = "200mg"; Manufacturer = "MediLab"; Price = 8.49; Description = "Nonsteroidal anti-inflammatory drug" },
    @{ Name = "Acetaminophen"; Dosage = "500mg"; Manufacturer = "HealthGen"; Price = 6.99; Description = "Pain reliever and fever reducer" },
    @{ Name = "Amoxicillin"; Dosage = "250mg"; Manufacturer = "BioPharm"; Price = 15.75; Description = "Antibiotic for bacterial infections" },
    @{ Name = "Lisinopril"; Dosage = "10mg"; Manufacturer = "CardioMed"; Price = 22.50; Description = "ACE inhibitor for high blood pressure" },
    @{ Name = "Metformin"; Dosage = "500mg"; Manufacturer = "DiabetesCare"; Price = 18.25; Description = "Diabetes medication" },
    @{ Name = "Atorvastatin"; Dosage = "20mg"; Manufacturer = "CholesterolRx"; Price = 28.99; Description = "Cholesterol lowering medication" },
    @{ Name = "Omeprazole"; Dosage = "20mg"; Manufacturer = "GastroMed"; Price = 16.75; Description = "Proton pump inhibitor" }
)

foreach ($medication in $medications) {
    $data = $medication | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$PharmacyBaseUrl/api/Medication" -Data $data
}

# Create Laboratory Appointments
Write-Host "`n🧪 Creating Laboratory Appointments..." -ForegroundColor Yellow

$labAppointments = @(
    @{ PatientId = 1; AppointmentDate = "2024-01-15T09:00:00Z"; Purpose = "Blood work"; Status = "Scheduled" },
    @{ PatientId = 2; AppointmentDate = "2024-01-16T10:30:00Z"; Purpose = "Urine analysis"; Status = "Scheduled" },
    @{ PatientId = 3; AppointmentDate = "2024-01-17T14:00:00Z"; Purpose = "X-ray chest"; Status = "Scheduled" },
    @{ PatientId = 4; AppointmentDate = "2024-01-18T11:15:00Z"; Purpose = "MRI scan"; Status = "Scheduled" },
    @{ PatientId = 5; AppointmentDate = "2024-01-19T08:45:00Z"; Purpose = "CT scan"; Status = "Scheduled" },
    @{ PatientId = 6; AppointmentDate = "2024-01-20T13:30:00Z"; Purpose = "Blood glucose test"; Status = "Scheduled" },
    @{ PatientId = 7; AppointmentDate = "2024-01-21T15:00:00Z"; Purpose = "Cholesterol panel"; Status = "Scheduled" },
    @{ PatientId = 8; AppointmentDate = "2024-01-22T09:30:00Z"; Purpose = "Liver function test"; Status = "Scheduled" }
)

foreach ($appointment in $labAppointments) {
    $data = $appointment | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$LaboratoryBaseUrl/api/Appointment" -Data $data
}

# Create Medical Prescriptions
Write-Host "`n📋 Creating Medical Prescriptions..." -ForegroundColor Yellow

$prescriptions = @(
    @{ PatientId = 1; PhysicianId = 1; Medicine = "Aspirin"; Dosage = "325mg twice daily"; Instructions = "Take with food"; DatePrescribed = "2024-01-10T00:00:00Z" },
    @{ PatientId = 2; PhysicianId = 2; Medicine = "Ibuprofen"; Dosage = "200mg as needed"; Instructions = "For pain relief"; DatePrescribed = "2024-01-11T00:00:00Z" },
    @{ PatientId = 3; PhysicianId = 3; Medicine = "Amoxicillin"; Dosage = "250mg three times daily"; Instructions = "Complete full course"; DatePrescribed = "2024-01-12T00:00:00Z" },
    @{ PatientId = 4; PhysicianId = 4; Medicine = "Lisinopril"; Dosage = "10mg once daily"; Instructions = "Take in morning"; DatePrescribed = "2024-01-13T00:00:00Z" },
    @{ PatientId = 5; PhysicianId = 5; Medicine = "Metformin"; Dosage = "500mg twice daily"; Instructions = "Take with meals"; DatePrescribed = "2024-01-14T00:00:00Z" },
    @{ PatientId = 6; PhysicianId = 6; Medicine = "Atorvastatin"; Dosage = "20mg once daily"; Instructions = "Take at bedtime"; DatePrescribed = "2024-01-15T00:00:00Z" }
)

foreach ($prescription in $prescriptions) {
    $data = $prescription | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$MedicalManagementBaseUrl/Prescription" -Data $data
}

# Create Laboratory Tests
Write-Host "`n🔬 Creating Laboratory Tests..." -ForegroundColor Yellow

$labTests = @(
    @{ PatientId = 1; TestType = "Complete Blood Count"; Description = "CBC with differential"; OrderedDate = "2024-01-15T00:00:00Z"; Status = "Ordered" },
    @{ PatientId = 2; TestType = "Urinalysis"; Description = "Complete urine analysis"; OrderedDate = "2024-01-16T00:00:00Z"; Status = "Ordered" },
    @{ PatientId = 3; TestType = "Chest X-Ray"; Description = "PA and lateral chest X-ray"; OrderedDate = "2024-01-17T00:00:00Z"; Status = "Ordered" },
    @{ PatientId = 4; TestType = "Basic Metabolic Panel"; Description = "Glucose, electrolytes, kidney function"; OrderedDate = "2024-01-18T00:00:00Z"; Status = "Ordered" },
    @{ PatientId = 5; TestType = "Lipid Panel"; Description = "Total cholesterol, HDL, LDL, triglycerides"; OrderedDate = "2024-01-19T00:00:00Z"; Status = "Ordered" },
    @{ PatientId = 6; TestType = "Liver Function Test"; Description = "ALT, AST, bilirubin, alkaline phosphatase"; OrderedDate = "2024-01-20T00:00:00Z"; Status = "Ordered" },
    @{ PatientId = 7; TestType = "Thyroid Function"; Description = "TSH, T3, T4"; OrderedDate = "2024-01-21T00:00:00Z"; Status = "Ordered" },
    @{ PatientId = 8; TestType = "Hemoglobin A1C"; Description = "3-month average blood sugar"; OrderedDate = "2024-01-22T00:00:00Z"; Status = "Ordered" }
)

foreach ($test in $labTests) {
    $data = $test | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$MedicalManagementBaseUrl/Test" -Data $data
}

# Create Medication Orders
Write-Host "`n💊 Creating Medication Orders..." -ForegroundColor Yellow

$medicationOrders = @(
    @{ PatientId = 1; PharmacyId = 1; OrderDate = "2024-01-10T00:00:00Z"; Status = "Pending"; TotalAmount = 12.99 },
    @{ PatientId = 2; PharmacyId = 2; OrderDate = "2024-01-11T00:00:00Z"; Status = "Pending"; TotalAmount = 8.49 },
    @{ PatientId = 3; PharmacyId = 3; OrderDate = "2024-01-12T00:00:00Z"; Status = "Pending"; TotalAmount = 15.75 },
    @{ PatientId = 4; PharmacyId = 4; OrderDate = "2024-01-13T00:00:00Z"; Status = "Pending"; TotalAmount = 22.50 },
    @{ PatientId = 5; PharmacyId = 5; OrderDate = "2024-01-14T00:00:00Z"; Status = "Pending"; TotalAmount = 18.25 }
)

foreach ($order in $medicationOrders) {
    $data = $order | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$PharmacyBaseUrl/api/MedicationOrder" -Data $data
}

# Assign patients to insurers
Write-Host "`n🏥 Assigning Patients to Insurers..." -ForegroundColor Yellow

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
    Invoke-CurlRequest -Url "$AccountingBaseUrl/api/Patient/$($assignment.PatientId)/assign-insurer/$($assignment.InsurerId)" -Method "PUT"
}

# Create Invoices
Write-Host "`n💰 Creating Invoices..." -ForegroundColor Yellow

$invoices = @(
    @{ PatientId = 1; InsurerId = 1; Amount = 150.00; Description = "Consultation and blood work"; DateCreated = "2024-01-15T00:00:00Z"; Status = "Pending" },
    @{ PatientId = 2; InsurerId = 2; Amount = 95.50; Description = "Urine analysis and consultation"; DateCreated = "2024-01-16T00:00:00Z"; Status = "Pending" },
    @{ PatientId = 3; InsurerId = 3; Amount = 275.75; Description = "X-ray and antibiotic prescription"; DateCreated = "2024-01-17T00:00:00Z"; Status = "Pending" },
    @{ PatientId = 4; InsurerId = 4; Amount = 180.25; Description = "MRI scan consultation"; DateCreated = "2024-01-18T00:00:00Z"; Status = "Pending" },
    @{ PatientId = 5; InsurerId = 5; Amount = 320.00; Description = "CT scan and diabetes medication"; DateCreated = "2024-01-19T00:00:00Z"; Status = "Pending" },
    @{ PatientId = 6; InsurerId = 1; Amount = 125.75; Description = "Blood glucose test and consultation"; DateCreated = "2024-01-20T00:00:00Z"; Status = "Pending" },
    @{ PatientId = 7; InsurerId = 2; Amount = 145.50; Description = "Cholesterol panel and medication"; DateCreated = "2024-01-21T00:00:00Z"; Status = "Pending" },
    @{ PatientId = 8; InsurerId = 3; Amount = 165.25; Description = "Liver function test consultation"; DateCreated = "2024-01-22T00:00:00Z"; Status = "Pending" }
)

foreach ($invoice in $invoices) {
    $data = $invoice | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$AccountingBaseUrl/api/Invoice" -Data $data
}

# Complete some tests to trigger notifications
Write-Host "`n✅ Completing Some Laboratory Tests..." -ForegroundColor Yellow

$testCompletions = @(
    @{ TestId = 1; Result = "Normal CBC values. WBC: 7.2, RBC: 4.5, Hemoglobin: 14.2" },
    @{ TestId = 2; Result = "Normal urinalysis. No protein, glucose, or bacteria detected" },
    @{ TestId = 3; Result = "Clear chest X-ray. No abnormalities detected" },
    @{ TestId = 4; Result = "Glucose: 95 mg/dL (normal), Creatinine: 1.0 mg/dL (normal)" },
    @{ TestId = 5; Result = "Total cholesterol: 185 mg/dL, HDL: 55 mg/dL, LDL: 110 mg/dL" }
)

foreach ($completion in $testCompletions) {
    $data = @{ Result = $completion.Result } | ConvertTo-Json -Compress
    Invoke-CurlRequest -Url "$LaboratoryBaseUrl/api/Test/$($completion.TestId)/Complete" -Method "PUT" -Data $data
}

# Complete some medication orders
Write-Host "`n💊 Completing Some Medication Orders..." -ForegroundColor Yellow

$medicationOrdersToComplete = @(1, 2, 3)

foreach ($orderId in $medicationOrdersToComplete) {
    Invoke-CurlRequest -Url "$PharmacyBaseUrl/api/MedicationOrder/$orderId/complete" -Method "PUT"
}

# Add medications to orders
Write-Host "`n📦 Adding Medications to Orders..." -ForegroundColor Yellow

$medicationAdditions = @(
    @{ OrderId = 1; MedicationId = 1; Amount = 1 },
    @{ OrderId = 2; MedicationId = 2; Amount = 1 },
    @{ OrderId = 3; MedicationId = 4; Amount = 1 },
    @{ OrderId = 4; MedicationId = 5; Amount = 1 },
    @{ OrderId = 5; MedicationId = 6; Amount = 1 }
)

foreach ($addition in $medicationAdditions) {
    Invoke-CurlRequest -Url "$PharmacyBaseUrl/api/MedicationOrder/$($addition.OrderId)/medication?medicationId=$($addition.MedicationId)&amount=$($addition.Amount)" -Method "POST"
}

Write-Host "`n🎉 Seeding Complete!" -ForegroundColor Green
Write-Host "Created:" -ForegroundColor Cyan
Write-Host "  - 5 Insurers" -ForegroundColor White
Write-Host "  - 5 General Practitioners" -ForegroundColor White
Write-Host "  - 8 Physicians" -ForegroundColor White
Write-Host "  - 10 Patients" -ForegroundColor White
Write-Host "  - 5 Pharmacies" -ForegroundColor White
Write-Host "  - 8 Medications" -ForegroundColor White
Write-Host "  - 8 Laboratory Appointments" -ForegroundColor White
Write-Host "  - 6 Prescriptions" -ForegroundColor White
Write-Host "  - 8 Laboratory Tests" -ForegroundColor White
Write-Host "  - 5 Medication Orders" -ForegroundColor White
Write-Host "  - 8 Invoices" -ForegroundColor White
Write-Host "  - Completed 5 tests and 3 medication orders" -ForegroundColor White
Write-Host "`n💡 Note: Adjust the base URLs and ports to match your actual microservice endpoints." -ForegroundColor Yellow