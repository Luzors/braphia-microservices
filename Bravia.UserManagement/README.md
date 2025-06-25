# Braphia.UserManagement

## Purpose
This Microservice is responsible for managing users in the system. It provides functionality to create, update, delete, and retrieve user information.

## Fired-Events
| Event Name | Description |
|------------|-------------|

## Consumed-Events
| Event Name | Description |
|------------|-------------|

## Enterprise-Integration Pattern
This microservices uses the external User "database" on Azure (csv file) as an input for the users in the system. The file is read and parsed on startup and the users are stored in a local database. The users are then used to authenticate the users in the system.
It synchronizes daily at 2am using a BackgroundService.
