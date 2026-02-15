
using Quantum.ApplicationService;
using XEvent.CheckIn.Bootstrapper;
using XEvent.Payment.Bootstrapper;
using XEvent.Reporting.Bootstrapper;
using XEvent.Reservation.Bootstrapper;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var bootstrappingMode = builder.Configuration.GetValue("BootstrappingMode", BootstrappingMode.Production);

Bootstrapper.Run(builder.Services, builder.Configuration, bootstrappingMode);
ReportingBootstrapper.Run(builder.Services, builder.Configuration, bootstrappingMode);
TicketBootstrapper.Run(builder.Services, builder.Configuration, bootstrappingMode);
PaymentBootstrapper.Run(builder.Services, builder.Configuration, bootstrappingMode);
ReservationBootstrapper.Run(builder.Services, builder.Configuration, bootstrappingMode);
CheckInBootstrapper.Run(builder.Services, builder.Configuration, bootstrappingMode);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();