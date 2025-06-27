var builder = DistributedApplication.CreateBuilder(args);

// -------- Databases -------------------
var sqlServer = builder
    .AddSqlServer("sql-server-usermanagement", port: 2015)
    .WithDataVolume("braphia-usermanagement")
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitMq = builder.AddRabbitMQ("eventbus", port: 5672)
    .WithManagementPlugin(port: 15672)
    .WithDataVolume("braphia-rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent);

var apiDatabase = sqlServer
    .AddDatabase("UserDB");

var laboratoryDatabase = sqlServer
    .AddDatabase("LaboratoryDb");

//Appointment Management db
var appointmentWriteDb = builder
 .AddSqlServer("appointment-mysql", port: 3306)
 .AddDatabase("AppointmentWriteDB");

var appointmentReadDb = builder
    .AddMongoDB("appointment-mongodb", port: 27017)
    .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "myuser")
    .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "mypassword")
    .AddDatabase("AppointmentReadDB");


// -----------Mangers-------------------

var userManagement = builder
    .AddProject<Projects.Braphia_UserManagement>("userManagement")
    .WithReference(apiDatabase)
    .WaitFor(apiDatabase)
    .WithReference(rabbitMq)
        .WaitFor(rabbitMq);

var appointmentManagement = builder
    .AddProject<Projects.Braphia_AppointmentManagement>("appointmentManagement")
    .WithReference(appointmentWriteDb)
    .WaitFor(appointmentWriteDb)
    .WithReference(appointmentReadDb)
    .WaitFor(appointmentReadDb)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

var accountingDbServer = builder
    .AddSqlServer("sql-server-accounting", port: 2016)
    .WithDataVolume("braphia-accounting")
    .WithLifetime(ContainerLifetime.Persistent);

var accountingDatabase = accountingDbServer
    .AddDatabase("AccountingDB");

var accounting = builder
    .AddProject<Projects.Braphia_Accounting>("accounting")
    .WithReference(accountingDatabase)
    .WaitFor(accountingDatabase)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);


var pharmacyDbServer = builder
    .AddSqlServer("sql-server-pharmacy", port: 2017)
    .WithDataVolume("braphia-pharmacy")
    .WithLifetime(ContainerLifetime.Persistent);

var pharmacyDatabase = pharmacyDbServer
    .AddDatabase("PharmacyDB");

var pharmacy = builder
    .AddProject<Projects.Braphia_Pharmacy>("braphia-pharmacy")
    .WithReference(pharmacyDatabase)
    .WaitFor(pharmacyDatabase)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

var laboratory = builder
    .AddProject<Projects.Braphia_Laboratory>("laboratory")
    .WithReference(laboratoryDatabase)
    .WaitFor(laboratoryDatabase)
    .WithReference(rabbitMq)
        .WaitFor(rabbitMq);

builder.Build().Run();
