# Medical Analysis API Usage

## Creating a new Medical Analysis

Now you can create a medical analysis by sending just the IDs instead of full objects:

### POST Request Example

```json
POST /MedicalAnalysis
Content-Type: application/json

{
    "patientId": 123,
    "physicianId": 456,
    "description": "Patient shows signs of improvement after treatment",
    "appointmentId": 789
}
```

### Example without appointment:

```json
POST /MedicalAnalysis
Content-Type: application/json

{
    "patientId": 123,
    "physicianId": 456,
    "description": "Routine checkup - patient in good health"
}
```

### Response:

```json
HTTP 201 Created
Location: /MedicalAnalysis/1

{
    "id": 1,
    "patientId": 123,
    "physicianId": 456,
    "description": "Patient shows signs of improvement after treatment",
    "appointmentId": 789,
    "patient": null,
    "physician": null,
    "appointment": null,
    "prescriptions": []
}
```

## Changes Made:

1. **Navigation Properties**: Made `Patient`, `Physician`, and `Appointment` navigation properties nullable
2. **DTO Created**: Added `CreateMedicalAnalysisDto` for clean API input
3. **Controller Updated**: Modified POST endpoint to accept DTO instead of full entity
4. **Validation**: Added proper validation for required fields

You no longer need to provide the full Patient, Physician, or Appointment objects when creating a medical analysis!
