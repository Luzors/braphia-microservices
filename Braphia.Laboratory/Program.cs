using MassTransit;
using Microsoft.EntityFrameworkCore;
using Braphia.Laboratory.Database;
using Braphia.Laboratory.Repositories;
using Braphia.Laboratory.Repositories.Interfaces;
using Braphia.Laboratory.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var connectionString = builder.Configuration.GetConnectionString("LaboratoryDb") ??
                           throw new InvalidOperationException("No connection string configured");

builder.Services
    .AddDbContext<DBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<LaboratoryMessageConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var rabbitMqConnection = configuration.GetConnectionString("eventbus");
        cfg.Host(rabbitMqConnection);
        
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<ITestRepository, SqlTestRepository>();
builder.Services.AddScoped<ICentralLabotoryRepository, SqlCentralLabotoryRepository>();
builder.Services.AddScoped<IPatientRepository, SqlPatientRepository>();


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