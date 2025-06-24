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

//var processor = builder
//    .AddProject<Projects.InsuranceDetails_Processor>("processor")
//    .WithReplicas(5)
//    .WithReference(apiDatabase)
//    .WaitFor(apiDatabase)
//    .WithReference(messages)
//    .WaitFor(messages);

builder.Build().Run();
