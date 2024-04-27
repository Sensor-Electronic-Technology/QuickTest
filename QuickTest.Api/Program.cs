using FastEndpoints;
using MongoDB.Driver;
using QuickTest.Api.Data;
using QuickTest.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var hostType = builder.Configuration["HostType"];
string connectionString = "";
switch (hostType) {
    case nameof(HostType.Pi):
        connectionString = builder.Configuration.GetConnectionString("PiConnection") ??
                           "mongodb://192.168.68.112:27017";
        break;
    case nameof(HostType.Windows):
    case nameof(HostType.Linux):
    default:
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                           "mongodb://172.20.3.41:27017";
        break;
}
builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints(o=>o.IncludeAbstractValidators = true);
builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructure();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseFastEndpoints();
/*// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseHttpsRedirection();

app.Run();
