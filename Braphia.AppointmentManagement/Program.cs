using Braphia.AppointmentManagement.Commands.AddAppointment;
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

var mongoConnectionString = builder.Configuration.GetConnectionString("AppointmentReadDB")
    ?? throw new InvalidOperationException("No connection string configured for AppointmentReadDB");

var connectionString = builder.Configuration.GetConnectionString("AppointmentWriteDB") ??
                       throw new InvalidOperationException("No connection string configured");

builder.Services
    .AddDbContext<DBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton<ReadDbContext>(provider =>
    new ReadDbContext(mongoConnectionString, "AppointmentManagement"));

// Repositories
builder.Services.AddScoped<IAppointmentRepository, SqlAppointmentRepository>();
builder.Services.AddScoped<AppointmentReadRepository>();
builder.Services.AddScoped<IMongoAppointmentReadRepository, AppointmentReadRepository>();

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AppointmentCreatedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
