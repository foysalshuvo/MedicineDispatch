using AutoWrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Services.Drones;
using UnitOfWork.Interfaces;
using UnitOfWork.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "ASP.NET Core Web API v1",
        Title = "Medicine Dispatch",
        Description = "Practice Test",
        Contact = new OpenApiContact
        {
            Name = "Foysal Alam",
            Email = "foysal.shuvo@live.com"
        },
        License = new OpenApiLicense
        {
            Name = "Self"
        }
    });
});
builder.Services.AddTransient<IUnitOfWork, UnitOfWorkSqlServer>();
builder.Services.AddTransient<IDroneService, DroneService>();
builder.Services.AddTransient<IDispatchMedicineService, DispatchMedicineService>();

builder.Services.AddMvc(options =>
{
    options.FormatterMappings.SetMediaTypeMappingForFormat
        ("xml", MediaTypeHeaderValue.Parse("application/xml"));
    options.FormatterMappings.SetMediaTypeMappingForFormat
        ("config", MediaTypeHeaderValue.Parse("application/xml"));
    options.FormatterMappings.SetMediaTypeMappingForFormat
        ("js", MediaTypeHeaderValue.Parse("application/json"));
    options.EnableEndpointRouting = false;
}).AddXmlSerializerFormatters();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medicine Dispatch API V1");
    });
    app.UseDeveloperExceptionPage();
}
app.UseMvc();
app.UseApiResponseAndExceptionWrapper();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapControllers();

app.Run();
