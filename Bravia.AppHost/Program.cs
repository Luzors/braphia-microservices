using Microsoft.Extensions.Configuration;
using Aspire.Hosting; // If needed for DistributedApplication

// 1. Build configuration first
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) // Ensures correct path
    .AddJsonFile("appsettings.env.dev.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// 2. Now create the builder
var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder
    .AddSqlServer("sql-server", port: 2015)
    .WithDataVolume("braphia")
    .WithLifetime(ContainerLifetime.Persistent);

// 3. Read RabbitMQ port from config (with a default value)
int rabbitMqPort = config.GetValue<int>("RabbitMQPort", 5672);

var rabbitMq = builder.AddRabbitMQ("eventbus", port: rabbitMqPort)
    .WithManagementPlugin(port: 15672)
    .WithDataVolume("braphia-rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent);

var apiDatabase = sqlServer
    .AddDatabase("UserDB");

var userManagement = builder
    .AddProject<Projects.Braphia_UserManagement>("userManagement")
    .WithReference(apiDatabase)
    .WaitFor(apiDatabase)
    .WithReference(rabbitMq)
        .WaitFor(rabbitMq);

var appointmentManagement = builder
    .AddProject<Projects.Braphia_AppointmentManagement>("appointmentManagement")
/*    .WithReference(apiDatabase)
    .WaitFor(apiDatabase)*/
    .WithReference(rabbitMq)
        .WaitFor(rabbitMq);

var medicalAnalysisDbServer = builder
    .AddSqlServer("sql-server-medicalAnalysis", port: 2017)
    .WithDataVolume("braphia-medicalAnalysis")
    .WithLifetime(ContainerLifetime.Persistent);
    
var medicalDatabase = medicalAnalysisDbServer
    .AddDatabase("MedicalDB");

var medicalManagement = builder
    .AddProject<Projects.Braphia_MedicalManagement>("medicalManagement")
    .WithReference(medicalDatabase)
    .WaitFor(medicalDatabase)
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


//var processor = builder
//    .AddProject<Projects.InsuranceDetails_Processor>("processor")
//    .WithReplicas(5)
//    .WithReference(apiDatabase)
//    .WaitFor(apiDatabase)
//    .WithReference(messages)
//    .WaitFor(messages);

builder.Build().Run();
