var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder
    .AddSqlServer("sql-server", port: 2015)
    .WithDataVolume("braphia")
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitMq = builder.AddRabbitMQ("eventbus", port: 5672)
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

var laboratoryDbServer = builder
    .AddSqlServer("sql-server-laboratory", port: 2017)
    .WithDataVolume("braphia-laboratory")
    .WithLifetime(ContainerLifetime.Persistent);

var laboratoryDatabase = laboratoryDbServer
    .AddDatabase("LaboratoryDb");

var laboratory = builder
    .AddProject<Projects.Braphia_Laboratory>("laboratory")
    .WithReference(laboratoryDatabase)
    .WaitFor(laboratoryDatabase)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

//var processor = builder
//    .AddProject<Projects.InsuranceDetails_Processor>("processor")
//    .WithReplicas(5)
//    .WithReference(apiDatabase)
//    .WaitFor(apiDatabase)
//    .WithReference(messages)
//    .WaitFor(messages);

//builder.AddProject<Projects.Braphia_Laboratory>("braphia-laboratory");

//var processor = builder
//    .AddProject<Projects.InsuranceDetails_Processor>("processor")
//    .WithReplicas(5)
//    .WithReference(apiDatabase)
//    .WaitFor(apiDatabase)
//    .WithReference(messages)
//    .WaitFor(messages);

builder.Build().Run();
