using Braphia.Accounting.Consumers;
using Braphia.Accounting.Database;
using Braphia.Accounting.EventSourcing.Repositories;
using Braphia.Accounting.EventSourcing.Services;
using Braphia.Accounting.Repositories;
using Braphia.Accounting.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var connectionString = builder.Configuration.GetConnectionString("AccountingDB") ??
                       throw new InvalidOperationException("No connection string configured");

builder.Services
    .AddDbContext<AccountingDBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AccountingMessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var rabbitMqConnection = configuration.GetConnectionString("eventbus");
        cfg.Host(rabbitMqConnection);

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IPatientRepository, SqlPatientRepository>();
builder.Services.AddScoped<IInsurerRepository, SqlInsurerRepository>();
builder.Services.AddScoped<IInvoiceRepository, SqlInvoiceRepository>();
builder.Services.AddScoped<IEventStoreRepository, SqlEventStoreRepository>();
builder.Services.AddScoped<IInvoiceEventService, InvoiceEventService>();

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
