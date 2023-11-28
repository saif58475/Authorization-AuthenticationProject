using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project.BL.DepartmentRepository;
using Project.BL.FileUpload;
using Project.BL.RoleRepository;
using Project.BL.UnitOfWork;
using Project.DAL.AppDBContext;
using Project.DAL.AutoMapper;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// this is connectionstring to the database
builder.Services.AddDbContext<ProjectDBContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
// this adds the configuration of the Authorization to the project 
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdmin", policy => policy.RequireRole("Super", "Admin"));
    options.InvokeHandlersAfterFailure = false;
});
// this adds the configuration of the Authentication to the project 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
// This will make swagger works only when recieving the tokens 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Structure BackEnd Project", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Jwt Authorization",
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
            new string[] {}
        }
    });
});

// ASP.NET Core Identity provides the IdentityUser class and contains properties to store user information such as UserName, PasswordHash
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
        //Password Settings
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireDigit = true;
    })
// We want to store and retrieve the User and Role information of the registered users using EntityFrameWork Core
// from the underlying SQL Server database. We specify this using AddEntityFrameworkStores<ApplicationDbContext>()
    .AddEntityFrameworkStores<ProjectDBContext>();

//This that allows the CROS Origins 
builder.Services.AddCors(options =>
{
    options.AddPolicy("Open Server", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
//Injecting the automapper in the pipeline
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Adding the unit of work to the DI Container
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IFURepository, FURepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
app.UseCors("Open Server");
app.MapControllers();

app.Run();


#region Alternatives
//Alternatives: In addition to AddIdentity, there’s also AddIdentityCore, which adds only the core parts of the Identity system, providing a                  more lightweight option if you don’t need the full services like user interface (UI) login functionality.
#endregion