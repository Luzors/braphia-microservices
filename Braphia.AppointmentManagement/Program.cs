using Braphia.AppointmentManagement.Commands.AddAppointment;
using Braphia.AppointmentManagement.Commands.AppointmentRescheduled;
using Braphia.AppointmentManagement.Consumers;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories;
using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AppointmentCreatedCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AppointmentRescheduledCommandHandler).Assembly));


var connectionStringRead = builder.Configuration.GetConnectionString("AppointmentReadDB")
    ?? throw new InvalidOperationException("No connection string configured for AppointmentReadDB");

var connectionStringWrite = builder.Configuration.GetConnectionString("AppointmentWriteDB") ??
                       throw new InvalidOperationException("No connection string configured");

builder.Services
    .AddDbContext<DBContext>(options => options.UseSqlServer(connectionStringWrite));

builder.Services
    .AddDbContext<ReadDbContext>(options => options.UseSqlServer(connectionStringRead));


// Repositories
builder.Services.AddScoped<SQLAppointmentReadRepository>();
builder.Services.AddScoped<IAppointmentReadRepository, SQLAppointmentReadRepository>();
builder.Services.AddScoped<IAppointmentRepository, SqlAppointmentRepository>();
builder.Services.AddScoped<IPatientRepository, SqlPatientRepository>();
builder.Services.AddScoped<IPhysicianRepository, SqlPhysicianRepository>();
builder.Services.AddScoped<IReceptionistRepository, SQLReceptionistRepository>();
builder.Services.AddScoped<IReferralRepository, SqlReferralRepository>();

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AppointmentCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var rabbitMqConnection = configuration.GetConnectionString("eventbus");
        cfg.Host(rabbitMqConnection);

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DBContext>();
    await db.Database.MigrateAsync();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
