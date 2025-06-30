# NotificationService
Dispatches all notifications to the right users/pharmacies/labs via email
*Sort of, just implemented console logs for now :)*

# Consumed Events
| Event Name | Description |
|------------|-------------|
| All register/modify/delete events for Patient, lab and pharmacy | keep dbs in sync |
| MedicationOrderCompleted from pharmacy | notify patient that order is ready for pickup |