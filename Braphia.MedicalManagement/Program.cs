using Microsoft.EntityFrameworkCore;
using Braphia.MedicalManagement.Database;
using Braphia.MedicalManagement.Repositories;
using MassTransit;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Braphia.MedicalManagement.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MedicalDB") ??
                       throw new InvalidOperationException("No connection string configured");

builder.Services
    .AddDbContext<DBContext>(options => options.UseSqlServer(connectionString));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new Braphia.MedicalManagement.Converters.DecimalJsonConverter());
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MedicalManagementMessageConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var rabbitMqConnection = configuration.GetConnectionString("eventbus");
        cfg.Host(rabbitMqConnection);
        cfg.ConfigureJsonSerializerOptions(options =>
        {
            options.Converters.Add(new Braphia.MedicalManagement.Converters.DecimalJsonConverter());
            return options;
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IPrescriptionRepository, SqlPrescriptionRepository>();
builder.Services.AddScoped<IPatientRepository, SqlPatientRepository>();
builder.Services.AddScoped<IPhysicianRepository, SqlPhysicianRepository>();
builder.Services.AddScoped<IMedicalAnalysisRepository, SqlMedicalAnalysisRepository>();
builder.Services.AddScoped<ITestRepository, SqlTestRepository>();
builder.Services.AddScoped<IAppointmentRepository, SqlAppointmentRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DBContext>();
    await db.Database.MigrateAsync();
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
