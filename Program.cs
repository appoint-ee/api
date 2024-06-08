using api.Data;
using api.Services;
using Microsoft.EntityFrameworkCore;
using RestSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy",
        cp => cp.AllowAnyOrigin() // TODO: Rollback this
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IRestClient, RestClient>();

builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IGoogleEventService, GoogleEventService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseCors("MyCorsPolicy");

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
