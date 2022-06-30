using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ParkyAPI;
using ParkyAPI.Data;
using ParkyAPI.ParkyMapper;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(ParkyMappings));
builder.Services.AddScoped<INationalParkRepository,NationalParkRepository>();
builder.Services.AddScoped<ITrailRepository, TrailRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(
    
    options =>
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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { 
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then yout token in the next input below. \r\n\r\n Example: \"Bearer 12345abcdef\" '",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme="Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
        { 
            new OpenApiSecurityScheme{ 
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme="oauth2",
                    Name="Bearer",
                    In = ParameterLocation.Header
            },
            new List<string>()
        }
    });



    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var cmlCommentFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
    options.IncludeXmlComments(cmlCommentFullPath);
});

    var appSettingsSection = builder.Configuration.GetSection("AppSettings");
    builder.Services.Configure<AppSettings>(appSettingsSection);

    var appSettings = appSettingsSection.Get<AppSettings>();
    var key = Encoding.ASCII.GetBytes(appSettings.Secret);

    builder.Services.AddAuthentication(x => {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x => {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                { 
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                };
    });

//builder.Services.AddApiVersioning(
//        options => 
//        {
//            options.AssumeDefaultVersionWhenUnspecified = true;
//            options.DefaultApiVersion = new ApiVersion(1,0);
//            options.ReportApiVersions = true;
//        });
//builder.Services.AddVersionedApiExplorer(
//    options => 
//    {
//        options.GroupNameFormat = "'v'VVV";
//    });



var app = builder.Build();
//var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint("ParkyOpenAPISpec/swagger.json", "Parky API");
        //options.SwaggerEndpoint("ParkyOpenAPISpecNationalParks/swagger.json", "Parky National Parks API");
        //options.SwaggerEndpoint("ParkyOpenAPISpecTrails/swagger.json", "Parky Trails API");

        //foreach (var desc in provider.ApiVersionDescriptions)
        //{
        //    options.SwaggerEndpoint($"{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        //}
    });
}
//app.UseApiVersioning();
app.UseHttpsRedirection();

//app.UseSwagger();
//app.UseSwaggerUI();
app.UseRouting();
app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
