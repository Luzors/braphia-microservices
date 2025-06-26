using Braphia.UserManagement.Consumers;
using Braphia.UserManagement.Database;
using Braphia.UserManagement.Repositories;
using Braphia.UserManagement.Repositories.Interfaces;
using Braphia.UserManagement.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var connectionString = builder.Configuration.GetConnectionString("UserDB") ??
                       throw new InvalidOperationException("No connection string configured");

builder.Services
    .AddDbContext<DBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ExternalUserFetchedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var rabbitMqConnection = configuration.GetConnectionString("eventbus");
        cfg.Host(rabbitMqConnection);

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IPatientRepository, SqlPatientRepository>();
builder.Services.AddScoped<IPhysicianRepository, SqlPhysicianRepository>();
builder.Services.AddScoped<IReceptionistRepository, SqlReceptionistRepository>();
builder.Services.AddScoped<IGeneralPracticionerRepository, SqlGeneralPracticionerRepository>();
builder.Services.AddScoped<IReferralRepository, SqlReferralRepository>();

builder.Services.AddHostedService<ExternalUserSyncService>();

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
