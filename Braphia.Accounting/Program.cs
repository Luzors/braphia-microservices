using Braphia.Accounting.Database;
using Braphia.Accounting.Repositories;
using Braphia.Accounting.Repositories.Interfaces;
using Braphia.Accounting.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var connectionString = builder.Configuration.GetConnectionString("AccountingDB") ??
                       throw new InvalidOperationException("No connection string configured");

builder.Services
    .AddDbContext<AccountingDBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PatientCreatedConsumer>();
    x.AddConsumer<LabTestFinishedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var rabbitMqConnection = configuration.GetConnectionString("eventbus");
        cfg.Host(rabbitMqConnection);

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IInsurerRepository, InsurerRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AccountingDBContext>();
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
