using Booking.Server.DbContexts;
using Booking.Server.Entities;
using Booking.Server.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddDbContext<RoomContext>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.MapFallbackToFile("/index.html");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomContext>();
    db.Database.Migrate();

    SeedDatabase(db);
}

app.Run();

void SeedDatabase(RoomContext context)
{

    context.Database.EnsureCreated();

    SeedRoom(context, new HotelRoomEntity { Id = 1, NightlyPrice = 50, RoomNumber = 1, SleepSpotCount = 2 });
    SeedRoom(context, new HotelRoomEntity { Id = 2, NightlyPrice = 60, RoomNumber = 2, SleepSpotCount = 3 });
    SeedRoom(context, new HotelRoomEntity { Id = 3, NightlyPrice = 40, RoomNumber = 3, SleepSpotCount = 1 });

    SeedCustomer(context, new CustomerEntity { IdCode = 10001010013, Email = "test@test.test", FirstName = "Test", LastName = "Test" });

    SeedReservation(
        context, 
        new RoomReservationEntity { 
            Id = 1, 
            HotelRoomId = 1, 
            ReservationStartDate = DateTime.UtcNow.Date, 
            ReservationEndDate = DateTime.UtcNow.AddDays(1).Date,
            ReserverIdCode = 10001010013
        });
}

void SeedRoom(RoomContext context, HotelRoomEntity entity)
{
    var room = context.Rooms.FirstOrDefault(r => r.Id == entity.Id);
    if (room == null)
        context.Rooms.Add(entity);

    context.SaveChanges();
}

void SeedReservation(RoomContext context, RoomReservationEntity reservation)
{
    var entity = context.Reservations.FirstOrDefault(r => r.Id == reservation.Id);
    if (entity == null)
        context.Reservations.Add(reservation);

    context.SaveChanges();
}

void SeedCustomer(RoomContext context, CustomerEntity customer)
{
    var entity = context.Customers.FirstOrDefault(r => r.IdCode == customer.IdCode);
    if (entity == null)
        context.Customers.Add(customer);

    context.SaveChanges();
}