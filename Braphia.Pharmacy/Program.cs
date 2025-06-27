using Braphia.Pharmacy.Consumers;
using Braphia.Pharmacy.Database;
using Braphia.Pharmacy.Repositories;
using Braphia.Pharmacy.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var connectionString = builder.Configuration.GetConnectionString("PharmacyDB") ??
                       throw new InvalidOperationException("No connection string configured");

builder.Services
    .AddDbContext<DBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var rabbitMqConnection = configuration.GetConnectionString("eventbus");
        cfg.Host(rabbitMqConnection);

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IMedicationOrderRepository, SqlMedicationOrderRepository>();
builder.Services.AddScoped<IMedicationRepository, SqlMedicationRepository>();
builder.Services.AddScoped<IPatientRepository, SqlPatientRepository>();
builder.Services.AddScoped<IPharmacyRepository, SqlPharmacyRepository>();
builder.Services.AddScoped<IPrescriptionRepository, SqlPrescriptionRepository>();

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
    app.UseSwaggerUI(c =>
    {
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
