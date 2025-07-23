using DotNetEnv;
using Hospital_Management;
using Hospital_Management.Models;
using Hospital_Management.Services;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
Env.Load();
//using IEmailService = NETCore.MailKit.Core.IEmailService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbcon")));

builder.Services.Configure<SmtpSetting>(builder.Configuration.GetSection("SmtpSetting"));

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])
            ),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

//Register your custom services
builder.Services.AddScoped<Ipatientservice, Patientservice>();
builder.Services.AddScoped<IdoctorService, DoctorService>();
builder.Services.AddScoped<IdepartmentService, DepartmentService>();
builder.Services.AddScoped<IdoctorOnLeaveService, DoctorOnLeaveService>();
builder.Services.AddScoped<IprescriptionService, PrescriptionService>();
builder.Services.AddScoped<IadminService, AdminService>();
builder.Services.AddScoped<IreceptionistService, ReceptionistService>();
builder.Services.AddScoped<IauthService, AuthService>();
builder.Services.AddScoped<IappointmentService, Appiontmentservice>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IdashBoardServive, DashboardService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

//Add Swagger and configure it to support JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hospital Management API", Version = "v1" });

    // JWT Auth configuration
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter 'Bearer {your JWT token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
            //Array.Empty<string>()
        }
    });
});

var app = builder.Build();

//Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseExceptionHandler();
//app.UseExceptionHandler(); // Optional base handler
//app.UseExceptionHandler<GlobalExceptionHandler>();

app.UseHttpsRedirection();

//Middleware order matters!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
