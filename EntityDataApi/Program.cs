using EntityDataApi.Data;
using EntityDataApi.Helpers;
using EntityDataApi.IRepositories;
using EntityDataApi.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEntitiesRepository, EntitiesRepository>();
builder.Services.AddScoped<RetryHelper>();

// Sqlite
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// SqlServer
builder.Services.AddDbContext<ApplicationDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerCon")));

// adding retry options 
builder.Services.Configure<RetryPolicyOptions>(builder.Configuration.GetSection("RetryPolicy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// data seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    var dataSeeder = new DataSeeder(dbContext);
    dataSeeder.Seed();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
