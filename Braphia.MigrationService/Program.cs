using Braphia.MigrationService;
using Braphia.UserManagement.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

// Seeding
builder.AddSqlServerDbContext<DBContext>("UserDB");

var host = builder.Build();
host.Run();
