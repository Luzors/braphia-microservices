var builder = DistributedApplication.CreateBuilder(args);

//builder.AddProject<Projects.Braphia_UserManagement>("braphia-usermanagement");

var sqlServer = builder
    .AddSqlServer("sql-server", port: 2015)
    .WithDataVolume("braphia")
    .WithLifetime(ContainerLifetime.Persistent);

//var messages = sqlServer
//    .AddDatabase("MessagesDb");

var apiDatabase = sqlServer
    .AddDatabase("UserDB");

var userManagement = builder
    .AddProject<Projects.Braphia_UserManagement>("userManagement")
    .WithReference(apiDatabase)
    .WaitFor(apiDatabase);
    //.WithReference(messages)
    //.WaitFor(messages);

//var identityDatabase = sqlServer
//    .AddDatabase("IdentityDb");

//var idp = builder
//    .AddProject<Projects.InsuranceDetails_Api_IdentityProvider>("idp")
//    .WithReference(identityDatabase)
//    .WaitFor(identityDatabase)
//    .WithReference(api)
//    .WaitFor(api);

//var processor = builder
//    .AddProject<Projects.InsuranceDetails_Processor>("processor")
//    .WithReplicas(5)
//    .WithReference(apiDatabase)
//    .WaitFor(apiDatabase)
//    .WithReference(messages)
//    .WaitFor(messages);

builder.Build().Run();
