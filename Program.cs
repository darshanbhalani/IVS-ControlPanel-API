using IVS_API.Models;
using Npgsql;

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
            catch (Exception ex)
            {
                Console.WriteLine("-------------------------------------------------------------------------");
                Console.WriteLine("Database connection error...");
                Console.WriteLine("Error : " + ex.Message);
                Console.WriteLine("-------------------------------------------------------------------------");

            }


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
