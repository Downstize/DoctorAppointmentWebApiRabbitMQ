using DoctorAppointmentWebApi;
using DoctorAppointmentWebApi.ExceptionHandler;
using DoctorAppointmentWebApi.Filters;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var bus = RabbitHutch.CreateBus(builder.Configuration.GetConnectionString("AutoRabbitMQ"));
builder.Services.AddSingleton<IBus>(bus);
builder.Services.AddControllers()
    .AddMvcOptions(options =>
    {
        options.Filters.Add<ExceptionHandlers>();
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerOperationFromInterfaceFilter>();
});

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