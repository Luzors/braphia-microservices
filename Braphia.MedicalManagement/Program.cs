using Microsoft.EntityFrameworkCore;
using Braphia.MedicalManagement.Database;
using Braphia.MedicalManagement.Repositories;
using MassTransit;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Braphia.MedicalManagement.Consumers;
using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MedicalDB") ??
                       throw new InvalidOperationException("No connection string configured");

builder.Services
    .AddDbContext<DBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PatientCreatedConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var rabbitMqConnection = configuration.GetConnectionString("eventbus");
        cfg.Host(rabbitMqConnection);

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IPrescriptionRepository, SqlPrescriptionRepository>();
builder.Services.AddScoped<IPatientRepository, SqlPatientRepository>();
builder.Services.AddScoped<IPhysicianRepository, SqlPhysicianRepository>();
builder.Services.AddScoped<IMedicalAnalysisRepository, SqlMedicalAnalysisRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DBContext>();
    await db.Database.MigrateAsync();
    await SeedDatabase(db);
}

// Seed database method
static async Task SeedDatabase(DBContext context)
{
    // Check if data already exists
    if (await context.Patient.AnyAsync() || await context.Physician.AnyAsync())
    {
        return; // Database has been seeded
    }

    // Seed Patients
    var patients = new List<Patient>
    {
        new Patient("John", "Doe", "john.doe@email.com", "+1234567890")
        {
            BirthDate = new DateTime(1985, 5, 15),
            RootId = 1001
        },
        new Patient("Jane", "Smith", "jane.smith@email.com", "+1234567891")
        {
            BirthDate = new DateTime(1990, 8, 22),
            RootId = 1002
        },
        new Patient("Bob", "Johnson", "bob.johnson@email.com", "+1234567892")
        {
            BirthDate = new DateTime(1978, 12, 3),
            RootId = 1003
        },
        new Patient("Alice", "Brown", "alice.brown@email.com", "+1234567893")
        {
            BirthDate = new DateTime(1995, 3, 18),
            RootId = 1004
        }
    };

    // Seed Physicians
    var physicians = new List<Physician>
    {
        new Physician("Dr. Sarah", "Wilson", "sarah.wilson@hospital.com", "+1234567900")
        {
            BirthDate = new DateTime(1975, 7, 10),
            RootId = 2001,
            Specialization = SpecializationEnum.CARDIOLIGY
        },
        new Physician("Dr. Michael", "Davis", "michael.davis@hospital.com", "+1234567901")
        {
            BirthDate = new DateTime(1980, 11, 25),
            RootId = 2002,
            Specialization = SpecializationEnum.INTERNAL_MEDICINE
        },
        new Physician("Dr. Emily", "Garcia", "emily.garcia@hospital.com", "+1234567902")
        {
            BirthDate = new DateTime(1982, 4, 14),
            RootId = 2003,
            Specialization = SpecializationEnum.PEDIATRICS
        },
        new Physician("Dr. David", "Martinez", "david.martinez@hospital.com", "+1234567903")
        {
            BirthDate = new DateTime(1977, 9, 8),
            RootId = 2004,
            Specialization = SpecializationEnum.NEUROLOGY
        }
    };

    // Add to context
    await context.Patient.AddRangeAsync(patients);
    await context.Physician.AddRangeAsync(physicians);
    await context.SaveChangesAsync();

    // Seed Appointments (after patients and physicians are saved)
    var appointments = new List<Appointment>
    {
        new Appointment(3001, DateTime.Now.AddDays(1).AddHours(9)), // Tomorrow at 9 AM
        new Appointment(3002, DateTime.Now.AddDays(1).AddHours(14)), // Tomorrow at 2 PM
        new Appointment(3003, DateTime.Now.AddDays(2).AddHours(10)), // Day after tomorrow at 10 AM
        new Appointment(3004, DateTime.Now.AddDays(3).AddHours(11)), // 3 days from now at 11 AM
        new Appointment(3005, DateTime.Now.AddDays(5).AddHours(15)), // 5 days from now at 3 PM
    };

    await context.Appointment.AddRangeAsync(appointments);
    await context.SaveChangesAsync();

    Console.WriteLine("Database seeded successfully!");
    Console.WriteLine($"Added {patients.Count} patients");
    Console.WriteLine($"Added {physicians.Count} physicians");
    Console.WriteLine($"Added {appointments.Count} appointments");
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
