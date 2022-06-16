using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ParkyAPI;
using ParkyAPI.Data;
using ParkyAPI.ParkyMapper;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(ParkyMappings));
builder.Services.AddScoped<INationalParkRepository,NationalParkRepository>();
builder.Services.AddScoped<ITrailRepository, TrailRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

//builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("ParkyOpenAPISpec",
//                                    new Microsoft.OpenApi.Models.OpenApiInfo()
//                                    {
//                                        Title = "Parky API",
//                                        Version = "1",
//                                        Description = "Parky API Description ...",
//                                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
//                                        {
//                                            Email = "scortarudaniel@yahoo.com",
//                                            Name = "Scortaru Daniel",
//                                            Url = new Uri("https://www.exemplu.ro")
//                                        },
//                                        License = new Microsoft.OpenApi.Models.OpenApiLicense()
//                                        {
//                                            Name = "MIT License",
//                                            Url = new Uri("https://en.wikipedia.org/wiki/MITLicense")
//                                        }
//                                    });

//    //options.SwaggerDoc("ParkyOpenAPISpecNationalParks",
//    //                                    new Microsoft.OpenApi.Models.OpenApiInfo()
//    //                                    {
//    //                                        Title = "Parky API National Parks",
//    //                                        Version = "1",
//    //                                        Description = "Parky National Parks API Description ...",
//    //                                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
//    //                                        { 
//    //                                            Email = "scortarudaniel@yahoo.com",
//    //                                            Name = "Scortaru Daniel",
//    //                                            Url = new Uri("https://www.exemplu.ro")
//    //                                        },
//    //                                        License = new Microsoft.OpenApi.Models.OpenApiLicense()
//    //                                        {
//    //                                            Name = "MIT License",
//    //                                            Url = new Uri("https://en.wikipedia.org/wiki/MITLicense")
//    //                                        }
//    //                                    });

//    //options.SwaggerDoc("ParkyOpenAPISpecTrails",
//    //                                   new Microsoft.OpenApi.Models.OpenApiInfo()
//    //                                   {
//    //                                       Title = "Parky API Trails",
//    //                                       Version = "1",
//    //                                       Description = "Parky Trails API Description ...",
//    //                                       Contact = new Microsoft.OpenApi.Models.OpenApiContact()
//    //                                       {
//    //                                           Email = "scortarudaniel@yahoo.com",
//    //                                           Name = "Scortaru Daniel",
//    //                                           Url = new Uri("https://www.exemplu.ro")
//    //                                       },
//    //                                       License = new Microsoft.OpenApi.Models.OpenApiLicense()
//    //                                       {
//    //                                           Name = "MIT License",
//    //                                           Url = new Uri("https://en.wikipedia.org/wiki/MITLicense")
//    //                                       }
//    //                                   });

//    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    var cmlCommentFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
//    options.IncludeXmlComments(cmlCommentFullPath);
//});

builder.Services.AddApiVersioning(
        options => 
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1,0);
            options.ReportApiVersions = true;
        });
builder.Services.AddVersionedApiExplorer(
    options => 
    {
        options.GroupNameFormat = "'v'VVV";
    });



var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => 
    {
        //options.SwaggerEndpoint("ParkyOpenAPISpec/swagger.json", "Parky API");
        //options.SwaggerEndpoint("ParkyOpenAPISpecNationalParks/swagger.json", "Parky National Parks API");
        //options.SwaggerEndpoint("ParkyOpenAPISpecTrails/swagger.json", "Parky Trails API");
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        }
    });
}
app.UseApiVersioning();
app.UseHttpsRedirection();

//app.UseSwagger();
//app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
