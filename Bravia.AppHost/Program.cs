using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.env.dev.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var builder = DistributedApplication.CreateBuilder(args);

// -------- Databases -------------------
int rabbitMqPort = config.GetValue<int>("RabbitMQPort", 5672);

var rabbitMq = builder.AddRabbitMQ("eventbus", port: rabbitMqPort)
    .WithManagementPlugin(port: 15672)
    .WithDataVolume("braphia-rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent);

var userSqlServer = builder
    .AddSqlServer("sql-server-usermanagement", port: 2015)
    .WithDataVolume("braphia-usermanagement")
    .WithLifetime(ContainerLifetime.Persistent);

var userDatabase = userSqlServer
    .AddDatabase("UserDB");

var medicalAnalysisDbServer = builder
    .AddSqlServer("sql-server-medicalAnalysis", port: 2016)
    .WithDataVolume("braphia-medicalAnalysis")
    .WithLifetime(ContainerLifetime.Persistent);

var medicalDatabase = medicalAnalysisDbServer
    .AddDatabase("MedicalDB");

var accountingDbServer = builder
    .AddSqlServer("sql-server-accounting", port: 2017)
    .WithDataVolume("braphia-accounting")
    .WithLifetime(ContainerLifetime.Persistent);

var accountingDatabase = accountingDbServer
    .AddDatabase("AccountingDB");

var pharmacyDbServer = builder
    .AddSqlServer("sql-server-pharmacy", port: 2018)
    .WithDataVolume("braphia-pharmacy")
    .WithLifetime(ContainerLifetime.Persistent);

var pharmacyDatabase = pharmacyDbServer
    .AddDatabase("PharmacyDB");

var laboratoryDbServer = builder
    .AddSqlServer("sql-server-laboratory", port: 2019)
    .WithDataVolume("braphia-laboratory")
    .WithLifetime(ContainerLifetime.Persistent);

var laboratoryDatabase = laboratoryDbServer
    .AddDatabase("LaboratoryDb");

var notificationDbServer = builder
    .AddSqlServer("sql-server-notification", port: 2020)
    .WithDataVolume("braphia-notification")
    .WithLifetime(ContainerLifetime.Persistent);

var notificationDatabase = notificationDbServer
    .AddDatabase("NotificationDb");

var appointmentWriteDb = builder
 .AddSqlServer("appointment-mysql-write", port: 2021)
 .WithDataVolume("braphia-appointment-write")
 .WithLifetime(ContainerLifetime.Persistent)
 .AddDatabase("AppointmentWriteDB");

var appointmentReadDb = builder
 .AddSqlServer("appointment-mysql-read", port: 2022)
  .WithDataVolume("braphia-appointment-read")
 .WithLifetime(ContainerLifetime.Persistent)
 .AddDatabase("AppointmentReadDB");

// -----------Managers-------------------
var userManagement = builder
    .AddProject<Projects.Braphia_UserManagement>("userManagement")
    .WithReference(userDatabase)
    .WaitFor(userDatabase)
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

var medicalManagement = builder
    .AddProject<Projects.Braphia_MedicalManagement>("medicalManagement")
    .WithReference(medicalDatabase)
    .WaitFor(medicalDatabase)
    .WithReference(rabbitMq)
        .WaitFor(rabbitMq);

var accounting = builder
    .AddProject<Projects.Braphia_Accounting>("accounting")
    .WithReference(accountingDatabase)
    .WaitFor(accountingDatabase)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

var pharmacy = builder
    .AddProject<Projects.Braphia_Pharmacy>("pharmacy")
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

var notification = builder
    .AddProject<Projects.Braphia_NotificationDispatcher>("notification")
    .WithReference(notificationDatabase)
    .WaitFor(notificationDatabase)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

builder.Build().Run();
