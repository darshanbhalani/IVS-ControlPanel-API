using IVS_API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Text;

namespace IVS_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(
    option =>
    {
        option.AddPolicy(
        "AllowRequests", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    });

            var configuration = builder.Configuration;
            var dbConfiguration = configuration.GetSection("DBConfiguration").Get<DBConfiguration>();
            var connectionString = $"Host={dbConfiguration!.Host};Port={dbConfiguration.Port};Username={dbConfiguration.Username};Password={dbConfiguration.Password};Database={dbConfiguration.Database}";

            try
            {
                using (var tempConnection = new NpgsqlConnection(connectionString))
                {
                    tempConnection.Open();
                    if (tempConnection.State == System.Data.ConnectionState.Open)
                    {
                        tempConnection.Close();
                        Console.WriteLine("-------------------------------------------------------------------------");
                        Console.WriteLine("Database connection successful!");
                        Console.WriteLine("-------------------------------------------------------------------------");
                        builder.Services.AddScoped<NpgsqlConnection>(_ => new NpgsqlConnection(connectionString));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("-------------------------------------------------------------------------");
                Console.WriteLine("Database connection error...");
                Console.WriteLine("Error : " + ex.Message);
                Console.WriteLine("-------------------------------------------------------------------------");

            }
            catch (Exception exx)
            {
                Console.WriteLine("-------------------------------------------------------------------------");
                Console.WriteLine("Unkown error...");
                Console.WriteLine("Error : " + exx.Message);
                Console.WriteLine("-------------------------------------------------------------------------");

            }
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });

                // Configure JWT Bearer authentication in Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                   
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JWT:Key").Value!))
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowRequests");

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
