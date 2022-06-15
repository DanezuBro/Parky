using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.ParkyMapper;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(ParkyMappings));
builder.Services.AddScoped<INationalParkRepository,NationalParkRepository>();


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("ParkyOpenAPISpec",
                                        new Microsoft.OpenApi.Models.OpenApiInfo()
                                        {
                                            Title = "Parky API",
                                            Version = "1",
                                            Description = "Parky API Description ...",
                                            Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                                            { 
                                                Email = "scortarudaniel@yahoo.com",
                                                Name = "Scortaru Daniel",
                                                Url = new Uri("https://www.exemplu.ro")
                                            },
                                            License = new Microsoft.OpenApi.Models.OpenApiLicense()
                                            {
                                                Name = "MIT License",
                                                Url = new Uri("https://en.wikipedia.org/wiki/MITLicense")
                                            }
                                        });

    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var cmlCommentFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
    options.IncludeXmlComments(cmlCommentFullPath);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint("ParkyOpenAPISpec/swagger.json","Parky API");
    });
}

app.UseHttpsRedirection();

//app.UseSwagger();
//app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
